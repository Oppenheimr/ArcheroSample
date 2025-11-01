using GamePlay.Enemy;
using UnityEngine;

namespace GamePlay.Attack
{
    public class BurnComponent : MonoBehaviour
    {
        private EnemyController _controller;
        private float _damagePerSecondAccumulated;
        private float _timeLeft;

        public void Apply(float damagePerSecond, float duration)
        {
            // Stack’lenebilir: DPS toplanır, süre büyük olanı al
            _damagePerSecondAccumulated += damagePerSecond;
            _timeLeft = Mathf.Max(_timeLeft, duration);
            enabled = true;

            _controller = GetComponent<EnemyController>();
        }

        private void Update()
        {
            if (!_controller.IsAlive)
            {
                _damagePerSecondAccumulated = 0f;
                enabled = false;
                return;
            }
            
            if (_timeLeft <= 0f)
            {
                _damagePerSecondAccumulated = 0f;
                enabled = false;
                return;
            }

            
            _timeLeft -= Time.deltaTime;
            _controller.TakeDamage(_damagePerSecondAccumulated * Time.deltaTime);
        }
    }
}