using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class HeadshotOnlyRound : BaseRound
{
    BasePlugin host;
    public HeadshotOnlyRound(BasePlugin host)
	{
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Headshot Only";
    }

    public override string GetRoundDescription()
    {
        return "Read above.";
    }

    public override void OnRoundStart() { 
		Server.ExecuteCommand("mp_damage_headshot_only 1");
    }

	public override void OnRoundEnd()
	{
        Server.ExecuteCommand("mp_damage_headshot_only 0");
    }
}