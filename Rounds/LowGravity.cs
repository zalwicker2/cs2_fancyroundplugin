using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class LowGravity : BaseRound
{
    BasePlugin host;
    public LowGravity(BasePlugin host)
	{
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Low Gravity";
    }

    public override string GetRoundDescription()
    {
        return "If you can't figure out what happens based on the name, quit.";
    }

    public override void OnRoundStart() { 
		Server.ExecuteCommand("sv_gravity 200; weapon_air_spread_scale 0; weapon_accuracy_nospread 1");
    }

	public override void OnRoundEnd()
	{
        Server.ExecuteCommand("sv_gravity 800; weapon_air_spread_scale 1; weapon_accuracy_nospread 0");
    }
}