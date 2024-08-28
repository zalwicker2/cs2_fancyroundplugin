using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;

using System.Drawing;

class WallhackRound : BaseRound
{
    Dictionary<CCSPlayerController, CBaseModelEntity[]> walls = new Dictionary<CCSPlayerController, CBaseModelEntity[]>();
    void SetGlowing(CCSPlayerController plr)
    {
        if (walls.ContainsKey(plr))
        {
            StopGlowing(plr);
        }
        CCSPlayerPawn pawn = plr.PlayerPawn!.Value!;
        CBaseModelEntity? modelGlow = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");
        CBaseModelEntity? modelRelay = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");
        if (modelGlow == null || modelRelay == null)
        {
            return;
        }

        string modelName = pawn.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName;

        modelRelay.SetModel(modelName);
        modelRelay.Spawnflags = 256u;
        modelRelay.RenderMode = RenderMode_t.kRenderNone;
        modelRelay.DispatchSpawn();

        modelGlow.SetModel(modelName);
        modelGlow.Spawnflags = 256u;
        modelGlow.DispatchSpawn();

        modelGlow.Glow.GlowColorOverride = Color.Red;
        modelGlow.Glow.GlowRange = 5000;
        modelGlow.Glow.GlowTeam = plr.Team == CsTeam.CounterTerrorist ? 2 : 3;
        modelGlow.Glow.GlowType = 3;
        modelGlow.Glow.GlowRangeMin = 100;

        modelRelay.AcceptInput("FollowEntity", pawn, modelRelay, "!activator");
        modelGlow.AcceptInput("FollowEntity", modelRelay, modelGlow, "!activator");

        walls.Add(plr, [modelGlow, modelRelay]);
    }

    void StopGlowing(CCSPlayerController plr)
    {
        if(!walls.ContainsKey(plr))
        {
            return;
        }
        CBaseModelEntity[] ents = this.walls[plr];
        foreach (CBaseModelEntity ent in ents)
        {
            ent.Remove();
        }

        this.walls.Remove(plr);
    }

    HookResult OnDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        StopGlowing(@event.Userid);
        return HookResult.Continue;
    }

    BasePlugin host;
    public WallhackRound(BasePlugin host)
    {
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Wallhack";
    }

    public override string GetRoundDescription()
    {
        return "Enjoy the premier experience.";
    }

    public override void OnRoundStart()
    {
        host.RegisterEventHandler<EventPlayerDeath>(OnDeath);
    }

    public override void PlayerCommands(CCSPlayerController plr)
    {
        SetGlowing(plr);
    }

    public override void OnFreezeEnd()
    {

    }

    public override void OnRoundEnd()
    {
        host.DeregisterEventHandler<EventPlayerDeath>(OnDeath);
        foreach (KeyValuePair<CCSPlayerController, CBaseModelEntity[]> ents in this.walls)
        {
            StopGlowing(ents.Key);
        }
    }
    public override void OnTick()
    {

    }
}
