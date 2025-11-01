using UnityEngine;

namespace GamePlay.Attack
{
    public class BurnComponent : MonoBehaviour
    {
        private float _dpsAccumulated;
        private float _timeLeft;

        public void Apply(float dps, float duration)
        {
            // Stack’lenebilir: DPS toplanır, süre büyük olanı al
            _dpsAccumulated += dps;
            _timeLeft = Mathf.Max(_timeLeft, duration);
            enabled = true;
        }

        private void Update()
        {
            if (_timeLeft <= 0f)
            {
                _dpsAccumulated = 0f;
                enabled = false;
                return;
            }

            float dt = Time.deltaTime;
            _timeLeft -= dt;

            float damageThisFrame = _dpsAccumulated * dt;
            var enemy = GetComponent<GamePlay.Enemy.EnemyController>();
            if (enemy) enemy.TakeDamage(damageThisFrame);
        }
    }
}