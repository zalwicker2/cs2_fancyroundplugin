using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using System.Numerics;

class BigArmorRound : BaseRound
{
    BasePlugin host;
    public BigArmorRound(BasePlugin host)
	{
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Big Armor";
    }

    public override string GetRoundDescription()
    {
        return "You have a lot of health and armor.";
    }

    public override void PlayerCommands(CCSPlayerController player)
    {
        player.PlayerPawn.Value.Health = 100;
        Util.SetArmor(player, 1000, true, true);
        Utilities.SetStateChanged(player.PlayerPawn.Value, "CCSPlayerPawnBase", "m_ArmorValue");
        if (player.Team == CsTeam.CounterTerrorist)
        {
            player.PlayerPawn.Value.SetModel("characters\\models\\ctm_heavy\\ctm_heavy.vmdl");
        } else
        {
            player.PlayerPawn.Value.SetModel("characters\\models\\tm_phoenix_heavy\\tm_phoenix_heavy.vmdl");
        }
    }

    public override void OnRoundStart() { 
    }

	public override void OnRoundEnd()
    {
        foreach (CCSPlayerController plr in Utilities.GetPlayers())
        { 
            Util.SetArmor(plr, 100, true, false);
        }
    }
}