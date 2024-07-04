using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

using System.Drawing;

class ChickenRound : BaseRound
{
    BasePlugin host;
    public ChickenRound(BasePlugin host)
    {
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Chicken";
    }

    public override string GetRoundDescription()
    {
        return "You're a mothafuckin chicken. Cluck cluck bitch.";
    }

    public override void PlayerCommands(CCSPlayerController plr)
    {
        var playerPawn = plr.PlayerPawn.Value!;
        playerPawn.SetModel("models\\chicken\\chicken.vmdl");
        if(plr.Team == CsTeam.CounterTerrorist)
        {
            playerPawn.Render = Color.FromArgb(255, 0, 0, 255);
        } else
        {
            playerPawn.Render = Color.FromArgb(255, 255, 127, 0);
        }
        playerPawn.CBodyComponent!.SceneNode!.Scale = 1.5f;
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
        Console.WriteLine("wtf");
    }

    public override void OnRoundStart()
    {
    }

    public override void OnRoundEnd()
    {
        foreach (CCSPlayerController plr in Utilities.GetPlayers())
        {
            var playerPawn = plr.PlayerPawn.Value!;
            playerPawn.Render = Color.FromArgb(255, 255, 255, 255);
            playerPawn.CBodyComponent!.SceneNode!.Scale = 1f;
            Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
        }
        
    }
}