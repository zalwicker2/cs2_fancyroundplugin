using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

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
        player.PlayerPawn.Value.Health = 500;
        player.PlayerPawn.Value.ArmorValue = 500;
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
		Server.ExecuteCommand("mp_damage_vampiric_amount 1; ");
    }

	public override void OnRoundEnd()
	{
        Server.ExecuteCommand("mp_damage_vampiric_amount 0; ");
    }
}