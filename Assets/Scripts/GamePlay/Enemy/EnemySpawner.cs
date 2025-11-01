using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils.BaseClasses;
using Random = UnityEngine.Random;

namespace GamePlay.Enemy
{
    public class EnemySpawner : SingletonBehavior<EnemySpawner>
    {
        [SerializeField] private EnemyController _enemy;
        [SerializeField] private Transform _minSpawnPoint;
        [SerializeField] private Transform _maxSpawnPoint;
        [SerializeField] private float _enemyCount = 5;

        private readonly List<EnemyController> _enemies = new();

        private Vector3 GetRandomSpawnPoint =>
            new(Random.Range(_minSpawnPoint.position.x, _maxSpawnPoint.position.x), _enemy.transform.position.y,
                Random.Range(_minSpawnPoint.position.z, _maxSpawnPoint.position.z));

        private Vector3 SpawnPoint
        {
            get
            {
                var spawnPoint = GetRandomSpawnPoint;
                return _enemies.Where(enemy => enemy.IsAlive).Any(enemy => 
                    Vector3.Distance(spawnPoint, enemy.transform.position) <= 1.25f) ? 
                    SpawnPoint : spawnPoint;
            }
        }

        private void Start()
        {
            SpawnEnemies();
            EventDispatcher.OnEnemyDie.AddListener(OnEnemyDie);
        }

        private void OnEnemyDie()
        {
            foreach (var enemy in _enemies.Where(t => !t.IsAlive))
                enemy.Respawn(SpawnPoint);
        }

        private void SpawnEnemies()
        {
            for (int i = 0; i < _enemyCount; i++)
                _enemies.Add(Instantiate(_enemy, SpawnPoint, Quaternion.identity));
        }

        public static EnemyController GetClosestEnemy(Vector3 position)
        {
            EnemyController closest = null;
            var bestSqr = float.MaxValue;

            foreach (var enemy in Instance._enemies)
            {
                if (!enemy.IsAlive) continue;
                var sqr = (enemy.transform.position - position).sqrMagnitude;

                if (!(sqr < bestSqr))
                    continue;
                bestSqr = sqr;
                closest = enemy;
            }

            return closest;
        }

        public static EnemyController GetClosestEnemyExcept(Vector3 position, EnemyController except)
        {
            EnemyController closest = null;
            float bestSqr = float.MaxValue;

            foreach (var enemy in Instance._enemies)
            {
                if (!enemy.IsAlive || enemy == except) continue;
                float sqr = (enemy.transform.position - position).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    closest = enemy;
                }
            }

            return closest;
        }
    }
}