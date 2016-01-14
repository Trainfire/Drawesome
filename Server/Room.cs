using System;
using System.Collections.Generic;
using System.Timers;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Protocol;

namespace Server
{
    public class GameSettings
    {
        public const double DrawingPhaseTimer = 5000;
    }

    public class Room : WebSocketBehavior
    {
        RoomState CurrentState { get; set; }

        List<RoomState> States;
        int CurrentStateIndex = 0;

        protected override void OnOpen()
        {
            base.OnOpen();

            States = new List<RoomState>();
            States.Add(new Setup());
            States.Add(new DrawingPhase());

            CurrentState = States[0];
            CurrentState.OnEnd += ChangeState;
            CurrentState.Begin();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            CurrentState.Update(e);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
        }

        void ChangeState()
        {
            // End current state
            CurrentState.OnEnd -= ChangeState;

            // Change to new state
            Console.WriteLine("State: " + CurrentStateIndex);
            CurrentStateIndex += 1;
            CurrentState = States[CurrentStateIndex];
            CurrentState.OnEnd += ChangeState;
            CurrentState.Begin();
        }
    }

    public abstract class RoomState : WebSocketBehavior
    {
        public delegate void StateChangeHandler();
        public event StateChangeHandler OnBegin;
        public event StateChangeHandler OnEnd;

        public virtual void Begin()
        {
            if (OnBegin != null)
                OnBegin();
        }

        public virtual void End()
        {
            if (OnEnd != null)
                OnEnd();
        }

        public virtual void Update(MessageEventArgs e)
        {

        }
    }

    public class Setup : RoomState
    {
        int playersReady = 0;

        public override void Begin()
        {
            playersReady = 0;
        }

        public override void Update(MessageEventArgs e)
        {
            var message = JsonConvert.DeserializeObject<Message>(e.Data);

            Console.WriteLine(message.Type);

            if (message.Type == MessageType.PlayerReady)
            {
                var data = message.Deserialise<PlayerReadyMessage>(e.Data);
                playersReady += data.IsReady ? 1 : -1;
                Console.WriteLine("Players Ready: " + playersReady);
            }

            if (message.Type == MessageType.ForceStartRound)
            {
                End();
            }
        }

        public override void End()
        {
            Console.WriteLine("End Setup");
            base.End();
        }
    }

    public class DrawingPhase : RoomState
    {
        Timer timer;
        double DrawingTime = 1;

        public override void Begin()
        {
            timer = new Timer(DrawingTime);
            timer.Interval = 1;
            timer.Elapsed += OnTimeElapsed;
            timer.Disposed += OnTimerDisposed;
            timer.Enabled = true;
            timer.Start();
        }

        private void OnTimerDisposed(object sender, EventArgs e)
        {
            End();
        }

        private void OnTimeElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Time elapsed");
            timer.Enabled = false;
            End();
        }

        public override void End()
        {
            Console.WriteLine("End Drawing Phase");
            base.End();
        }
    }
}
