using Core;
using GamePlay.Attack;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils.Attribute;
using AutoAssignScope = UnityUtils.Attribute.AutoAssignAttribute.AutoAssignScope;

namespace GamePlay.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField, AutoAssign(scope: AutoAssignScope.Children)] private Slider _healthBar;
        [SerializeField] private float _maxHealth = 100f;
        private float _currentHealth;

        public bool IsAlive => _currentHealth > 0;

        private void Awake()
        {
            _currentHealth = _maxHealth;
            _healthBar.maxValue = _maxHealth;
            _healthBar.value = _currentHealth;
        }

        public void Respawn(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
            _currentHealth = _maxHealth;
            _healthBar.value = _currentHealth;
            
            if (gameObject.TryGetComponent(out BurnEffectComponent burn))
                burn.enabled = false;
        }
        
        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;
            _healthBar.value = _currentHealth;
            if (_currentHealth <= 0)
                EventDispatcher.OnEnemyDieEvent();
        }
    }
}