using HarmonyLib.Tools;
using IDC_Utils;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers.Http;
using SPTarkov.Server.Core.Utils;

namespace IDC_HTTPListener
{
    [Injectable(TypePriority = 0)]
    public class LocalRaidEnd_Listener(ISptLogger<LocalRaidEnd_Listener> logger, IDC_RaidManager raidManager) : IHttpListener 
    {
        public bool CanHandle(MongoId sessionId, HttpContext context)
        {
            return context.Request.Method == "GET" && context.Request.Path.Value!.Contains("/client/match/local/end");
        }

        public async Task Handle(MongoId sessionId, HttpContext context)
        {
            logger.Error("IDC: client/match/local/end Http Listener received");
            //raidManager.OnLocalRaidStarted(context);

            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync("[1] This is the first example of a mod hooking into the HttpServer"u8.ToArray());
            await context.Response.StartAsync();
            await context.Response.CompleteAsync();
        }
    }

    public class LocalRaidStarted_Listener(ISptLogger<LocalRaidStarted_Listener> logger, IDC_RaidManager raidManager) : IHttpListener
    {
        public bool CanHandle(MongoId sessionId, HttpContext context)
        {
            return context.Request.Method == "GET" && context.Request.Path.Value!.Contains("/client/match/local/start");
        }

        public async Task Handle(MongoId sessionId, HttpContext context)
        {
            logger.Error("IDC: client/match/local/start Http Listener received");
            //raidManager.OnLocalRaidStarted(context);

            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync("[1] This is the first example of a mod hooking into the HttpServer"u8.ToArray());
            await context.Response.StartAsync();
            await context.Response.CompleteAsync();
        }
    }
}
