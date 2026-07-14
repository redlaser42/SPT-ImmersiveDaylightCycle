using SPT.Reflection.Utils;

namespace ImmersiveDaylightCycle.Fika;

public class FikaBridge
{
    public delegate bool SimpleBoolReturnEvent();
    public delegate string SimpleStringReturnEvent();

    public static event SimpleBoolReturnEvent IAmHostEmitted;
    public static bool IAmHost()
    {
        bool? eventResponse = IAmHostEmitted?.Invoke();

        if (eventResponse == null)
        {
            return true;
        }
        else
        {
            return eventResponse.Value;
        }
    }


    public static event SimpleStringReturnEvent GetRaidIdEmitted;
    public static string GetRaidId()
    {
        string eventResponse = GetRaidIdEmitted?.Invoke();

        if (eventResponse == null)
        {
            return ClientAppUtils.GetMainApp().GetClientBackEndSession().Profile.ProfileId;
        }
        else
        {
            return eventResponse;
        }
    }
}