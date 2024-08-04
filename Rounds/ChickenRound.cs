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
    Dictionary<CCSPlayerPawn, string> models = new Dictionary<CCSPlayerPawn, string>();

    HookResult HideBodyOnDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        Util.HidePlayer(@event.Userid!.PlayerPawn!.Value!);
        var playerPawn = @event.Userid!.PlayerPawn.Value!;
        playerPawn.Render = Color.FromArgb(0, 0, 0, 255);
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");



        return HookResult.Continue;
    }
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
        if (!models.ContainsKey(plr.PlayerPawn.Value!))
        {
            models[playerPawn] = playerPawn.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName;
            Console.WriteLine(playerPawn.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName);
        }

        playerPawn.SetModel("models\\chicken\\chicken.vmdl");
        if(plr.Team == CsTeam.CounterTerrorist)
        {
            playerPawn.Render = Color.FromArgb(255, 0, 0, 255);
        } else
        {
            playerPawn.Render = Color.FromArgb(255, 255, 127, 0);
        }
        playerPawn.Health = 3;
        Utilities.SetStateChanged(plr, "CBaseEntity", "m_iHealth");
        playerPawn.CBodyComponent!.SceneNode!.Scale = 1.5f;
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
    }

    public override void OnRoundStart()
    {
        host.RegisterEventHandler<EventPlayerDeath>(HideBodyOnDeath);
    }

    public override void OnRoundEnd()
    {
        foreach (CCSPlayerController plr in Utilities.GetPlayers())
        {
            if(!plr.PawnIsAlive) continue;
            var playerPawn = plr.PlayerPawn.Value!;
            Console.WriteLine(playerPawn == null);
            if(playerPawn != null)
            {
                playerPawn.Render = Color.FromArgb(255, 255, 255, 255);
                playerPawn.CBodyComponent!.SceneNode!.Scale = 1f;
                Console.WriteLine(models[playerPawn]);
                playerPawn.SetModel(models[playerPawn]);
                Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
            }
        }
        
    }
}