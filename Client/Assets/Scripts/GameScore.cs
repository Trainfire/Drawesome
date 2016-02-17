public class GameScore
{
    public uint PreviousScore;
    public uint CurrentScore;

    public uint PointsEarned { get { return CurrentScore - PreviousScore; } }

    public override string ToString()
    {
        return CurrentScore.ToString();
    }
}
