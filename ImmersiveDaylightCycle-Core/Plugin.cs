using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using EFT.UI;
using ImmersiveDaylightCycle.Common;
using Jehree.ImmersiveDaylightCycle.Helpers;
using Jehree.ImmersiveDaylightCycle.Patches;
using JsonType;
using System;
using System.Reflection;
using static Jehree.ImmersiveDaylightCycle.Patches.OfflineRaidEndedPatch;


namespace Jehree.ImmersiveDaylightCycle
{
    [BepInPlugin("Jehree.ImmersiveDaylightCycle", "Immersive Daylight Cycle", "2.1.1")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource { get; private set; }
        public static bool FikaInstalled { get; private set; }
        public static bool IAmDedicatedClient { get; private set; }
        public static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("IDC Log:");

        private void Awake()
        {
            FikaInstalled = Chainloader.PluginInfos.ContainsKey("com.fika.core");
            IAmDedicatedClient = Chainloader.PluginInfos.ContainsKey("com.fika.headless");

            Settings.Init(Config);

            if (FikaInstalled)
            {
                TryInitFikaModuleAssembly();

                if (IAmDedicatedClient)
                {
                    //Patches only for headless clients
                    //new FikaClient_PrintStatisticsPatch().Enable();
                    //new HeadlessGame_CreatePatch().Enable();

                }
                else
                {
                    //Patches only for player clients

                    new TimeUIPanelPatch().Enable();
                    new LocationConditionsPanelPatch().Enable();
                    new TimeUIUpdatePatch().Enable();
                }
            }
            //Patches for all clients
            //new OfflineRaidEndedPatch().Enable();
            new OnGameStartedPatch().Enable();
            ConsoleScreen.Processor.RegisterCommandGroup<CommandGroup>();
        }

        private void TryInitFikaModuleAssembly()
        {
            if (!FikaInstalled) return;
            Assembly fikaModuleAssembly = Assembly.Load("ImmersiveDaylightCycle-FikaModule");
            Type main = fikaModuleAssembly.GetType("ImmersiveDaylightCycle.FikaModule.Main");
            MethodInfo init = main.GetMethod("Init");
            init.Invoke(main, null);
        }


    }
}
