using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class Timer
    {
        private readonly ViewManager _view;
        private float _startTime;

        public float Value { get; private set; }

        public Timer(ViewManager view)
        {
            _view = view;
            _startTime = Time.time;
        }

        public void Tick()
        {
            Value = Time.time - _startTime;
            _view.SetTimer(Value);
        }

        public void Reset()
        {
            _startTime = Time.time;
            _view.SetTimer(0f);
        }
    }
}