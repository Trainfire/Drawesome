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
        Action OnFinish { get; set; }

        public float ElapsedTime { get; set; }
        public float CurrentTimer { get; set; }
        public float Duration { get; set; }

        public GameTimer()
        {
            Timer = new Timer();
        }

        public GameTimer(string name, float duration) : this(duration)
        {
            Name = name;
        }

        public GameTimer(string name, float duration, Action onFinish) : this(name, duration)
        {
            OnFinish = onFinish;
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
            Timer.Elapsed -= OnTimerElapsed;
        }

        void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("{0}: Time remaining: {1}", Name, Duration - ElapsedTime);

            if (Tick != null)
                Tick(this, null);

            ElapsedTime += 1f;

            if (ElapsedTime > Duration)
            {
                Timer.Enabled = false;

                if (Finish != null)
                    Finish(this, null);

                if (OnFinish != null)
                    OnFinish();
            }
        }
    }
}
