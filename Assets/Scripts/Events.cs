using System;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class Events
    {
        public Action<Type, int> entityCollided;
        public Action<int> enemyCollided;
        public Action<int> projectileCollided;
        public Action<Entity.State> projectileShot;
        public Action TimeIsUp;
        public Action GameOver;
    }
}