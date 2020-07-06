using System;

namespace LazySamurai.RadialShooter
{
    public class Events
    {
        public Action<int> EntityCollided;
        public Action<Entity.State> ProjectileShot;
        public Action GameStarted;
    }
}