using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

using System.Drawing;

class OverwatchRound : BaseRound
{
    Dictionary<CCSPlayerController, string> roles = new Dictionary<CCSPlayerController, string>();

    void SetTank(CCSPlayerController plr)
    {
        var playerPawn = plr.PlayerPawn!.Value!;
        playerPawn.Health = 750;
        Utilities.SetStateChanged(plr, "CBaseEntity", "m_iHealth");
        Util.SetArmor(plr.PlayerPawn.Value!, 750, true);
        Utilities.SetStateChanged(playerPawn, "CCSPlayerPawnBase", "m_ArmorValue");
        playerPawn.VelocityModifier = -1;
        if (plr.Team == CsTeam.CounterTerrorist)
        {
            playerPawn.SetModel("characters\\models\\ctm_heavy\\ctm_heavy.vmdl");
        }
        else
        {
            playerPawn.SetModel("characters\\models\\tm_phoenix_heavy\\tm_phoenix_heavy.vmdl");
        }
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
    }

    void SetDamage(CCSPlayerController plr)
    {
        var playerPawn = plr.PlayerPawn!.Value!;
        playerPawn.Health = 250;
        Utilities.SetStateChanged(plr, "CBaseEntity", "m_iHealth");
        Util.SetArmor(plr.PlayerPawn.Value!, 250, true);
        playerPawn.Render = Color.FromArgb(255, 180, 180, 255);
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
    }

    void SetHealer(CCSPlayerController plr)
    {
        var playerPawn = plr.PlayerPawn!.Value!;
        playerPawn.Health = 250;
        Utilities.SetStateChanged(plr, "CBaseEntity", "m_iHealth");
        Util.SetArmor(plr.PlayerPawn.Value!, 250, true);
        playerPawn.Render = Color.FromArgb(255, 180, 255, 180);
        Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
    }

    bool AddHealth(CCSPlayerController plr, float health)
    {
        bool playSound = true;
        int maxHealth = 250;
        if (roles[plr] == "tank")
        {
            maxHealth = 750;
        }

        CCSPlayerPawn pawn = plr.PlayerPawn!.Value!;

        if(pawn.Health == maxHealth)
        {
            playSound = false;
        }

        if(pawn.Health + health > maxHealth)
        {
            pawn.Health = maxHealth;
        } else
        {
            pawn.Health += (int) health;
        }
        return playSound;
    }

    HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        CCSPlayerController attacker = @event.Attacker;
        CCSPlayerController victim = @event.Userid;

        Console.WriteLine(attacker);
        if(attacker == null)
        {
            return HookResult.Continue;
        }

        if (roles[attacker] == "healer")
        {
            if(attacker.Team == victim.Team)
            {
                attacker.ExecuteClientCommand("snd_toolvolume .2; play sounds/ui/armsrace_become_leader_match.vsnd_c"); 
                victim.ExecuteClientCommand("snd_toolvolume .2; play sounds/ui/armsrace_become_leader_match.vsnd_c");
                AddHealth(victim, @event.DmgHealth + 5);
            } else
            {
                Console.WriteLine("half_damage");
                AddHealth(victim, (float)@event.DmgHealth / 2);
            }
            return HookResult.Continue;
        }

        return HookResult.Continue;

    }

    private void SetRole(CCSPlayerController plr, string role)
    {
        if(role == "tank")
        {
            SetTank(plr);
            plr.PrintToChat($" > You are {ChatColors.Red}TANK");
            plr.PrintToChat($" > You have a shitload of HP and armor. Don't die.");
        } else if(role == "damage")
        {
            SetDamage(plr);
            plr.PrintToChat($" > You are {ChatColors.Blue}DAMAGE");
            plr.PrintToChat($" > You're basically a regular player.");
        } else if(role == "healer")
        {
            SetHealer(plr);
            plr.PrintToChat($" > You are {ChatColors.Green}HEALER");
            plr.PrintToChat($" > Shoot your teammates to heal them. You deal 50% damage to enemies.");
        }

        roles.Add(plr, role);
    }

    BasePlugin host;
    public OverwatchRound(BasePlugin host)
	{
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Overwatch";
    }

    public override string GetRoundDescription()
    {
        return "Become a Healer, DPS, or Tank.";
    }

    public override void PlayerCommands(CCSPlayerController plr)
    {
        
    }

    public override void OnFreezeEnd()
    {
        string[] roleOrder = ["damage", "healer", "tank", "damage", "healer"];
        List<CCSPlayerController> ts = Util.GetTeamPlayers(CsTeam.Terrorist);
        Util.ShuffleList(ts);
        Console.WriteLine(ts.Count);
        for(int i = 0; i < ts.Count; i++)
        {
            SetRole(ts[i], roleOrder[i % roleOrder.Length]);
        }

        List<CCSPlayerController> cts = Util.GetTeamPlayers(CsTeam.CounterTerrorist);
        Util.ShuffleList(cts);
        for (int i = 0; i < cts.Count; i++)
        {
            SetRole(cts[i], roleOrder[i % roleOrder.Length]);
        }

        foreach(KeyValuePair<CCSPlayerController, string> entry in roles)
        {
            Console.WriteLine(entry.Key.Slot + " " + entry.Key.Team + " " + entry.Value);
        }
    }

    public override void OnRoundStart() {
        host.RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
    }

	public override void OnRoundEnd()
    {
        host.DeregisterEventHandler<EventPlayerHurt>(OnPlayerHurt);
        foreach (CCSPlayerController plr in Utilities.GetPlayers())
        {
            var playerPawn = plr.PlayerPawn!.Value!;
            playerPawn.Render = Color.FromArgb(255, 255, 255, 255);
            Utilities.SetStateChanged(playerPawn, "CBaseEntity", "m_CBodyComponent");
        }
        roles.Clear();
    }
}