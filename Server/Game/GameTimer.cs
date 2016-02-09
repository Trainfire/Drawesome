using System;
using System.Timers;

namespace Server.Game
{
    public class GameTimer
    {
        public event EventHandler Tick;
        public event EventHandler Finish;

        Timer Timer { get; set; }

        float ElapsedTime = 0f;
        float Duration = 0f;

        public GameTimer(float duration)
        {
            Duration = duration;

            Timer.Elapsed += OnTimerElapsed;
            Timer.Interval = 1000;
            Timer.Start();
        }

        public void Start()
        {
            Timer.Start();
        }

        public void Stop()
        {
            Timer.Stop();
        }

        void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ElapsedTime += 1f;

            if (Tick != null)
                Tick(this, null);

            if (ElapsedTime > Duration && Finish != null)
                Finish(this, null);
        }
    }
}
