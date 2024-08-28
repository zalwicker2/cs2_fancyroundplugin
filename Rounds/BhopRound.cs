using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class BhopRound : BaseRound
{
	BasePlugin host;
	public BhopRound(BasePlugin host)
	{
		this.host = host;
	}

    public override string GetRoundName()
    {
        return "BHOP";
    }

    public override string GetRoundDescription()
    {
        return "Hold space to automatically bhop.";
    }

    public override void OnRoundStart()
    {
        Server.ExecuteCommand("sv_airaccelerate 10000; sv_autobunnyhopping 1; sv_enablebunnyhopping 1; weapon_air_spread_scale 0; weapon_accuracy_nospread 1");
    }

	public override void OnRoundEnd()
	{
		Server.ExecuteCommand("sv_airaccelerate 800; sv_autobunnyhopping 0; sv_enablebunnyhopping 0; weapon_air_spread_scale 1; weapon_accuracy_nospread 0");
    }
}