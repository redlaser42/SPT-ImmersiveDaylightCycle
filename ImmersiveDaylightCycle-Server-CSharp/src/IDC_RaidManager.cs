using ImmersiveDaylightCycleServer;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;
using System.Reflection;
using System.Text.Json;
using IDC_Config;

namespace IDC_Utils
{
    public class IDC_RaidManager(ISptLogger<IDC_RaidManager> logger, ModHelper modHelper, ProfileHelper profileHelper, IDCConfigManager configManager, IDC_TimeManager timeUtil)
    {
        
            private readonly string _runtimeDataPath = Path.Join(modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly()), "runtime_data");

            private string RaidPath => Path.Combine(_runtimeDataPath, "raid_session.json");
            

            public bool RaidFileExists()
            {
                return File.Exists(RaidPath);
            }

            public void CreateNewRaidFile(IDCRaid raid)
            {
                File.WriteAllText(
                    RaidPath,
                    JsonSerializer.Serialize(raid, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));
            }

            public IDCRaid CreateNewRaidSession(string raidID)
            {
                return new IDCRaid(raidID);
            }

            public IDCRaid GetRaidSession()
            {
                string json = File.ReadAllText(RaidPath);
                return JsonSerializer.Deserialize<IDCRaid>(json)!;
            }

            public void SaveRaid(IDCRaid raid)
            {
                File.WriteAllText(
                    RaidPath,
                    JsonSerializer.Serialize(
                        raid,
                        new JsonSerializerOptions
                        {
                            WriteIndented = true
                        }));
                logger.Info("IDC: Raid Saved");

            }

            public void CleanUpRaidSession()
            {
                if (!RaidFileExists())
                {
                    return;
                }
                File.Delete(RaidPath);
            }

        private bool AnyClientHasExitStatus(IDCRaid raid, ExitStatus exitStatus)
            {
                foreach (var exitInfo in raid.ClientExits.Values)
                {
                    if (exitInfo.IsDedicatedClient)
                    {
                        continue;
                    }

                    if (exitInfo.ExitStatus == exitStatus)
                    {
                        return true;
                    }
                }
                return false;
            }


            private void AddClientExitInfo(IDCClientExitInfoRequest info, IDCRaid raid)
            {
                raid.ClientExits[info.ProfileId] = info;
            }


            public void OnLocalRaidStarted(StartLocalRaidRequestData info)
            {
                var raid = CreateNewRaidSession(info.ServerId);

                logger.Error($"New Raid Id: '{info.ServerId}'");

                if (RaidFileExists())
                {

                    if (info.ServerId != GetRaidSession().RaidId)
                    {
                        logger.Error("Raid session cleanup needed! This usually means someone crashed. Cleaning...");

                        CleanUpRaidSession();
                    }
                    else
                    {
                        logger.Error($"Raid session already exists, raid {raid.RaidId} will not advance global time");

                        return;
                    }

                }
                CreateNewRaidFile(raid);
            }


            public void OnLocalRaidEnded(EndLocalRaidRequestData info)
            {
                logger.Error($"Seconds In Raid: '{info.Results.PlayTime}'");
                logger.Error($"Exit raid id: '{info.ServerId}'");
                double secondsInRaid = (double)info.Results.PlayTime;

                if (!RaidFileExists())
                {
                    return;
                }

                var raid = GetRaidSession();
                logger.Error($"Stored raid id: '{info.ServerId}'");
                if (raid.RaidId != "singleplayer" && raid.RaidId != info.ServerId)
                {
                    logger.Error("IDC: Exiting Before writting time");
                    return;
                }

                SaveRaid(raid);

                var config = configManager.GetConfig();

                bool anyoneDisconnected = AnyClientHasExitStatus(raid, ExitStatus.LEFT);

                bool anyoneDied = AnyClientHasExitStatus(raid, ExitStatus.KILLED);


                if ((config.TimeResetsOnDeath && anyoneDied) || (config.TimeResetsOnDisconnect && anyoneDisconnected))
                {
                timeUtil.ResetCurrentTime();
                }
                else
                {
                    timeUtil.AddSeconds(secondsInRaid);

                    if (secondsInRaid > 60 * 5)
                    {
                        timeUtil.DoTimeJump();
                    }
                }

                CleanUpRaidSession();
            }


            //Old methods that get raid information from client routes.............................................
            public void OnHostRaidStarted(string info)
            {
                var raid = CreateNewRaidSession(info);

                logger.Error($"New Raid Id: '{raid.RaidId}'");

                if (RaidFileExists())
                {
                    var profileIds = profileHelper.GetProfiles().Keys;

                    bool currentRaidSessionHostIsLoggedIn =
                        profileIds.Contains(GetRaidSession().RaidId);

                    if (!currentRaidSessionHostIsLoggedIn)
                    {
                        logger.Error("Raid session cleanup needed! This usually means someone crashed. Cleaning...");

                        CleanUpRaidSession();
                    }
                    else
                    {
                        logger.Error($"Raid session already exists, raid [{raid.RaidId}] will not advance global time");

                        return;
                    }
                }
                CreateNewRaidFile(raid);
            }

            public void OnClientRaidExited(IDCClientExitInfoRequest exitInfo)
            {
                logger.Error($"Exit raid id: '{exitInfo.RaidId}'");

                if (!RaidFileExists())
                {
                    return;
                }

                var raid = GetRaidSession();
                logger.Error($"Stored raid id: '{raid.RaidId}'");
                if (raid.RaidId != "singleplayer" && raid.RaidId != exitInfo.RaidId)
                {
                    logger.Error("IDC: Exiting Before writting time");
                    return;
                }

                AddClientExitInfo(exitInfo, raid);
                SaveRaid(raid);

                if (exitInfo.IsHost)
                {
                    var config = configManager.GetConfig();

                    bool anyoneDisconnected = AnyClientHasExitStatus(raid, ExitStatus.LEFT);

                    bool anyoneDied = AnyClientHasExitStatus(raid, ExitStatus.KILLED);


                    if ((config.TimeResetsOnDeath && anyoneDied) || (config.TimeResetsOnDisconnect && anyoneDisconnected))
                    {
                        timeUtil.ResetCurrentTime();
                    }
                    else
                    {
                        timeUtil.AddSeconds(exitInfo.SecondsInRaid);

                        if (exitInfo.SecondsInRaid > 60 * 5)
                        {
                            timeUtil.DoTimeJump();
                        }
                    }

                    CleanUpRaidSession();
                }
            }
        }

}
