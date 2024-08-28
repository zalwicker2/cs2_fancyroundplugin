using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class ZoomedOutRound : BaseRound
{
    BasePlugin host;
    public ZoomedOutRound(BasePlugin host)
    {
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Zoomed Out";
    }

    public override string GetRoundDescription()
    {
        return "High FOV round.";
    }

    public override void OnRoundStart()
    {

    }

    public override void PlayerCommands(CCSPlayerController plr)
    {
        plr.DesiredFOV = 150;
    }

    public override void OnRoundEnd()
    {
        foreach (CCSPlayerController plr in Utilities.GetPlayers())
        {
            plr.DesiredFOV = 0;
        }
    }
}