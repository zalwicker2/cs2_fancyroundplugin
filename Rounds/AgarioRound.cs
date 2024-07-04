using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class AgarioRound : BaseRound
{
    BasePlugin host;
    public AgarioRound(BasePlugin host)
    {
        this.host = host;
    }

    HookResult OnDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        CCSPlayerController attacker = @event.Attacker;
        var playerPawn = attacker.PlayerPawn!.Value!;
        playerPawn.CBodyComponent!.SceneNode!.Scale = 1 + playerPawn.NumEnemiesKilledThisRound * .5f;
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
        return HookResult.Continue;
    }

    public override string GetRoundName()
    {
        return "Agar.io Round";
    }

    public override string GetRoundDescription()
    {
        return "Gain Size with Each Kill";
    }
    public override void OnRoundStart()
    {
        host.RegisterEventHandler<EventPlayerDeath>(OnDeath);
    }
    public override void OnRoundEnd()
    {
        host.DeregisterEventHandler<EventPlayerDeath>(OnDeath);
    }
}