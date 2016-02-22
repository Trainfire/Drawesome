using Protocol;

public class GameScore
{
    public uint PreviousScore;
    public uint CurrentScore;

    public ScoreData ScoreData { get; private set; }

    public uint PointsEarned { get { return CurrentScore - PreviousScore; } }

    public GameScore(ScoreData scoreData)
    {
        ScoreData = scoreData;
    }

    public override string ToString()
    {
        return CurrentScore.ToString();
    }
}
