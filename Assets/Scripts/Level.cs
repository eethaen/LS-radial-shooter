using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LazySamurai.RadialShooter
{
    public class Level
    {
        private readonly Events _events;
        private readonly Settings _settings;
        private readonly List<Entity> _activeEntities;
        private readonly Pool<Enemy> _enemyPool;
        private readonly Pool<Projectile> _projectilePool;
        private readonly Player _player;

        public Level(Events events, Settings settings, List<Entity> activeEntities)
        {
            _events = events;
            _settings = settings;
            _activeEntities = activeEntities;

            _enemyPool = new Pool<Enemy>(_settings.enemyMaxCount, _settings.enemyPrefab, _events, _settings);
            _projectilePool = new Pool<Projectile>(30, _settings.projectilePrefab, _events, _settings);

            _player = new Player(_settings.playerPrefab, _events, _settings);
            _activeEntities.Add(_player);

            _events.enemyCollided += OnEnemyCollided;
            _events.projectileCollided += OnProjectileCollided;
            _events.projectileShot += OnProjectileShot;
        }

        private void OnProjectileCollided(int id)
        {
            var projectile = (Projectile)(_activeEntities.Single(e => e.Id == id));
            _projectilePool.Despawn(projectile);
            _activeEntities.Remove(projectile);
        }

        private void OnEnemyCollided(int id)
        {
            var enemy = (Enemy)(_activeEntities.Single(e => e.Id == id));
            _enemyPool.Despawn(enemy);
            _activeEntities.Remove(enemy);
        }

        private void OnProjectileShot(Entity.State state)
        {
            _activeEntities.Add(_projectilePool.Spawn(state));
        }

        public void Tick()
        {
            var count = (int)(_settings.enemyMaxCount * _settings.enemyCount.Evaluate(Time.time / _settings.maxDuration)) - _activeEntities.Count(e => e.GetType() == typeof(Enemy));

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
    }
}