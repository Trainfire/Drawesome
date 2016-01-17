using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;
using Protocol;
using Newtonsoft.Json;

namespace Server
{
    public class GameSettings
    {
        public const double DrawingPhaseTimer = 5000;
    }

    public class Room : WebSocketBehavior
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

        protected override void OnOpen()
        {
            base.OnOpen();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            CurrentState.OnRecieveMessage(e);
            ProtocolHelper.LogMessage(e);
            messageHandler.HandleMessage(e.Data);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            Console.WriteLine("Room Closed");
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
            var json = JsonConvert.SerializeObject(message);
            Send(json);
        }

        void AddPlayer(Player player)
        {
            if (!players.Contains(player))
            {
                Console.WriteLine("Add {0} to player pool.", player.Name);
                players.Add(player);
            }
            else
            {
                Console.WriteLine("Cannot add {0} to player pool as they already exist.", player.Name);
            }
        }

        void RemovePlayer(WebSocket webSocket)
        {
            Player foundPlayer = null;
            players.Find(x => x.Socket == webSocket);
            if (foundPlayer != null)
            {
                players.Remove(foundPlayer);
                Console.WriteLine("{0} was removed from the game.", foundPlayer.Name);
            }
        }

        void OnPlayerConnected(PlayerConnectMessage message)
        {
            Console.WriteLine("Player {0} connected.", message.PlayerName);
            AddPlayer(new Player(message.PlayerName));

        }

        void OnPlayerReady(PlayerReadyMessage message)
        {
            Console.WriteLine("Player {0} is ready.", message.IsReady);
        }
    }

    public abstract class RoomState : WebSocketBehavior
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

        public virtual void OnRecieveMessage(MessageEventArgs e)
        {

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

        public override void OnRecieveMessage(MessageEventArgs e)
        {
            var message = JsonConvert.DeserializeObject<Message>(e.Data);

            ProtocolHelper.LogMessage(message);

            if (message.Type == MessageType.PlayerReady)
            {
                var data = message.Deserialise<PlayerReadyMessage>(e.Data);

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

        public override void OnRecieveMessage(MessageEventArgs e)
        {
            base.OnRecieveMessage(e);

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
