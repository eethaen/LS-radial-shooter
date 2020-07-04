using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class Timer
    {
        private readonly Events _events;

        public Timer(Events events)
        {
            _events = events;
        }

        public void Tick()
        {
            if (Time.timeSinceLevelLoad > 300f)
            {
                _events.TimeIsUp.Invoke();
            }
        }
    }
}