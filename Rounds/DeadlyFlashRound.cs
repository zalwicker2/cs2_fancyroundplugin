using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

class DeadlyFlashRound : BaseRound
{
	BasePlugin host;
	public DeadlyFlashRound(BasePlugin host)
	{
		this.host = host;
	}

	private HookResult DamageOnBlind(EventPlayerBlind @event, GameEventInfo info)
	{
		var attacker = @event.Attacker!;
		var victim = @event.Userid!;
		var victimPawn = victim.PlayerPawn.Value!;

		if(attacker.Team == victim.Team && attacker != victim)
		{
			return HookResult.Continue;
		}
		int damage = (int)Math.Round(Math.Exp(.923f * @event.BlindDuration)); //(int)Math.Round(20f * @event.BlindDuration);
		Console.WriteLine(damage);
		if(attacker != victim)
		{
			damage *= 3;
		}
		int health = victimPawn.Health - damage;
		if(health <= 0)
		{
			try
			{
				if (victim.PawnIsAlive)
				{
					victim.CommitSuicide(true, true);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			
        } else {

			victimPawn.Health = health;
            Utilities.SetStateChanged(victimPawn, "CBaseEntity", "m_iHealth");
        }

		Console.WriteLine(@event.BlindDuration);

		return HookResult.Continue;
	}

	private HookResult KillOnFlashHit(EventPlayerHurt @event, GameEventInfo info)
	{
		if(@event.Weapon == "flashbang" && @event.Userid!.PawnIsAlive)
        {
            @event.Userid!.CommitSuicide(false, false);
        }
		return HookResult.Continue;
	}

    public override string GetRoundName()
    {
        return "Deadly Flash";
    }

    public override string GetRoundDescription()
    {
        return "Players take damage based on how flashed they are. Flashbang impacts are instant kills.";
    }

    public override void OnRoundStart()
    {
        Util.RemoveBreakables();
        host.RegisterEventHandler<EventGrenadeThrown>(Util.InfiniteGrenades);
		host.RegisterEventHandler<EventPlayerBlind>(DamageOnBlind);
        host.RegisterEventHandler<EventPlayerHurt>(KillOnFlashHit);
    }

	public override void PlayerCommands(CCSPlayerController plr)
    {
        Util.SetInventory(plr, ["weapon_flashbang"], true);
		plr.ExecuteClientCommand("slot7");
		plr!.InGameMoneyServices!.Account = 0;
	}

	public override void OnRoundEnd()
    {
        host.DeregisterEventHandler<EventGrenadeThrown>(Util.InfiniteGrenades);
        host.DeregisterEventHandler<EventPlayerBlind>(DamageOnBlind);
        host.DeregisterEventHandler<EventPlayerHurt>(KillOnFlashHit);
    }
}