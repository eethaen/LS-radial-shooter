using System;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class SceneEntity : MonoBehaviour
    {
        private Events _events;
        private Type _type;

        public int Id { get; private set; }
        public Type Type { get; private set; }

        public void Initialize(Events events, Type type)
        {
            _events = events;
            Type = type;
            Id = transform.GetInstanceID();
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            CustomDebug.Log($"Collision {Type}");
            CustomDebug.Assert(null != _events, "Events not initilized in scene entity");
            _events.entityCollided(Type, Id);
        }
    }
}