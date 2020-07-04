using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class Input
    {
        public bool LeftClick { get; private set; }
        public bool RightClick { get; private set; }
        public Vector2 MousePosition { get; private set; }

        public void Process()
        {
            LeftClick = false;
            RightClick = false;

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                LeftClick = true;
            }
            else if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                RightClick = true;
            }

            MousePosition = UnityEngine.Input.mousePosition;
        }
    }
}