using System.Collections;
using Core;
using GamePlay.Player;
using GamePlay.Skill;
using UnityEngine;
using UnityUtils.Attribute;

namespace GamePlay.Attack
{
    public class SimplePlayerGun : MonoBehaviour
    {
        public static int ProjectilePoolKey;
        public bool fire = true;
        public float baseDamage = 15;
        public float baseBulletSpeed = 30;
        
        [SerializeField, AutoAssign] private PlayerController _controller;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Projectile _projectile;
        [SerializeField] private float _fireDelay = 2.3f;
        [SerializeField] private float _angle = 45;
        
        private Coroutine _loop;
        private const float MultipleProjectileSpacing = 0.4f;

        protected virtual void OnEnable()
        {
            if (_loop != null) StopCoroutine(_loop);
            _loop = StartCoroutine(ShootLoop());
            ProjectilePoolKey = ObjectPooler.CreatePool(_projectile, 300);
        }

        private IEnumerator ShootLoop()
        {
            while (true)
            {
                // UI’daki durumdan AttackContext’i oluştur
                var context = SkillManager.Instance.CreateContext(baseDamage, _fireDelay, baseBulletSpeed, _angle);
                yield return new WaitForSeconds(context.fireDelay);

                if (!_controller.FireCondition || !fire)
                    continue;

                ShootWithContext(context);
            }
        }

        private void ShootWithContext(AttackContext context)
        {
            var totalWidth = (context.shots - 1) * MultipleProjectileSpacing;
            var halfWidth = totalWidth / 2f;

            for (var i = 0; i < context.shots; i++)
            {
                var offset = (i * MultipleProjectileSpacing) - halfWidth;
                var spawnPos = _firePoint.position + _firePoint.right * offset;

                var insProjectile = (Projectile)ObjectPooler.GetPoolObject(ProjectilePoolKey);
                insProjectile.transform.position = spawnPos;
                insProjectile.transform.rotation = _firePoint.rotation;

                //Balistik hız vektörü helper’dan
                var velocity = ProjectileHelper.ComputeBallisticVelocity(
                    origin: spawnPos,
                    target: _controller.target.transform.position,
                    angleDeg: context.angle,
                    gravityPos: -Physics.gravity.y
                );

                insProjectile.projectileRigid.linearVelocity = velocity;

                var runtime = new ProjectileRuntimeData
                {
                    bounceLeft = context.bounceCount,
                    burnDuration = context.burnDuration,
                    burnDamagePerSecond = context.burnDamagePerSecond,
                    rigidbody = insProjectile.projectileRigid,
                    angle = _angle
                };
                
                insProjectile.Setup(context.baseDamage, runtime, context.projectileEffects);
            }
        }
    }
}