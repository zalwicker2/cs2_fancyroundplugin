using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

class MartydomRound : BaseRound
{
    HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var heProjectile = Utilities.CreateEntityByName<CHEGrenadeProjectile>("hegrenade_projectile");
        var deadPlayer = @event.Userid;
        var pawn = deadPlayer!.PlayerPawn.Value!;
        if (heProjectile == null || !heProjectile.IsValid) return HookResult.Continue;
        var node = pawn.CBodyComponent!.SceneNode;
        Vector pos = node!.AbsOrigin;
        pos.Z += 10;
        heProjectile.TicksAtZeroVelocity = 100;
        heProjectile.TeamNum = pawn.TeamNum;
        heProjectile.Damage = 300f;
        heProjectile.DmgRadius = 500;
        heProjectile.Teleport(pos, node!.AbsRotation, new Vector(0, 0, -10));
        heProjectile.DispatchSpawn();
        heProjectile.AcceptInput("InitializeSpawnFromWorld", pawn, pawn, "");
        heProjectile.DetonateTime = 0;
        heProjectile.Thrower.Raw = deadPlayer.PlayerPawn!.Raw;
        return HookResult.Continue;
    }

    BasePlugin host;
    public MartydomRound(BasePlugin host)
	{
        this.host = host;
    }

    public override string GetRoundName()
    {
        return "Martydom Round";
    }

    public override string GetRoundDescription()
    {
        return "Players explode when they die.";
    }

    public override void OnRoundStart() {
        host.RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }

	public override void OnRoundEnd()
    {
        host.DeregisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
    }
}