using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class NoFallingRound : BaseRound
{
    BasePlugin host;
    public NoFallingRound(BasePlugin host)
	{
		this.host = host;
	}

    public override string GetRoundName()
    {
        return "No Falling";
    }

    public override string GetRoundDescription()
    {
        return "Die immediately if you take any fall damage.";
    }

    public override void OnRoundStart()
    {
        Server.ExecuteCommand("sv_falldamage_scale 100;");
    }

	public override void OnRoundEnd()
	{
		Server.ExecuteCommand("sv_falldamage_scale 1;");
    }
}