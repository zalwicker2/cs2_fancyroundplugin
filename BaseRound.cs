using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

public abstract class BaseRound
{

    public virtual string GetRoundName()
    {
        return "";
    }

    public virtual string GetRoundDescription()
    {
        return "";
    }

    public virtual void PlayerCommands(CCSPlayerController plr)
    {

    }

    public virtual void OnRoundStart()
    {

    }

    public virtual void OnFreezeEnd()
    {

    }

    public virtual void OnRoundEnd()
    {

    }

    public virtual void OnTick()
    {

    }
}