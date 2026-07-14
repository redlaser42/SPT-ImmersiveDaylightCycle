using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Helpers;

using System.Reflection;


namespace ImmersiveDaylightCycleServer;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "Jehree.ImmersiveDaylightCycleServer";
    public override string Name { get; init; } = "Immersive Daylight Cycle Server";
    public override string Author { get; init; } = "Jehree";
    public override List<string>? Contributors { get; init; }
    public override SemanticVersioning.Version Version { get; init; } = new("1.0.2");
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
    public override List<string>? Incompatibilities { get; init; }
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
    public override string? Url { get; init; }
    public override bool? IsBundleMod { get; init; }
    public override string License { get; init; } = "MIT";
 }

//Class definitions:

public class IDCRaid
{
    public string RaidId { get; set; }
    public Dictionary<string, IDCClientExitInfoRequest> ClientExits { get; set; }
    public IDCRaid(string raidId)
    {
        RaidId = raidId;
        ClientExits = new Dictionary<string, IDCClientExitInfoRequest>();
    }
}

public class IDCClientExitInfoRequest : IRequestData
{
    public string RaidId { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public ExitStatus ExitStatus { get; set; }
    public bool IsHost { get; set; }
    public bool IsDedicatedClient { get; set; }
    public int SecondsInRaid { get; set; }
}

public class IDCHostRaidStartedRequest : IRequestData
{
    public string Data { get; set; } = string.Empty;
}

public class IDCTime
{
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Second { get; set; }
    public float CycleRate { get; set; }
}

public class IDCCommand : IRequestData
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}