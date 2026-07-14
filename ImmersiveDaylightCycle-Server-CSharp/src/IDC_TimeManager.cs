using ImmersiveDaylightCycleServer;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Models.Eft.Match;

using System.Reflection;
using System.Text.Json;
using IDC_Config;

namespace IDC_Utils
{
    [Injectable]
    public class IDC_TimeManager(ISptLogger<IDC_TimeManager> logger, ModHelper modHelper, IDCConfigManager configManager, IDC_RaidManager raidManager)
    {
        private readonly string _runtimeDataPath = Path.Join(modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly()), "runtime_data");

        public string RaidTime => Path.Combine(_runtimeDataPath, "time.json");


        public IDCTime GetCurrentTime()
        {
            var currentTime = JsonSerializer.Deserialize<IDCTime>(File.ReadAllText(RaidTime))!;
            return currentTime;
        }

        private void SetCurrentTime(IDCTime newTime)
        {
            File.WriteAllText(RaidTime, JsonSerializer.Serialize(newTime));
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


        public void DoTimeJump()
        {
            var config = configManager.GetConfig();
            AddSeconds(config.RaidExitTimeJump * 3600);
        }

        public void AddSeconds(double seconds)
        {
            var time = GetCurrentTime();

            var dateTime = new DateTime(
                2024,
                1,
                1,
                time.Hour,
                time.Minute,
                time.Second);

            dateTime = dateTime.AddSeconds(seconds);

            time.Hour = dateTime.Hour;
            time.Minute = dateTime.Minute;
            time.Second = dateTime.Second;

            SetCurrentTime(time);
        }

        public void ResetCurrentTime()
        {
            SetCurrentTime(GetResetTime());
        }

        private IDCTime GetResetTime()
        {
            var config = configManager.GetConfig();

            return new IDCTime
            {
                Hour = config.ResetHour,
                Minute = config.ResetMinute,
                Second = config.ResetSecond,
                CycleRate = config.CycleRate
            };
        }
              
    }
}
