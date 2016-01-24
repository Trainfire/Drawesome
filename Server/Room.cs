using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using Fleck;
using Protocol;
using Newtonsoft.Json;

namespace Server
{
    public class GameSettings
    {
        public const double DrawingPhaseTimer = 5000;
    }

    public class Room : WebSocketBehaviour
    {
        RoomState CurrentState { get; set; }

        List<Player> players = new List<Player>();

        MessageHandler messageHandler = new MessageHandler();
        List<RoomState> States;
        int CurrentStateIndex = 0;

        public Room()
        {
            States = new List<RoomState>();
            States.Add(new Setup());
            States.Add(new DrawingPhase());

            CurrentState = States[0];
            CurrentState.OnEnd += ChangeState;
            CurrentState.BeginState();

            messageHandler.OnPlayerConnected += OnPlayerConnected;
            messageHandler.OnPlayerReady += OnPlayerReady;
        }

        public override void OnOpen(IWebSocketConnection socket)
        {
            base.OnOpen(socket);
            Console.WriteLine("Player connected");
        }

        public override void OnMessage(string message)
        {
            base.OnMessage(message);

            CurrentState.OnMessage(message);
            ProtocolHelper.LogMessage(message);
            messageHandler.HandleMessage(message);
        }

        public override void OnClose(IWebSocketConnection socket)
        {
            base.OnClose(socket);
            Console.WriteLine("Player {0} disconnected.", "PlayerName");
        }

        void ChangeState()
        {
            // End current state
            CurrentState.OnEnd -= ChangeState;

            // Change to the next state.
            Console.WriteLine("State: " + CurrentStateIndex);

            if (CurrentStateIndex == States.Count - 1)
            {
                // TODO: Game loop ends here.
                Console.WriteLine("Game Over!");
            }
            else
            {
                CurrentStateIndex += 1;
                CurrentState = States[CurrentStateIndex];
                CurrentState.OnEnd += ChangeState;
                CurrentState.BeginState();
            }
        }

        void SendMessage(Message message)
        {

        }

        void AddPlayer(Player player)
        {
            if (!players.Contains(player))
            {
                Console.WriteLine("Add {0} to player pool with GUID {1}.", player.Name, player.ID);
                players.Add(player);
            }
            else
            {
                Console.WriteLine("Cannot add {0} to player pool as they already exist.", player.Name);
            }
        }

        void OnPlayerConnected(PlayerFirstConnectMessage message)
        {
            Console.WriteLine("Player {0} connected.", message.PlayerName);
            //AddPlayer(new Player(message.PlayerName));

        }

        void OnPlayerReady(PlayerReadyMessage message)
        {
            Console.WriteLine("Player {0} is ready.", message.IsReady);
        }
    }

    public abstract class RoomState : WebSocketBehaviour
    {
        public delegate void StateChangeHandler();
        public event StateChangeHandler OnBegin;
        public event StateChangeHandler OnEnd;

        public virtual void BeginState()
        {
            if (OnBegin != null)
                OnBegin();
        }

        public virtual void EndState()
        {
            if (OnEnd != null)
                OnEnd();
        }
    }

    #region States

    public class Setup : RoomState
    {
        int playersReady = 0;

        public override void BeginState()
        {
            Console.WriteLine("Waiting for all players to join / be ready");
            playersReady = 0;
        }

        public override void OnMessage(string m)
        {
            var message = JsonConvert.DeserializeObject<Message>(m);

            ProtocolHelper.LogMessage(message);

            if (message.Type == MessageType.PlayerReady)
            {
                var data = message.Deserialise<PlayerReadyMessage>(m);

                // If all player's are ready, continue to next state.
                playersReady += data.IsReady ? 1 : -1;

                EndState();
            }

            if (message.Type == MessageType.ForceStartRound)
            {
                EndState();
            }
        }

        public override void EndState()
        {
            Console.WriteLine("End Setup");
            base.EndState();
        }
    }

    public class DrawingPhase : RoomState
    {
        Timer timer;
        float timeElapsed = 0f;
        const float DrawingTime = 10; // in seconds

        public override void BeginState()
        {
            Console.WriteLine("Begin Drawing Phase");
            timer = new Timer(1000);
            timer.Elapsed += OnTimeElapsed;
            timer.Enabled = true;
        }

        private void OnTimeElapsed(object sender, ElapsedEventArgs e)
        {
            timeElapsed += 1f;

            if (timeElapsed > DrawingTime)
            {
                timer.Enabled = false;
                timer.Dispose();
                EndState();
            }
            else
            {
                // Send update to client?

            }

            Console.WriteLine("Time Remaining: " + (DrawingTime - timeElapsed));
        }

        public override void OnMessage(string m)
        {
            // here we would store each player's drawing
        }

        public override void EndState()
        {
            Console.WriteLine("End Drawing Phase");
            base.EndState();
        }
    }

    #endregion
}
