using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using System.Numerics;

class BigArmorRound : BaseRound
{
    BasePlugin host;
    Dictionary<CCSPlayerController, string> models = new Dictionary<CCSPlayerController, string>();
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
        return "Take reduced damage with the heavy armor suit.";
    }

    public override void PlayerCommands(CCSPlayerController player)
    {
        var playerPawn = player!.PlayerPawn.Value!;
        if (!models.ContainsKey(player))
        {
            models[player] = playerPawn.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName;
        }
        playerPawn.Health = 100;
        Util.SetArmor(player, 1000, true, true);
        Utilities.SetStateChanged(playerPawn, "CCSPlayerPawnBase", "m_ArmorValue");
        if (player.Team == CsTeam.CounterTerrorist)
        {
            playerPawn.SetModel("characters\\models\\ctm_heavy\\ctm_heavy.vmdl");
        } else
        {
            playerPawn.SetModel("characters\\models\\tm_phoenix_heavy\\tm_phoenix_heavy.vmdl");
        }
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
    }

    public override void OnRoundStart() { 
    }

	public override void OnRoundEnd()
    {
        foreach (CCSPlayerController plr in Utilities.GetPlayers())
        { 
            Util.SetArmor(plr, 100, true, false);
            if (models.ContainsKey(plr))
            {
                plr.PlayerPawn.Value!.SetModel(models[plr]);
            }
        }
    }
}