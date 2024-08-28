using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

class TeleportOnKill : BaseRound
{
    private HookResult OnDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var attacker = @event.Attacker!;
        var victim = @event.Userid!;

        var attackerPawn = attacker.PlayerPawn.Value!;
        var victimPawn = victim.PlayerPawn.Value!;

        if(victimPawn != null && attackerPawn != null)
        {
            attackerPawn.Teleport(victimPawn.AbsOrigin, victimPawn.AbsRotation);
        }
        
        return HookResult.Continue;
    }

    BasePlugin host;
    public TeleportOnKill(BasePlugin host)
	{
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Teleport on Kill";
    }

    public override string GetRoundDescription()
    {
        return "Teleport to the position of the player you kill.";
    }

    public override void OnFreezeEnd()
	{
        host.RegisterEventHandler<EventPlayerDeath>(OnDeath);
    }

	public override void OnRoundEnd()
    {
        host.DeregisterEventHandler<EventPlayerDeath>(OnDeath);
    }
}