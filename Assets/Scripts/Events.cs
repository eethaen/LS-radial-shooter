using System;

namespace LazySamurai.RadialShooter
{
    public class Events
    {
        public Action<int> entityCollided;
        public Action<Entity.State> projectileShot;
        public Action TimeIsUp;
    }
}