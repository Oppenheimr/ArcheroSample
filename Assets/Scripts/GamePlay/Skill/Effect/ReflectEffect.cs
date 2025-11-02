using GamePlay.Attack;
using GamePlay.Enemy;
using UnityEngine;

namespace GamePlay.Skill.Effect
{
    public sealed class ReflectEffect : IProjectileEffect
    {
        public void OnHit(EnemyController enemy, ProjectileRuntimeData runtimeData)
        {
            if (runtimeData.bounceLeft <= 0) return;
            runtimeData.bounceLeft--;

            var rb = runtimeData.rigidbody;
            var from = rb.position;

            var next = EnemySpawner.GetClosestEnemyExcept(from, enemy);
            if (next == null) return;

            // Yeni hız vektörü
            var velocity = ProjectileHelper.ComputeBallisticVelocity(
                origin: from,
                target: next.transform.position,
                angleDeg: runtimeData.angle,
                gravityPos: -Physics.gravity.y
            );

            rb.linearVelocity = velocity;
            rb.angularVelocity = Vector3.zero;
            rb.WakeUp();
        }
    }
}