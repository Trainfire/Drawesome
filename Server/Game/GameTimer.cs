using System;
using System.Timers;

namespace Server.Game
{
    public class GameTimer
    {
        public event EventHandler Tick;
        public event EventHandler Finish;

        Timer Timer { get; set; }
        string Name { get; set; }

        float ElapsedTime = 0f;
        float Duration = 0f;

        public GameTimer(string name, float duration) : this(duration)
        {
            Name = name;
        }

        public GameTimer(float duration)
        {
            if (Name == null)
                Name = "Unnamed Timer";

            Duration = duration;

            Timer = new Timer();
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
            Console.WriteLine("{0}: Tick...", Name);

            ElapsedTime += 1f;

            if (Tick != null)
                Tick(this, null);

            if (ElapsedTime > Duration && Finish != null)
            {
                Timer.Enabled = false;
                Finish(this, null);
            }
        }
    }
}
