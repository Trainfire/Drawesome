using UnityEngine;
using Protocol;

public class GameOverState : State, Game.IGameState
{
    public State State { get { return this; } }
    public GameState Type { get { return GameState.GameOver; } }
    public DrawingCanvas Canvas { private get; set; }

    public GameOverState(Client client, UiGameStateGameOver view) : base(client, view)
    {

    }
}
