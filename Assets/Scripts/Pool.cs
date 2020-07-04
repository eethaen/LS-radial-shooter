using System;
using System.Collections.Generic;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class Pool<T> where T : IPoolable
    {
        private readonly Queue<T> _members;
        private readonly Transform _parent;
        private T _cachedMember;

        public Pool(int capacity, SceneEntity prefab, Events events, Settings settings, bool membersGenerateCollisionEvents = true)
        {
            _members = new Queue<T>(capacity);
            _parent = new GameObject($"{typeof(T)}Pool").GetComponent<Transform>();

            var args = new object[] { prefab, events, settings };

            for (var i = 0; i < capacity; i++)
            {
                _cachedMember = (T)Activator.CreateInstance(typeof(T), args);
                _members.Enqueue(_cachedMember);
                _cachedMember.OnInstantiated(_parent);
            }
        }

        public T Spawn(Entity.State state)
        {
            _cachedMember = _members.Dequeue();
            _cachedMember.OnSpawned(state);

            return _cachedMember;
        }

        public void Despawn(T instance)
        {
            _members.Enqueue(instance);
            instance.OnDespawned();
        }
    }
}