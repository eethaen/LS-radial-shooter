using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LazySamurai.RadialShooter
{
    public class EntitiesManager
    {
        private readonly Settings _settings;
        private readonly List<Entity> _activeEntities;
        private readonly Pool<Enemy> _enemyPool;
        private readonly Pool<Projectile> _projectilePool;
        private readonly Entity _player;

        private Entity _cachedEntity;

        public EntitiesManager(Events events, Settings settings, List<Entity> activeEntities)
        {
            _settings = settings;
            _activeEntities = activeEntities;

            var initialState = new Entity.State()
            {
                Position = Vector2.zero,
                Rotation = Quaternion.identity,
                Color = Color.cyan,
            };

            _enemyPool = new Pool<Enemy>(_settings.enemyMaxCount, _settings.enemyPrefab, initialState, events, _settings);
            _projectilePool = new Pool<Projectile>(30, _settings.projectilePrefab, initialState, events, _settings);
            _player = new Player(_settings.playerPrefab, initialState, events, _settings);
            _activeEntities.Add(_player);
        }

        public void Tick()
        {
            var count = (int)(_settings.enemyMaxCount * _settings.enemyCount.Evaluate(Timer.Value / _settings.maxDuration)) - _activeEntities.Count(e => e.GetType() == typeof(Enemy));

            if (count <= 0)
            {
                return;
            }

            var state = new Entity.State();

            var rnd = Random.Range(0f, 1f);
            var radius = Mathf.Lerp(_settings.enemyMinRadius, _settings.enemyMaxRadius, rnd);
            var offset = Mathf.Lerp(0f, 360f, rnd);
            var angle = 0f;

            for (int i = 0; i < count; i++)
            {
                angle = (i * 360f / count + offset) * Mathf.Deg2Rad;

                state.Position.x = radius * Mathf.Cos(angle);
                state.Position.y = radius * Mathf.Sin(angle);
                state.Rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)_player.Transform.position - state.Position);

                state.Color = Random.Range(0f, 1f) > 0.5f ? Color.red : Color.blue;

                _activeEntities.Add(_enemyPool.Spawn(state));
            }
        }

        public void ShootProjectile(Entity.State state)
        {
            _activeEntities.Add(_projectilePool.Spawn(state));
        }

        public void Despawn(Entity entity)
        {
            if (entity.Type == typeof(Enemy))
            {
                _enemyPool.Despawn((Enemy)entity);
            }
            else if (entity.Type == typeof(Projectile))
            {
                _projectilePool.Despawn((Projectile)entity);
            }

            _activeEntities.Remove(entity);
        }

        public void DespawnAll()
        {
            for (var i = _activeEntities.Count - 1; i >= 0; i--)
            {
                _cachedEntity = _activeEntities[i];

                if (_cachedEntity.Type == typeof(Player))
                {
                    continue;
                }

                Despawn(_cachedEntity);
            }

            CustomDebug.Assert(_activeEntities.Count(e => e.Type == typeof(Enemy)) == 0);
            CustomDebug.Assert(_activeEntities.Count(e => e.Type == typeof(Projectile)) == 0);
            CustomDebug.Assert(_activeEntities.Count(e => e.Type == typeof(Player)) == 1);
        }
    }
}