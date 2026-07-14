using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Helpers;
using System.Reflection;
using System.Text.Json;

namespace IDC_Config
{
    [Injectable]
    public class IDCConfigManager
    {
        public readonly string _configPath;

        public IDCConfigManager(ModHelper modHelper)
        {
            _configPath = Path.Combine(
                modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly()),
                "config.json");
        }

        public IDCConfig GetConfig()
        {
            return JsonSerializer.Deserialize<IDCConfig>(
                File.ReadAllText(_configPath))!;
        }

        public record IDCConfig(
        int RaidExitTimeJump,
        float CycleRate,
        bool TimeResetsOnDeath,
        bool TimeResetsOnDisconnect,
        int ResetHour,
        int ResetMinute,
        int ResetSecond);
    }
}
