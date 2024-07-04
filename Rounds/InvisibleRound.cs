using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class InvisibleRound : BaseRound
{
    BasePlugin host;
    public InvisibleRound(BasePlugin host)
    {
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Invisible Round";
    }

    public override void OnRoundStart()
    {
    }

    public override void PlayerCommands(CCSPlayerController plr)
    {
        Util.HidePlayer(plr.PlayerPawn.Value!);
    }

    public override void OnRoundEnd()
    {

    }
}