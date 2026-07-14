using EFT;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ImmersiveDaylightCycle.Common
{
    public class IDCClientExitInfoRequest
    {
        public string RaidId { get; set; } = string.Empty;
        public string ProfileId { get; set; } = string.Empty;
        public ExitStatus ExitStatus { get; set; }
        public bool IsHost { get; set; }
        public bool IsDedicatedClient { get; set; }
        public int SecondsInRaid { get; set; }
    }

    public class HostRaidStartedRequest
    {
        public string Data { get; set; }
    }

    public class IDCTime
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public float CycleRate { get; set; }
    }

    public class IDCCommand
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; } = "";

        public IDCCommand(string type)
        {
            Type = type;
        }
    }
}
