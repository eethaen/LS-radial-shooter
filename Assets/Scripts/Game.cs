using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class Game : MonoBehaviour
    {
        public enum StopGameMode { Win, Lose }

        private Events _events;
        private EntitiesManager _entitiesManager;
        private Input _input;
        private Timer _timer;
        private ScoreBoard _scoreBoard;

        private readonly List<Entity> _activeEntities = new List<Entity>();
        private readonly List<Entity> _collidedEntities = new List<Entity>();

        private ITrasnsformable _cachedTransformable;
        private Entity _cachedEntity;
        private bool _paused = true;

        [SerializeField] private Settings _settings;
        [SerializeField] private Camera _camera;
        [SerializeField] private ViewManager _view;

        private void Awake()
        {
            _events = new Events();
            _entitiesManager = new EntitiesManager(_events, _settings, _activeEntities);
            _input = new Input();

            _events.entityCollided += OnEntityCollided;
            _events.projectileShot += _entitiesManager.ShootProjectile;
        }

        public void StartGame()
        {
            if (null == _timer)
            {
                _timer = new Timer(_view);
            }
            else
            {
                _timer.Reset();
            }

            if (null == _scoreBoard)
            {
                _scoreBoard = new ScoreBoard(_view);
            }
            else
            {
                _scoreBoard.Reset();
            }

            if (_activeEntities.Count > 1)
            {
                _entitiesManager.DespawnAll();
            }

            _paused = false;
        }

        public void StopGame(StopGameMode mode)
        {
            _paused = true;

            if (mode == StopGameMode.Lose)
            {
                _view.ShowGameOverPanel();
            }
            else
            {
                _view.ShowWinPanel();
            }
        }

        private void Update()
        {
            if (_paused)
            {
                return;
            }

            _timer.Tick();

            if (_timer.Value > _settings.maxDuration && _scoreBoard.Value < _settings.winScore)
            {
                StopGame(StopGameMode.Lose);
                return;
            }

            if (_timer.Value < _settings.maxDuration && _scoreBoard.Value >= _settings.winScore)
            {
                StopGame(StopGameMode.Win);
                return;
            }

            _input.Process();

            if (Time.frameCount % 50 == 0)
            {
                _entitiesManager.Tick();
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
            if (_paused)
            {
                return;
            }

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

            HandleCollisions();
        }

        private void HandleCollisions()
        {
            if (_collidedEntities.Count < 2)
            {
                return;
            }

            if (_collidedEntities.Any(e => e is IPlayable))
            {
                StopGame(StopGameMode.Lose);
                return;
            }

            var color = _collidedEntities[0].Color;

            for (var i = 0; i < _collidedEntities.Count; i++)
            {
                _cachedEntity = _collidedEntities[i];

                if (_cachedEntity.Color == color)
                {
                    _scoreBoard.AddScore();
                    _entitiesManager.Despawn(_cachedEntity);
                }
            }

            _collidedEntities.Clear();
        }

        private void OnEntityCollided(int id)
        {
            _collidedEntities.Add(_activeEntities.Single(e => e.Id == id));
        }
    }
}