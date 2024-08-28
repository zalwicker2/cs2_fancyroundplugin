using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class DontMissRound : BaseRound
{
    BasePlugin host;
    public DontMissRound(BasePlugin host)
    {
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Don't miss";
    }

    public override string GetRoundDescription()
    {
        return "Take damage if you miss a shot.";
    }

    public override void OnRoundStart()
    {
        Server.ExecuteCommand("mp_weapon_self_inflict_amount .33");
    }

    public override void OnRoundEnd()
    {
        Server.ExecuteCommand("mp_weapon_self_inflict_amount 0");
    }
}