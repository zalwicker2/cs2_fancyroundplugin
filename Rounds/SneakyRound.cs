using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

class SneakyRound : BaseRound
{


    HookResult OnEquip(EventItemEquip @event, GameEventInfo info)
    {
        if (@event.Item == "knife")
        {
            Util.HidePlayer(@event.Userid.PlayerPawn.Value!);
            Console.WriteLine(@event.Userid.PlayerPawn.Value!.Speed);
        }
        else
        {
            Util.ShowPlayer(@event.Userid.PlayerPawn.Value!);
        }
        return HookResult.Continue;
    }

    HookResult OnShot(EventPlayerHurt @event, GameEventInfo info)
    {
        Console.WriteLine("attack");
        Util.ShowPlayer(@event.Userid.PlayerPawn.Value!);
        return HookResult.Continue;
    }

    HookResult OnFlash(EventPlayerBlind @event, GameEventInfo info)
    {
        Util.ShowPlayer(@event.Userid.PlayerPawn.Value!);
        return HookResult.Continue;
    }

    BasePlugin host;
    public SneakyRound(BasePlugin host)
	{
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Sneaky";
    }

    public override string GetRoundDescription()
    {
        return "Turn invisible when you have your knife out, but become visible whenever you get flashed or take damage.";
    }

    public override void OnFreezeEnd()
	{
        host.RegisterEventHandler<EventItemEquip>(OnEquip);
        host.RegisterEventHandler<EventPlayerHurt>(OnShot);
        host.RegisterEventHandler<EventPlayerBlind>(OnFlash);
    }

	public override void OnRoundEnd()
    {
        host.DeregisterEventHandler<EventItemEquip>(OnEquip);
        host.DeregisterEventHandler<EventPlayerHurt>(OnShot);
        host.DeregisterEventHandler<EventPlayerBlind>(OnFlash);
        foreach (CCSPlayerController plr in Utilities.GetPlayers())
        {
            Util.ShowPlayer(plr.PlayerPawn.Value);
        }
    }
}