using GamePlay.Attack;
using GamePlay.Enemy;
using UnityEngine;

namespace GamePlay.Skill
{
    public interface IBulletEffect
    {
        void OnHit(EnemyController enemy, BulletRuntime data);
    }

    public sealed class BulletRuntime
    {
        public int bounceLeft;
        public float burnDuration;
        public float burnDamagePerSecond;
        public float angle;
        public Transform currentOrigin; // sekmede yeni hedefe yönelmek için
        public Rigidbody rigidbody;
        public Collider collider;
    }

    public sealed class BurnEffect : IBulletEffect
    {
        public void OnHit(EnemyController enemy, BulletRuntime data)
        {
            if (data.burnDuration <= 0f) return;
            // Basit: hedefte bir BurnComponent olsun, stack’lenebilir
            var burn = enemy.GetComponent<BurnComponent>();
            if (!burn) burn = enemy.gameObject.AddComponent<BurnComponent>();
            burn.Apply(data.burnDamagePerSecond, data.burnDuration); // stacking içeride ele alınır
        }
    }

    public sealed class BounceEffect : IBulletEffect
    {
        public void OnHit(EnemyController enemy, BulletRuntime runtime)
        {
            if (runtime.bounceLeft <= 0) return;
            runtime.bounceLeft--;

            var rb = runtime.rigidbody;
            var from = rb.position;

            var next = EnemySpawner.GetClosestEnemyExcept(from, enemy);
            if (next == null) return;

            // Yeni hız vektörü
            Vector3 v0 = ProjectileHelper.ComputeBallisticVelocity(
                origin: from,
                target: next.transform.position,
                angleDeg: runtime.angle,
                gravityPos: -Physics.gravity.y
            );

            //Yön doğrultusunda ileri alalım biraz
            // const float separateDist = 0.35f;
            // Vector3 sepPos = from + v0.normalized * separateDist;
            // rb.position = sepPos;
            //
            rb.linearVelocity = v0;
            rb.angularVelocity = Vector3.zero;
            rb.WakeUp();
        }

    }
}