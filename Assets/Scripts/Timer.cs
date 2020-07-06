using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class Timer
    {
        private readonly ViewController _viewController;
        private float _startTime;

        public float Value { get; private set; }

        public Timer(ViewController viewController)
        {
            _viewController = viewController;
            _startTime = Time.time;
        }

        public void Tick()
        {
            Value = Time.time - _startTime;
            _viewController.SetTimer(Value);
        }

        public void Reset()
        {
            _startTime = Time.time;
            _viewController.SetTimer(0f);
        }
    }
}