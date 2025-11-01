using GamePlay.Attack;
using GamePlay.Enemy;
using UnityEngine;

namespace GamePlay.Skill
{
    public interface IBulletEffect
    {
        void OnHit(Enemy.EnemyController enemy, BulletRuntime data);
    }

    public sealed class BulletRuntime
    {
        public int BounceLeft;
        public float BurnDuration;
        public float BurnDps;
        public Transform CurrentOrigin; // sekmede yeni hedefe yönelmek için
        public Rigidbody Rigidbody; 
    }

    public sealed class BurnEffect : IBulletEffect
    {
        public void OnHit(Enemy.EnemyController enemy, BulletRuntime data)
        {
            if (data.BurnDuration <= 0f) return;
            // Basit: hedefte bir BurnComponent olsun, stack’lenebilir
            var burn = enemy.GetComponent<BurnComponent>();
            if (!burn) burn = enemy.gameObject.AddComponent<BurnComponent>();
            burn.Apply(data.BurnDps, data.BurnDuration); // stacking içeride ele alınır
        }
    }

    public sealed class BounceEffect : IBulletEffect
    {
        public void OnHit(EnemyController enemy, BulletRuntime runtime)
        {
            if (runtime.BounceLeft <= 0) return;
            runtime.BounceLeft--;

            var rb = runtime.Rigidbody;
            var currentPos = rb.transform.position;

            var next = EnemySpawner.GetClosestEnemyExcept(currentPos, enemy);
            if (next == null)
                return;

            // Yeni hedef yönünü hesapla
            var dir = (next.transform.position - rb.transform.position).normalized;
            rb.linearVelocity = dir * rb.linearVelocity.magnitude;
        }
    }
}