using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LazySamurai.RadialShooter
{
    public class Game : MonoBehaviour
    {
        public enum StopGameMode { Win, Lose }

        private Events _events;
        private Input _input;
        private EntitiesManager _entitiesManager;
        private ViewController _viewController;
        private Timer _timer;
        private ScoreBoard _scoreBoard;

        private readonly List<Entity> _activeEntities = new List<Entity>();
        private readonly List<Entity> _collidedEntities = new List<Entity>();

        private ITrasnsformable _cachedTransformable;
        private IPerishable _cachedPerishable;
        private IPlayable _cachedPlayable;
        private Entity _cachedEntity;
        private bool _paused = true;

        [SerializeField] private Settings _settings;
        [SerializeField] private Camera _camera;

        private void Awake()
        {
            _events = new Events();
            _entitiesManager = new EntitiesManager(_events, _settings, _activeEntities);
            _input = new Input();
            _viewController = new ViewController(_settings.viewPrefab, _events);

            _events.EntityCollided += OnEntityCollided;
            _events.ProjectileShot += _entitiesManager.ShootProjectile;
            _events.GameStarted += StartGame;
        }

        public void StartGame()
        {
            if (null == _timer)
            {
                _timer = new Timer(_viewController);
            }
            else
            {
                _timer.Reset();
            }

            if (null == _scoreBoard)
            {
                _scoreBoard = new ScoreBoard(_viewController);
            }
            else
            {
                _scoreBoard.Reset();
            }

            if (_activeEntities.Count(e => e.Type == typeof(Enemy)) > 0)
            {
                _entitiesManager.DespawnAll();
            }

            _viewController.ShowGameOverPanel(false);
            _viewController.ShowWinPanel(false);

            _paused = false;
        }

        public void StopGame(StopGameMode mode)
        {
            _paused = true;

            switch (mode)
            {
                case StopGameMode.Lose:
                    _viewController.ShowGameOverPanel(true);
                    break;
                case StopGameMode.Win:
                    _viewController.ShowWinPanel(true);
                    break;
            }
        }

        private void Update()
        {
            if (_paused)
            {
                return;
            }

            if (Time.frameCount % 50 == 0)
            {
                _timer.Tick();
                _entitiesManager.Tick();
            }

            if (Timer.Value >= _settings.maxDuration && _scoreBoard.Value < _settings.winScore)
            {
                StopGame(StopGameMode.Lose);
                return;
            }

            if (_scoreBoard.Value >= _settings.winScore)
            {
                StopGame(StopGameMode.Win);
                return;
            }

            _input.Process();

            HandleEntitiesUpdate();
        }

        private void FixedUpdate()
        {
            if (_paused)
            {
                return;
            }

            HandleEntitiesFixedUpdate();
            HandleCollisions();
        }

        private void HandleEntitiesUpdate()
        {
            for (var i = 0; i < _activeEntities.Count; i++)
            {
                if (_activeEntities[i] is IPlayable)
                {
                    _cachedPlayable = (IPlayable)_activeEntities[i];
                    _cachedPlayable.SetInput(_camera.ScreenToWorldPoint(_input.MousePosition), _input.LeftClick, _input.RightClick);
                }

                if (_activeEntities[i] is IPerishable)
                {
                    _cachedPerishable = (IPerishable)_activeEntities[i];
                    _cachedPerishable.UpdateLife(Time.deltaTime);

                    if (_cachedPerishable.Life <= 0f)
                    {
                        _entitiesManager.Despawn(_cachedPerishable as Entity);
                    }
                }
            }
        }

        private void HandleEntitiesFixedUpdate()
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

        // I am not happy with this collision handling system, but I am reaching the deadline
        // and it works
        private void HandleCollisions()
        {
            if (_collidedEntities.Count < 2)
            {
                return;
            }

            if (_collidedEntities.Any(e => e is IPlayable))
            {
                StopGame(StopGameMode.Lose);
                _collidedEntities.Clear();
                return;
            }

            var color = _collidedEntities[0].Color;

            if (_collidedEntities.All(enabled => enabled.Color == color))
            {
                for (var i = 0; i < _collidedEntities.Count; i++)
                {
                    _cachedEntity = _collidedEntities[i];

                    if (_cachedEntity.Color == color)
                    {
                        _entitiesManager.Despawn(_cachedEntity);
                    }
                }

                _scoreBoard.AddScore();
            }

            _collidedEntities.Clear();
        }

        private void OnEntityCollided(int id)
        {
            _cachedEntity = FindEntityById(id);

            if (null != _cachedEntity && !_collidedEntities.Contains(_cachedEntity))
            {
                _collidedEntities.Add(_cachedEntity);
            }
        }

        private Entity FindEntityById(int id)
        {
            for (var i = 0; i < _activeEntities.Count; i++)
            {
                if (_activeEntities[i].Id == id)
                {
                    return _activeEntities[i];
                }
            }

            return null;
        }
    }
}