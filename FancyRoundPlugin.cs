using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

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

        int nextRound = -1;

        int testRound = -1;

        void SetNextRound()
        {
            if (testRound != -1)
            {
                nextRound = testRound;
            }
            else if (nextRound == -1)
            {
                nextRound = rng.PickRandom();
            }

            switch (nextRound)
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
                case 7:
                    currentRound = new TeleportRound(this);
                    break;
                case 8:
                    currentRound = new DeadlyFlashRound(this);
                    break;
                case 9:
                    currentRound = new SneakyRound(this);
                    break;
                case 10:
                    currentRound = new BigArmorRound(this);
                    break;
                case 11:
                    currentRound = new OverwatchRound(this);
                    break;
                case 12:
                    currentRound = new WallhackRound(this);
                    break;
                case 13:
                    currentRound = new MartydomRound(this);
                    break;
                case 14:
                    currentRound = new DontMissRound(this);
                    break;
                case 15:
                    currentRound = new ZeusRound(this);
                    break;
                case 16:
                    currentRound = new ChickenRound(this);
                    break;
            }
        }

        public override void Load(bool hotReload)
        {

            Console.WriteLine("mike is a bitch, plugin loaded 2");

            RegisterListener<Listeners.OnClientConnect>(OnPlayerConnect);

            rng = new Randomizer(16);

            AddCommand("resetround", "resets teh round", (client, commandinfo) =>
            {
                CCSGameRules gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;
                gameRules.TerminateRound(0, RoundEndReason.RoundDraw);
            });

            AddCommand("setnext", "sets next round type", (client, commandinfo) =>
            {
                Console.WriteLine(commandinfo.GetArg(1));
                nextRound = int.Parse(commandinfo.GetArg(1));
            });

            AddCommand("testround", "sets next round type", (client, commandinfo) =>
            {
                Console.WriteLine(commandinfo.GetArg(1));
                testRound = int.Parse(commandinfo.GetArg(1));
            });

            RegisterEventHandler<EventRoundPrestart>((@event, info) =>
            {
                CCSGameRules gameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules!;

                if (gameRules is null || gameRules.WarmupPeriod)
                {
                    return HookResult.Continue;
                }

                int rounds = gameRules.TotalRoundsPlayed;
                Console.WriteLine("round " + rounds + " start!");

                if (rounds % 12 == 0)
                {
                    return HookResult.Continue;
                }

                SetNextRound();

                nextRound = -1;

                return HookResult.Continue;

            });

            RegisterEventHandler<EventRoundStart>((@event, info) =>
            {
                Console.WriteLine(currentRound!.GetRoundName());

                if (currentRound != null)
                {
                    currentRound.OnRoundStart();
                }

                foreach (CCSPlayerController plr in Utilities.GetPlayers())
                {
                    plr.PrintToCenterAlert(currentRound!.GetRoundName() + " Round");
                    currentRound!.PlayerCommands(plr);
                }

                Server.PrintToChatAll($" >>> {ChatColors.Yellow}" + currentRound!.GetRoundName() + " Round");
                Server.PrintToChatAll($" > {currentRound!.GetRoundDescription()}");

                return HookResult.Continue;
            });

            RegisterEventHandler<EventPlayerSpawn>((@event, info) =>
            {
                Console.WriteLine("test 1");
                 Console.WriteLine(currentRound);
                if (currentRound != null)
                {
                    currentRound.PlayerCommands(@event.Userid!);
                    @event.Userid!.PrintToCenterAlert(currentRound!.GetRoundName() + " Round");
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

        public override void Unload(bool hotReload)
        {
            if(currentRound != null)
            {
                currentRound.OnRoundEnd();
            }
            currentRound = null;
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