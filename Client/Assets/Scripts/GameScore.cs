using Protocol;

public class GameScore
{
    public uint PreviousScore { get; private set; }
    public ScoreData CurrentScoreData { get; private set; }
    public uint PointsEarned { get { return CurrentScoreData.Score - PreviousScore; } }

    public GameScore(ScoreData scoreData)
    {
        
    }

    public void UpdateScore(ScoreData newScore)
    {
        // Cache the previous score
        PreviousScore = CurrentScoreData != null ? CurrentScoreData.Score : 0;

        // Update the current score
        CurrentScoreData = newScore;
    }

    public override string ToString()
    {
        return CurrentScoreData.Score.ToString();
    }
}
