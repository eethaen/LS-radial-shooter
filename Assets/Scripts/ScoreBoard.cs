using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class ScoreBoard
    {
        private readonly ViewManager _view;

        public int Value { get; private set; }

        public ScoreBoard(ViewManager view)
        {
            _view = view;
        }

        public void AddScore()
        {
            Value++;
            _view.SetScore(Value);
        }

        public void Reset()
        {
            Value = 0;
            _view.SetScore(0);
        }
    }
}