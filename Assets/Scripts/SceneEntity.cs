using System;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class SceneEntity : MonoBehaviour
    {
        private Events _events;

        public int Id { get; private set; }
        public Type Type { get; private set; }

        public void Initialize(Events events, Type type, int id)
        {
            _events = events;

            Type = type;
            Id = id;
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            CustomDebug.Assert(null != _events, "Events not initilized in scene entity");
            _events.EntityCollided.Invoke(Id);
        }
    }
}