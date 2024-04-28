using System;
using System.Collections.Generic;
using System.Linq;

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

namespace FancyRoundPlugin
{
    public class FancyRoundPlugin : BasePlugin
    {
        public override string ModuleName => "Mike Sucks";

        public override string ModuleVersion => "0.0.1";

        private void SendMessageToAllPlayers(string msg)
        {
            foreach(CCSPlayerController plr in Utilities.GetPlayers())
            {
                
                plr.PrintToChat(msg);
                plr.PrintToCenter(msg);
            }
        }

        private CCSPlayerController[] GetTeamPlayers(CsTeam team)
        {
            List<CCSPlayerController> plrs = new List<CCSPlayerController>();
            foreach (CCSPlayerController plr in Utilities.GetPlayers())
            {

                if(plr.Team == team)
                {
                    plrs.Add(plr);
                }
            }

            return plrs.ToArray();
        }

        private void GiveOutBomb()
        {
            Random rnd = new Random();
            CCSPlayerController[] ts = GetTeamPlayers(CsTeam.Terrorist);

            int bomber = rnd.Next(ts.Length);
            ts[bomber].GiveNamedItem("weapon_c4");
        }

        private void RemovePlayerWeapons(CCSPlayerController plr)
        {
            Console.WriteLine("has " + plr.PlayerPawn.Value.WeaponServices.ActiveWeapon.Value.Globalname);
        }

        private HookResult InfiniteGrenades(EventGrenadeThrown @event, GameEventInfo info)
        {
            Console.WriteLine("thrown " + @event.Weapon);
            @event.Userid.GiveNamedItem("weapon_" + @event.Weapon);
            return HookResult.Continue;
        }

        private HookResult DamageOnBlind(EventPlayerBlind @event, GameEventInfo info)
        {
                int damage = (int)Math.Round(Math.Exp(.923f * @event.BlindDuration)) - 1; //(int)Math.Round(20f * @event.BlindDuration);
                Console.WriteLine(damage);
                @event.Userid.PlayerPawn.Value.Health = @event.Userid.PlayerPawn.Value.Health - damage;
                Utilities.SetStateChanged(@event.Userid.PlayerPawn.Value, "CBaseEntity", "m_iHealth");
                if (@event.Userid.PlayerPawn.Value.Health <= 0)
                {
                    @event.Userid.CommitSuicide(true, true);
                }
                //@event.Userid.PawnHealth = @event.Userid.PawnHealth - (uint) Math.Round(20 * @event.BlindDuration);
                Console.WriteLine(@event.BlindDuration);

                return HookResult.Continue;
        }

        private HookResult TeleportOnPing(EventPlayerPing @event, GameEventInfo info)
        {
            Console.WriteLine("" + @event.X + " " + @event.Y + " " + @event.Z);
            Console.WriteLine(@event.Userid.PlayerPawn.Value.AbsOrigin);
            Vector pingLocation = new Vector(@event.X, @event.Y, @event.Z);
            Vector diff = @event.Userid.PlayerPawn.Value.AbsOrigin - pingLocation;
            int length = 50;
            Vector unit = new Vector(length * diff.X / diff.Length(), length * diff.Y / diff.Length(), length * diff.Z / diff.Length());
            if (@event.Z - @event.Userid.PlayerPawn.Value.AbsOrigin.Z < 300)
            {
                @event.Userid.PlayerPawn.Value.Teleport(new Vector(@event.X, @event.Y, @event.Z + 16f) + unit);
            }

            return HookResult.Continue;
        }

