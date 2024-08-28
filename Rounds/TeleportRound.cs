using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

class TeleportRound : BaseRound
{
    private HookResult TeleportOnPing(EventPlayerPing @event, GameEventInfo info)
    {
        Vector pingLocation = new Vector(@event.X, @event.Y, @event.Z);
        if (@event.Userid!.PlayerPawn.Value != null)
        {
            Vector diff = @event.Userid!.PlayerPawn.Value!.AbsOrigin! - pingLocation;
            int length = 50;
            Vector unit = new Vector(length * diff.X / diff.Length(), length * diff.Y / diff.Length(), length * diff.Z / diff.Length());
            if (@event.Z - @event.Userid!.PlayerPawn.Value!.AbsOrigin!.Z < 300)
            {
                @event.Userid!.PlayerPawn.Value!.Teleport(new Vector(@event.X, @event.Y, @event.Z + 16f) + unit);
            }
        }

        return HookResult.Continue;
    }

    BasePlugin host;
    public TeleportRound(BasePlugin host)
    {
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Teleport";
    }

    public override string GetRoundDescription()
    {
        return "Teleport to wherever you ping, but beware of the cooldown.";
    }

    public override void OnFreezeEnd()
    {
        host.RegisterEventHandler<EventPlayerPing>(TeleportOnPing);
    }

    public override void OnRoundEnd()
    {
        host.DeregisterEventHandler<EventPlayerPing>(TeleportOnPing);
    }
}