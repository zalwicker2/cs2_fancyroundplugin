using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class VampireRound : BaseRound
{
    BasePlugin host;
    public VampireRound(BasePlugin host)
	{
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Vampire";
    }

    public override string GetRoundDescription()
    {
        return "Gain health when you damage your enemies.";
    }

    public override void OnRoundStart() { 
		Server.ExecuteCommand("mp_damage_vampiric_amount 1.5; ");
    }

	public override void OnRoundEnd()
	{
        Server.ExecuteCommand("mp_damage_vampiric_amount 0; ");
    }
}