        public override void Load(bool hotReload)
        {
            Console.WriteLine("mike is a bitch");

            RegisterListener<Listeners.OnClientConnect>(OnPlayerConnect);

            AddCommand("fuck", "fuck", OnFuckCommand);

            string eventName = "";

            GameEventHandler<EventPlayerPing> hook = null;

            RegisterEventHandler<EventRoundStart>((@event, info) =>
            {
                CCSGameRules gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;

                int rounds = gameRules.TotalRoundsPlayed;
                Console.WriteLine("round " + rounds + " start!");
                Random rnd = new Random();
                int num = rnd.Next(13);

                eventName = "";

                Server.ExecuteCommand("sv_airaccelerate 3.6; sv_autobunnyhopping 0; sv_enablebunnyhopping 0; sv_falldamage_scale 1; mp_damage_vampiric_amount 0; sv_hegrenade_damage_multiplier 1; mp_damage_headshot_only 0;mp_damage_scale_ct_body 1; mp_damage_scale_ct_head 1; mp_damage_scale_t_body 1; mp_damage_scale_t_head 1; mp_taser_recharge_time 30; sv_gravity 800; weapon_air_spread_scale 1; weapon_accuracy_nospread 0; ammo_grenade_limit_flashbang 2; ammo_grenade_limit_total 4; ");

                foreach (CCSPlayerController plr in Utilities.GetPlayers())
                {
                    int money = rnd.Next(2000, 10000);
                    plr.InGameMoneyServices.Account = money;
                }
                switch (num)
                {
                    default:
                        SendMessageToAllPlayers("NORMAL ROUND");
                        break;
                    case 1:
                        SendMessageToAllPlayers("LOW GRAV round");
                        Server.ExecuteCommand("sv_gravity 200; weapon_air_spread_scale 0; weapon_accuracy_nospread 1");
                        break;
                    case 2:
                        SendMessageToAllPlayers("BHOP round");
                        Server.ExecuteCommand("sv_airaccelerate 1000; sv_autobunnyhopping 1; sv_enablebunnyhopping 1");
                        break;
                    case 3:
                        SendMessageToAllPlayers("NO FALLING ROUND");
                        Server.ExecuteCommand("sv_falldamage_scale 100");
                        break;
                    case 4:
                        SendMessageToAllPlayers("VAMPIRE ROUND");
                        Server.ExecuteCommand("mp_damage_vampiric_amount .25");
                        break;
                    case 5:
                        SendMessageToAllPlayers("INSTA KILL NADES");
                        Server.ExecuteCommand("sv_hegrenade_damage_multiplier 100;");
                        RegisterEventHandler<EventGrenadeThrown>(InfiniteGrenades);
                        eventName = "grenade";
                        foreach (CCSPlayerController plr in Utilities.GetPlayers())
                        {
                            RemovePlayerWeapons(plr);
                            plr.RemoveWeapons();
                            plr.GiveNamedItem("weapon_knife");
                            plr.GiveNamedItem("weapon_hegrenade");
                            plr.InGameMoneyServices.Account = 0;
                        }
                        GiveOutBomb();
                        break;
                    case 6:
                        SendMessageToAllPlayers("HEADSHOT ONLY");
                        Server.ExecuteCommand("mp_damage_headshot_only 1");
                        break;
                    case 8:
                        SendMessageToAllPlayers("TELEPORT ROUND");
                        eventName = "teleport";
                        break;
                    case 9:
                        SendMessageToAllPlayers("ZEUS ROUND");
                        eventName = "zeus";
                        foreach (CCSPlayerController plr in Utilities.GetPlayers())
                        {
                            plr.RemoveWeapons();
                            plr.GiveNamedItem("weapon_taser");
                            plr.InGameMoneyServices.Account = 0;
                        }
                        GiveOutBomb();
                        Server.ExecuteCommand("mp_taser_recharge_time 1");
                        break;
                    case 10:
                        SendMessageToAllPlayers("DEADLY FLASH");

                        RegisterEventHandler<EventGrenadeThrown>(InfiniteGrenades);
                        RegisterEventHandler<EventPlayerBlind>(DamageOnBlind);
                        foreach (CCSPlayerController plr in Utilities.GetPlayers()) {
                            RemovePlayerWeapons(plr);
                            plr.RemoveWeapons();
                            plr.GiveNamedItem("weapon_knife");
                            plr.GiveNamedItem("weapon_flashbang");
                            plr.InGameMoneyServices.Account = 0;
                        }
                        GiveOutBomb();

                        break;
                    /*case 11:
                        Server.ExecuteCommand("mp_respawn_on_death_ct 1");
                        Server.ExecuteCommand("mp_respawn_on_death_t 1");
                        SendMessageToAllPlayers("JUGGERNAUTS");

                        CCSPlayerController[] players = Utilities.GetPlayers().ToArray();
                        int PLAYERS_PER_JUGGERNAUT = 4;
                        List<int> juggernautIds = new List<int>();
                        for(int i = rnd.Next(0, PLAYERS_PER_JUGGERNAUT + 1); i < players.Length; i+=4)
                        {
                            juggernautIds.Add(i);
                        }

                        if(juggernautIds.Count == 0)
                        {
                            juggernautIds.Add(rnd.Next(0, players.Length));
                        }

                        Console.WriteLine(juggernautIds.ToString());

                        for (int i = 0; i < players.Length; i++)
                        {
                            if (juggernautIds.Contains(i))
                            {
                                players[i].ChangeTeam(CsTeam.Terrorist);
                                players[i].PlayerPawn.Value.Health = 500;
                                Utilities.SetStateChanged(players[i].PlayerPawn.Value, "CBaseEntity", "m_iHealth");
                            }
                            else
                            {
                                players[i].ChangeTeam(CsTeam.CounterTerrorist);
                            }
                        }

                        Server.ExecuteCommand("mp_respawn_on_death_ct 0");
                        Server.ExecuteCommand("mp_respawn_on_death_t 0");
                        break;*/

                }
                return HookResult.Continue;
            });
            RegisterEventHandler<EventPlayerSpawn>((@event, info) =>
            {
                CCSPlayerController plr = @event.Userid;
                switch (eventName)
                {
                    case "zeus":
                        plr.RemoveWeapons();
                        plr.GiveNamedItem("weapon_taser");
                        plr.InGameMoneyServices.Account = 0;
                        break;
                    case "grenade":
                        plr.RemoveWeapons();
                        plr.GiveNamedItem("weapon_hegrenade");
                        plr.InGameMoneyServices.Account = 0;
                        break;
                }
                return HookResult.Continue;
            });

            //Console.WriteLine(@event.Userid.PlayerPawn.Value.WeaponServices.ActiveWeapon.Value.Globalname);

            RegisterEventHandler<EventRoundFreezeEnd>((@event, info) =>
            {
                if(eventName == "teleport")
                {
                    RegisterEventHandler<EventPlayerPing>(TeleportOnPing);
                } else if (eventName == "glow")
                {
                    foreach (CCSPlayerController plr in Utilities.GetPlayers())
                    {
                        Schema.SetSchemaValue(plr.PlayerPawn.Value.Handle, "CBasePlayerPawn", "m_flDetectedByEnemySensorTime", 86400);
                    }
                }
                return HookResult.Continue;
            });

            RegisterEventHandler<EventRoundEnd>((@event, info) =>
            {
                DeregisterEventHandler<EventPlayerPing>(TeleportOnPing);
                DeregisterEventHandler<EventPlayerBlind>(DamageOnBlind);
                DeregisterEventHandler<EventGrenadeThrown>(InfiniteGrenades);
                return HookResult.Continue;
            });

            RegisterListener<Listeners.OnEntitySpawned>(entity =>
            {
                Console.WriteLine(entity.DesignerName);
                if (entity.DesignerName != "smokegrenade_projectile") return;

                var projectile = new CSmokeGrenadeProjectile(entity.Handle);

                // Changes smoke grenade colour to a random colour each time.
                Server.NextFrame(() =>
                {
                    projectile.SmokeColor.X = 0f;
                    projectile.SmokeColor.Y = 0f;
                    projectile.SmokeColor.Z = 0f;
                });
            });
        }

        private void OnPlayerConnect(int slot, string name, string ip)
        {
            var plr = new CCSPlayerController(NativeAPI.GetEntityFromIndex(slot + 1));
            Console.WriteLine(name + " sucks");
        }

        private void OnFuckCommand(CCSPlayerController? client, CommandInfo commandinfo)
        {
            ShowMenu(client);

            Server.NextFrame(() =>
            {
                client.PrintToCenterHtml("<h2>CS2 SUCKS</h2>" + "<p>MIKE IS GAY</p>", 5000);
            });

            //client.PlayerPawn.Value.Teleport(new Vector(0f, 0f, 0f));
        }

        public void ShowMenu(CCSPlayerController client)
        {
            var menu = new ChatMenu($"What mode do you want to play?");
            menu.AddMenuOption("Low Gravity", (player, option) =>
            {

            }, false);
            menu.AddMenuOption("Fast Speed", (player, option) =>
            {

            }, false);
            menu.AddMenuOption("No Recoil", (player, option) =>
            {

            }, false);
            menu.AddMenuOption("1HP", (player, option) =>
            {

            }, false);
            menu.AddMenuOption("Negev Only", (player, option) =>
            {

            }, false);

            MenuManager.OpenChatMenu(client, menu);
        }
    }

}