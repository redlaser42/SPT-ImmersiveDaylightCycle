using IDC.Callbacks;
using ImmersiveDaylightCycleServer;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Models.Eft.Match;
using SPTarkov.Server.Core.Utils;

[Injectable]
public class CustomStaticRouter(JsonUtil jsonUtil,IDC_Callbacks IDCCallbacks) : StaticRouter(jsonUtil, [

                new RouteAction<IDCHostRaidStartedRequest>(
                "/jehree/idc/host_raid_started",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await IDCCallbacks.HandleHostRaidStarted(url, info, sessionId)
            ),
                new RouteAction<IDCClientExitInfoRequest>(
                "/jehree/idc/client_exited",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await IDCCallbacks.HandleClientExtracted(url, info, sessionId)
            ),
                new RouteAction<EmptyRequestData>(
                "/jehree/idc/request_time",
                async (
                    url,
                    info,
                    sessionId,
                    output
                ) => await IDCCallbacks.HandleGetTime(url, info, sessionId)
            )
        ])
    {   
}
