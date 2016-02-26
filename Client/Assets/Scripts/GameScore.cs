using Protocol;

public class GameScore
{
    public ScoreData PreviousScoreData { get; private set; }
    public ScoreData CurrentScoreData { get; private set; }
    public uint PointsEarned { get { return CurrentScoreData.Score - PreviousScoreData.Score; } }

    public GameScore(ScoreData scoreData)
    {
        CurrentScoreData = scoreData;
    }

    public void UpdateScore(ScoreData newScore)
    {
        // Cache the previous score
        PreviousScoreData = CurrentScoreData;

        // Update the current score
        CurrentScoreData = newScore;
    }

    public override string ToString()
    {
        return CurrentScoreData.Score.ToString();
    }
}
