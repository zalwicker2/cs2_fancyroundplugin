using CounterStrikeSharp.API.Core;

class DoubleRound : BaseRound
{

    BasePlugin host;
    BaseRound[] rounds;
    public DoubleRound(BasePlugin host, BaseRound[] rounds)
    {
        this.host = host;
        this.rounds = rounds;
    }

    public override string GetRoundName()
    {
        string roundNames = "";
        for (int i = 0; i < rounds.Length; i++)
        {
            roundNames += rounds[i].GetRoundName();
            if (i != rounds.Length - 1)
            {
                roundNames += " & ";
            }
        }
        return "Double Round! " + roundNames;
    }

    public override string GetRoundDescription()
    {
        string roundDescs = "";
        for (int i = 0; i < rounds.Length; i++)
        {
            roundDescs += rounds[i].GetRoundDescription();
            if (i != rounds.Length - 1)
            {
                roundDescs += "\n";
            }
        }
        return roundDescs;
    }

    public override void OnRoundStart()
    {
        foreach (BaseRound round in rounds)
        {
            round.OnRoundStart();
        }
    }

    public override void PlayerCommands(CCSPlayerController plr)
    {
        foreach (BaseRound round in rounds)
        {
            round.PlayerCommands(plr);
        }
    }

    public override void OnFreezeEnd()
    {
        foreach (BaseRound round in rounds)
        {
            round.OnFreezeEnd();
        }
    }

    public override void OnRoundEnd()
    {
        foreach (BaseRound round in rounds)
        {
            round.OnRoundEnd();
        }
    }
}