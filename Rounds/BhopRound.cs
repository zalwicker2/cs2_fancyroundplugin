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
        return "Hippity hoppity make the bombsite your property.";
    }

    public override void OnRoundStart()
    {
        Server.ExecuteCommand("sv_airaccelerate 10000; sv_autobunnyhopping 1; sv_enablebunnyhopping 1");
    }

	public override void OnRoundEnd()
	{
		Server.ExecuteCommand("sv_airaccelerate 800; sv_autobunnyhopping 0; sv_enablebunnyhopping 0;");
    }
}