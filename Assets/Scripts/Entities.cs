using System;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public abstract class Entity
    {
        public struct State
        {
            public Vector2 Position;
            public Quaternion Rotation;
            public Color Color;
        }

        protected Events _events = null;
        protected Settings _settings = null;

        private readonly SceneEntity _sceneEntity = null;

        private int _id = -1;
        private Transform _transform = null;
        private Rigidbody2D _rigidbody = null;
        private SpriteRenderer _renderer = null;

        protected Entity(SceneEntity prefab, State state, Events events, Settings settings)
        {
            _settings = settings;
            _events = events;

            Type = this.GetType();

            _sceneEntity = SceneEntity.Instantiate(prefab);
            _sceneEntity.Initialize(_events, Type, Id);

            Transform.position = state.Position;
            Transform.rotation = state.Rotation;
            Color = state.Color;
        }

        public Type Type { get; private set; }

        public bool Active
        {
            get
            {
                CustomDebug.Assert(null != _sceneEntity, "SceneEntity is not instantiated");
                return _sceneEntity.gameObject.activeInHierarchy;
            }

            set
            {
                CustomDebug.Assert(null != _sceneEntity, "SceneEntity is not instantiated");
                _sceneEntity.gameObject.SetActive(value);
            }
        }

        public int Id
        {
            get
            {
                if (_id == -1)
                {
                    CustomDebug.Assert(null != Transform, "Transform is not defined");
                    _id = Transform.GetInstanceID();
                }

                return _id;
            }
        }

        public Transform Transform
        {
            get
            {
                if (null == _transform)
                {
                    CustomDebug.Assert(null != _sceneEntity, "SceneEntity is not instantiated");
                    _transform = _sceneEntity.transform;
                }

                return _transform;
            }
        }

        public Rigidbody2D RigidBody
        {
            get
            {
                if (null == _rigidbody)
                {
                    CustomDebug.Assert(null != _sceneEntity, "SceneEntity is not instantiated");
                    _rigidbody = _sceneEntity.GetComponent<Rigidbody2D>();
                }

                return _rigidbody;
            }
        }

        public Color Color
        {
            get
            {
                if (null == _renderer)
                {
                    CustomDebug.Assert(null != _sceneEntity, "SceneEntity is not instantiated");
                    _renderer = _sceneEntity.GetComponent<SpriteRenderer>();
                }

                return _renderer.color;
            }

            set
            {
                if (null == _renderer)
                {
                    CustomDebug.Assert(null != _sceneEntity, "SceneEntity is not instantiated");
                    _renderer = _sceneEntity.GetComponent<SpriteRenderer>();
                }

                _renderer.color = value;
            }
        }
    }

    public abstract class PoolableEntity : Entity, IPoolable
    {
        public PoolableEntity(SceneEntity prefab, State state, Events events, Settings settings) : base(prefab, state, events, settings)
        {
        }

        public virtual void OnInstantiated(Transform parent)
        {
            Transform.SetParent(parent);
            Active = false;
        }

        public virtual void OnSpawned(State state)
        {
            Transform.position = state.Position;
            Transform.rotation = state.Rotation;
            Color = state.Color;
            Active = true;
        }

        public virtual void OnDespawned()
        {
            Active = false;
        }
    }

    public class Player : Entity, ITrasnsformable, IPlayable
    {
        private Vector2 _mousePosition;

        public Player(SceneEntity prefab, State state, Events events, Settings settings) : base(prefab, state, events, settings)
        {
        }

        public void Move()
        {
        }

        public void Rotate()
        {
            RigidBody.MoveRotation(Quaternion.LookRotation(Transform.forward, _mousePosition - (Vector2)Transform.position));
        }

        public void Scale()
        {
        }

        public void SetInput(Vector2 MousePosition, bool leftClick, bool rightClick)
        {
            _mousePosition = MousePosition;

            if (leftClick || rightClick)
            {
                Shoot(leftClick ? Color.red : Color.blue);
            }
        }

        private void Shoot(Color color)
        {
            var state = new State
            {
                Position = Transform.position + 1.2f * Transform.up,
                Rotation = Transform.rotation,
                Color = color,
            };

            _events.ProjectileShot.Invoke(state);
        }
    }

    public class Enemy : PoolableEntity, ITrasnsformable
    {
        public Enemy(SceneEntity prefab, State state, Events events, Settings settings) : base(prefab, state, events, settings)
        {
        }

        public void Move()
        {
            var t = Timer.Value / _settings.maxDuration;
            var oscillation = _settings.enemyOscillation.Evaluate(t);

            RigidBody.velocity = _settings.enemyMaxSpeed * _settings.enemySpeed.Evaluate(t) * Transform.up +
                                 _settings.enemyOscillationAmplitude * oscillation * Mathf.Sin(_settings.enemyOscillationFrequency * oscillation * Time.time) * Transform.right;
        }

        public void Rotate()
        {
        }

        public void Scale()
        {
        }
    }

    public class Projectile : PoolableEntity, ITrasnsformable, IPerishable
    {
        public float Life { get; private set; }

        public Projectile(SceneEntity prefab, State state, Events events, Settings settings) : base(prefab, state, events, settings)
        {
        }

        public void Move()
        {
            RigidBody.velocity = _settings.projectileSpeed * Transform.up;
        }

        public void Rotate()
        {
        }

        public void Scale()
        {
        }

        public override void OnSpawned(State state)
        {
            base.OnSpawned(state);

            Life = _settings.projectileLife;
        }

        public void UpdateLife(float deltaTime)
        {
            Life -= deltaTime;
        }
    }
}