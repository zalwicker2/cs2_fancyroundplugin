using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

class ZeusRound : BaseRound
{
    BasePlugin host;
    public ZeusRound(BasePlugin host)
    {
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Zeus";
    }

    public override string GetRoundDescription()
    {
        return "Only tasers with a 1s recharge.";
    }

    public override void OnRoundStart()
    {
        Server.ExecuteCommand("mp_taser_recharge_time 1");
    }

    public override void PlayerCommands(CCSPlayerController plr)
    {
        Util.SetInventory(plr, ["weapon_taser"], true);
        plr!.InGameMoneyServices!.Account = 0;
    }

    public override void OnRoundEnd()
    {
        Server.ExecuteCommand("mp_taser_recharge_time 30");
    }
}