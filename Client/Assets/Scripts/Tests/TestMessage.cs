using UnityEngine;
using System.Collections;

public class TestMessage : MonoBehaviour
{
    Transition transition;
    Game.IGameStateEndHandler handler;

    void Awake()
    {
        transition = GetComponent<Transition>();
        handler = transition.GetComponent<Game.IGameStateEndHandler>();
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            handler.HandleStateEnd(Protocol.GameState.Drawing, Protocol.GameStateEndReason.TimerExpired);
        }
    }
}
