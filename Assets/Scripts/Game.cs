using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class Game : MonoBehaviour
    {
        private Events _events;
        private Level _level;
        private Input _input;
        private Timer _timer;
        private ITrasnsformable _cachedTransformable;
        private readonly List<Entity> _activeEntities = new List<Entity>();

        [SerializeField] private Settings _settings;
        [SerializeField] private Camera _camera;

        private void Awake()
        {
            _events = new Events();
            _level = new Level(_events, _settings, _activeEntities);
            _input = new Input();

            _events.entityCollided += OnEntityCollided;
        }

        private void OnEntityCollided(System.Type type, int id)
        {
            if (type == typeof(Player))
            {
                _events.GameOver.Invoke();
            }
            else if (type == typeof(Enemy))
            {
                _events.enemyCollided.Invoke(id);
            }
            else if (type == typeof(Projectile))
            {
                _events.projectileCollided.Invoke(id);
            }
        }

        private void Start()
        {
            _timer = new Timer(_events);
        }

        private void Update()
        {
            _timer.Tick();
            _input.Process();

            if (Time.frameCount % 50 == 0)
            {
                _level.Tick();
            }

            for (var i = 0; i < _activeEntities.Count; i++)
            {
                if (_activeEntities[i] is IPlayable)
                {
                    ((IPlayable)_activeEntities[i]).SetInput(_camera.ScreenToWorldPoint(_input.MousePosition), _input.LeftClick, _input.RightClick);
                }
            }
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < _activeEntities.Count; i++)
            {
                if (_activeEntities[i] is ITrasnsformable)
                {
                    _cachedTransformable = (ITrasnsformable)_activeEntities[i];

                    _cachedTransformable.Rotate();
                    _cachedTransformable.Move();
                    _cachedTransformable.Scale();
                }
            }
        }
    }
}