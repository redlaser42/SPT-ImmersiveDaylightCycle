using Comfort.Common;
using Fika.Core.Main.Utils;
using Fika.Core.Networking;
using ImmersiveDaylightCycle.Fika;


namespace ImmersiveDaylightCycle.FikaModule;

internal class Main
{
    public static void Init()
    {
        FikaBridge.IAmHostEmitted += IAmHost;
        FikaBridge.GetRaidIdEmitted += GetRaidId;
    }

    public static bool IAmHost()
    {
        return Singleton<FikaServer>.Instantiated;
    }

    public static string GetRaidId()
    {
        return FikaBackendUtils.GroupId;
    }
}
