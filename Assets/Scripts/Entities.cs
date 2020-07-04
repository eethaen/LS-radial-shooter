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
        protected SceneEntity _sceneEntity = null;

        private int _id = -1;
        private Transform _transform = null;
        private Rigidbody2D _rigidbody = null;
        private SpriteRenderer _renderer = null;

        protected Entity(SceneEntity prefab, Events events, Settings settings)
        {
            _settings = settings;
            _events = events;
            _sceneEntity = SceneEntity.Instantiate(prefab);
            _sceneEntity.Initialize(_events, this.GetType());
        }

        public int Id
        {
            get
            {
                if (_id == -1)
                {
                    _id = _transform?.GetInstanceID() ?? -1;
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
                    _transform = _sceneEntity?.transform;
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
                    _rigidbody = _sceneEntity?.GetComponent<Rigidbody2D>();
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
                    _renderer = _sceneEntity?.GetComponent<SpriteRenderer>();
                }

                return _renderer.color;
            }

            set
            {
                if (null == _renderer)
                {
                    _renderer = _sceneEntity?.GetComponent<SpriteRenderer>();
                }

                _renderer.color = value;
            }
        }

        public bool Active => _sceneEntity.gameObject.activeInHierarchy;
    }

    public abstract class PoolableEntity : Entity, IPoolable
    {
        public PoolableEntity(SceneEntity prefab, Events events, Settings settings) : base(prefab, events, settings)
        {
        }

        public void OnInstantiated(Transform parent)
        {
            Transform.SetParent(parent);
            _sceneEntity.gameObject.SetActive(false);
        }

        public void OnSpawned(State state)
        {
            Transform.position = state.Position;
            Transform.rotation = state.Rotation;
            Color = state.Color;
            _sceneEntity.gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            _sceneEntity.gameObject.SetActive(false);
        }
    }

    public class Player : Entity, ITrasnsformable, IPlayable
    {
        private Vector2 _mousePosition;

        public Player(SceneEntity prefab, Events events, Settings settings) : base(prefab, events, settings)
        {
            _sceneEntity.name = "Player";
            Transform.position = Vector3.zero;
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

            _events.projectileShot.Invoke(state);
        }
    }

    public class Enemy : PoolableEntity, ITrasnsformable
    {
        public Enemy(SceneEntity prefab, Events events, Settings settings) : base(prefab, events, settings)
        {
        }

        public void Move()
        {
            var t = Time.time / _settings.maxDuration;
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

    public class Projectile : PoolableEntity, ITrasnsformable
    {
        public Projectile(SceneEntity prefab, Events events, Settings settings) : base(prefab, events, settings)
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
    }
}