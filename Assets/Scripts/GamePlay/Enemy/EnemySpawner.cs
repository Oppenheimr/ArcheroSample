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
            new (Random.Range(_minSpawnPoint.position.x, _maxSpawnPoint.position.x), _enemy.transform.position.y,
                Random.Range(_minSpawnPoint.position.z, _maxSpawnPoint.position.z));
        
        private void Start()
        {
            SpawnEnemies();
            EventDispatcher.OnEnemyDie.AddListener(OnEnemyDie);
        }

        private void OnEnemyDie()
        {
            foreach (var enemy in _enemies.Where(t => !t.IsAlive))
                enemy.Respawn(GetRandomSpawnPoint);
        }

        private void SpawnEnemies()
        {
            for (int i = 0; i < _enemyCount; i++)
                _enemies.Add(Instantiate(_enemy, GetRandomSpawnPoint, Quaternion.identity));
        }
        
        public static EnemyController GetClosestEnemy(Vector3 position)
        {
            EnemyController closestEnemyController = null;
            var closestDistance = float.MaxValue;

            foreach (var enemy in Instance._enemies)
            {
                if (!enemy.IsAlive)
                    continue;

                var distance = Vector3.Distance(position, enemy.transform.position);
                if (!(distance < closestDistance)) continue;
                closestDistance = distance;
                closestEnemyController = enemy;
            }

            return closestEnemyController;
        }
    }
}