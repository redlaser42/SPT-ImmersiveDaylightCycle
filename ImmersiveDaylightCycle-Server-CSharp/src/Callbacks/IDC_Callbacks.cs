using IDC_Utils;
using ImmersiveDaylightCycleServer;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;
using System.Text.Json;

namespace IDC.Callbacks;


[Injectable]
public class IDC_Callbacks(ISptLogger<IDC_Callbacks> logger, HttpResponseUtil httpResponseUtil, IDC_TimeManager timeUtils, IDC_RaidManager raidManager)
{
    public ValueTask<string> HandleGetTime(string url, EmptyRequestData info, MongoId sessionID)
    {
        return new ValueTask<string>(httpResponseUtil.NoBody(timeUtils.GetCurrentTime()));
    }
    public ValueTask<string> HandleHostRaidStarted(string url, IDCHostRaidStartedRequest info, MongoId sessionID)
    {
        logger.Error(JsonSerializer.Serialize(info));
        //timeUtils.OnHostRaidStarted(info.Data);

        return new ValueTask<string>(httpResponseUtil.NullResponse());
    }
    public ValueTask<string> HandleClientExtracted(string url, IDCClientExitInfoRequest info, MongoId sessionID)
    {
        logger.Error(JsonSerializer.Serialize(info));
       // timeUtils.OnClientRaidExited(info);
        return new ValueTask<string>(httpResponseUtil.NullResponse());
    }
    public ValueTask<string> HandleLocalRaidStart(string url, StartLocalRaidRequestData info, MongoId sessionID)
    {
        return new ValueTask<string>(httpResponseUtil.NullResponse());
        raidManager.OnLocalRaidStarted(info);
    }
    public ValueTask<string> HandleLocalRaidEnd(string url, EndLocalRaidRequestData info, MongoId sessionID)
    {
        raidManager.OnLocalRaidEnded(info);
        return new ValueTask<string>(httpResponseUtil.NullResponse());
    }
}
