using UnityEngine.UI;
using Protocol;

public class UiGameStateResults : UiGameState
{
    public Text Answer;
    public Text Author;

    protected override void OnBegin()
    {
        base.OnBegin();
    }

    public void ShowResult(ResultData result)
    {
        
    }
}
