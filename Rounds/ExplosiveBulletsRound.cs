using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
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

class ExplosiveBulletsRound : BaseRound
{
    HookResult OnImpact(EventBulletImpact @event, GameEventInfo info)
    {
        var explosion = Utilities.CreateEntityByName<CEnvExplosion>("env_explosion");
        explosion.Teleport(new Vector(@event.X, @event.Y, @event.Z)) ;
        explosion.PlayerDamage = 100;

        explosion.DispatchSpawn();
        
        return HookResult.Continue;
    }
    BasePlugin host;
    public ExplosiveBulletsRound(BasePlugin host)
    {
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Explosive Bullets ";
    }

    public override string GetRoundDescription()
    {
        return "boom";
    }

    public override void OnRoundStart()
    {
    }

    public override void PlayerCommands(CCSPlayerController plr)
    {

    }

    public override void OnRoundEnd()
    {

    }
}