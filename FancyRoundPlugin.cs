using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

using System;
using System.IO;

namespace FancyRoundPlugin
{
    public class FancyRoundPlugin : BasePlugin
    {
        public override string ModuleName => "Mike Sucks";

        public override string ModuleVersion => "0.0.1";

        private BaseRound? currentRound = null;

        private void SendMessageToAllPlayers(string msg)
        {
            foreach (CCSPlayerController plr in Utilities.GetPlayers())
            {

                plr.PrintToChat(msg);
                plr.PrintToCenter(msg);
            }
        }

        int roundTick = 0;

        Randomizer rng;

        public override void Load(bool hotReload)
        {

            Console.WriteLine("mike is a bitch, plugin loaded");

            RegisterListener<Listeners.OnClientConnect>(OnPlayerConnect);

            string eventName = "";

            rng = new Randomizer(15);

            AddCommand("resetround", "resets teh round", (client, commandinfo) =>
            {
                CCSGameRules gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;
                gameRules.TerminateRound(0, RoundEndReason.RoundDraw);
            });

            RegisterEventHandler<EventRoundStart>((@event, info) =>
            {
                CCSGameRules gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;

                if (gameRules is null || gameRules.WarmupPeriod)
                {
                    return HookResult.Continue;
                }
                Random rnd = new Random();

                int rounds = gameRules.TotalRoundsPlayed;
                Console.WriteLine("round " + rounds + " start!");
                //if((rounds + 1) % 3 == 0)
                //{
                int num = rng.PickRandom();

                int money = rnd.Next(2, 10) * 1000;
                foreach (CCSPlayerController plr in Utilities.GetPlayers())
                {
                    plr!.InGameMoneyServices!.Account = money;
                }

                switch (num)
                {
                    case 0:
                        currentRound = new ZoomedOutRound(this);
                        break;
                    case 1:
                        currentRound = new LowGravity(this);
                        break;
                    case 2:
                        currentRound = new BhopRound(this);
                        break;
                    case 3:
                        currentRound = new NoFallingRound(this);
                        break;
                    case 4:
                        currentRound = new VampireRound(this);
                        break;
                    case 5:
                        currentRound = new InstaKillNadesRound(this);
                        break;
                    case 6:
                        currentRound = new HeadshotOnlyRound(this);
                        break;
                    case 8:
                        currentRound = new TeleportRound(this);
                        break;
                    case 9:
                        currentRound = new ZeusRound(this);
                        break;
                    case 10:
                        currentRound = new DeadlyFlashRound(this);
                        break;
                    case 11:
                        currentRound = new SneakyRound(this);
                        break;
                    case 12:
                        currentRound = new BigArmorRound(this);
                        break;
                    case 13:
                        currentRound = new OverwatchRound(this);
                        break;
                    case 14:
                        currentRound = new WallhackRound(this);
                        break;
                    case 15:
                        currentRound = new ChickenRound(this);
                        break;
                    case 16:
                        currentRound = new AgarioRound(this);
                        break;
                    case 17:
                        currentRound = new ExplosiveBulletsRound(this);
                        break;
                }

                if (currentRound != null)
                {
                    currentRound.OnRoundStart();
                }

                roundTick = 0;
                foreach (CCSPlayerController plr in Utilities.GetPlayers())
                {
                    plr.PrintToCenterAlert(currentRound!.GetRoundName() + " Round");
                    currentRound!.PlayerCommands(plr);
                }

                Server.PrintToChatAll($" >>> {ChatColors.Yellow}" + currentRound!.GetRoundName() + " Round");
                Server.PrintToChatAll($" > {currentRound.GetRoundDescription()}");
                //}

                return HookResult.Continue;
            });

            RegisterEventHandler<EventPlayerSpawn>((@event, info) =>
            {
                if (currentRound != null)
                {
                    currentRound.PlayerCommands(@event.Userid!);
                }
                return HookResult.Continue;
            });

            RegisterEventHandler<EventRoundFreezeEnd>((@event, info) =>
            {
                if (currentRound != null)
                {
                    currentRound.OnFreezeEnd();
                    foreach (CCSPlayerController plr in Utilities.GetPlayers())
                    {
                        plr.PrintToCenterAlert(currentRound.GetRoundName() + " Round");
                    }
                }

                return HookResult.Continue;
            });

            RegisterEventHandler<EventRoundEnd>((@event, info) =>
            {
                if (currentRound != null)
                {
                    currentRound.OnRoundEnd();
                }
                currentRound = null;
                return HookResult.Continue;
            });

            RegisterListener<Listeners.OnTick>(() =>
            {
                if(currentRound != null)
                {
                    currentRound.OnTick();
                }
            });
        }

        private void OnPlayerConnect(int slot, string name, string ip)
        {
            var plr = new CCSPlayerController(NativeAPI.GetEntityFromIndex(slot + 1));
            if (plr != null && !plr.IsBot)
            {
                Console.WriteLine("STEAMID: " + plr.SteamID);
                Server.PrintToChatAll($"some loser named {name} joined.");
            }
        }
    }

}