using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class InstaKillNadesRound : BaseRound
{
    BasePlugin host;
    public InstaKillNadesRound(BasePlugin host)
	{
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Insta Kill Nades";
    }

    public override string GetRoundDescription()
    {
        return "All nades are instant kills.";
    }

    public override void OnRoundStart() {
        Server.ExecuteCommand("sv_hegrenade_damage_multiplier 100;");
        host.RegisterEventHandler<EventGrenadeThrown>(Util.InfiniteGrenades);
    }

	public override void PlayerCommands(CCSPlayerController plr)
	{
        Util.SetInventory(plr, ["weapon_hegrenade"], true);
		plr!.InGameMoneyServices!.Account = 0;
    }

	public override void OnRoundEnd()
	{
        Server.ExecuteCommand("sv_hegrenade_damage_multiplier 0;");
        host.DeregisterEventHandler<EventGrenadeThrown>(Util.InfiniteGrenades);
    }
}