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

        Func<BaseRound>[] roundTypes;

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

            currentRound = roundTypes[nextRound]();
        }

        public override void Load(bool hotReload)
        {

            roundTypes = new Func<BaseRound>[]
            {
                () => new ZoomedOutRound(this),
                () => new LowGravity(this),
                () => new BhopRound(this),
                () => new NoFallingRound(this),
                () => new VampireRound(this),
                () => new InstaKillNadesRound(this),
                () => new HeadshotOnlyRound(this),
                () => new TeleportRound(this),
                () => new DeadlyFlashRound(this),
                () => new SneakyRound(this),
                () => new BigArmorRound(this),
                () => new OverwatchRound(this),
                () => new WallhackRound(this),
                () => new MartydomRound(this),
                () => new DontMissRound(this),
                () => new ZeusRound(this),
                () => new TeleportOnKill(this)
            };

            Console.WriteLine("mike is a bitch, plugin loaded 2");

            rng = new Randomizer(17);

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

                Random r = new Random();

                if(r.Next(20) == 10)
                {
                    currentRound = new DoubleRound(this, new BaseRound[] { roundTypes[rng.PickRandom()](), roundTypes[rng.PickRandom()]() });
                } else
                {
                    SetNextRound();
                }

                nextRound = -1;

                return HookResult.Continue;

            });

            RegisterEventHandler<EventRoundStart>((@event, info) =>
            {

                if (currentRound != null)
                {
                    Console.WriteLine(currentRound!.GetRoundName());
                    currentRound.OnRoundStart();

                    foreach (CCSPlayerController plr in Utilities.GetPlayers())
                    {
                        plr.PrintToCenterAlert(currentRound!.GetRoundName() + " Round");
                        currentRound!.PlayerCommands(plr);
                    }

                    Server.PrintToChatAll($" >>> {ChatColors.Yellow}" + currentRound!.GetRoundName() + " Round");
                    Server.PrintToChatAll($" > {currentRound!.GetRoundDescription()}");
                }

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
    }

}