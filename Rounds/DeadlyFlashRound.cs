using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class DeadlyFlashRound : BaseRound
{
	BasePlugin host;
	public DeadlyFlashRound(BasePlugin host)
	{
		this.host = host;
	}

	private HookResult DamageOnBlind(EventPlayerBlind @event, GameEventInfo info)
	{
		if(@event.Attacker.Team == @event.Userid.Team)
		{
			return HookResult.Continue;
		}
		int damage = (int)Math.Round(Math.Exp(.923f * @event.BlindDuration)) - 1; //(int)Math.Round(20f * @event.BlindDuration);
		Console.WriteLine(damage);
		@event.Userid!.PlayerPawn.Value!.Health = @event.Userid!.PlayerPawn.Value!.Health - damage;
		Utilities.SetStateChanged(@event.Userid.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
		if (@event.Userid.PlayerPawn.Value.Health <= 0)
		{
			@event.Userid.CommitSuicide(true, true);
		}
		//@event.Userid.PawnHealth = @event.Userid.PawnHealth - (uint) Math.Round(20 * @event.BlindDuration);
		Console.WriteLine(@event.BlindDuration);

		return HookResult.Continue;
	}

    public override string GetRoundName()
    {
        return "Deadly Flash";
    }

    public override string GetRoundDescription()
    {
        return "Players take damage based on how flashed they are.";
    }

    public override void OnRoundStart()
	{
		host.RegisterEventHandler<EventGrenadeThrown>(Util.InfiniteGrenades);
		host.RegisterEventHandler<EventPlayerBlind>(DamageOnBlind);
	}

	public override void PlayerCommands(CCSPlayerController plr)
    {
		Util.SetInventory(plr, ["weapon_flashbang"], true); 
		plr!.InGameMoneyServices!.Account = 0;
	}

	public override void OnRoundEnd()
	{
		host.DeregisterEventHandler<EventPlayerBlind>(DamageOnBlind);
	}
}