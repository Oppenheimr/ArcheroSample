using System;
using System.Collections;
using Core;
using Data;
using GamePlay.Player;
using GamePlay.Skill;
using UnityEngine;
using UnityUtils.Attribute;

namespace GamePlay.Attack
{
    public class SimplePlayerGun : MonoBehaviour
    {
        public bool fire = true;
        public float baseDamage = 15;
        public float baseBulletSpeed = 30;
        
        [SerializeField, AutoAssign] private PlayerController _controller;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Bomb _bomb;
        [SerializeField] private float _fireDelay = 2.3f;
        [SerializeField] private float _angle = 45;
        
        public static int BulletPoolKey;
        private Coroutine _loop;

        protected virtual void OnEnable()
        {
            if (_loop != null) StopCoroutine(_loop);
            _loop = StartCoroutine(ShootLoop());
            BulletPoolKey = ObjectPooler.CreatePool(_bomb, 300);
        }

        private IEnumerator ShootLoop()
        {
            while (true)
            {
                // UI’daki durumdan AttackContext’i oluştur
                var context = SkillManager.Instance.CreateContext(baseDamage, _fireDelay, baseBulletSpeed, _angle);

                yield return new WaitForSeconds(context.FireDelay);

                if (!_controller.FireCondition || !fire)
                    continue;

                ShootWithContext(context);
            }
        }

        private void ShootWithContext(AttackContext context)
        {
            float spacing = 0.4f;
            int shotCount = context.Shots;
            float totalWidth = (shotCount - 1) * spacing;
            float halfWidth = totalWidth / 2f;

            for (int i = 0; i < shotCount; i++)
            {
                float offset = (i * spacing) - halfWidth;
                Vector3 spawnPos = _firePoint.position + _firePoint.right * offset;

                var insBullet = (Bomb)ObjectPooler.GetPoolObject(BulletPoolKey);
                insBullet.transform.position = spawnPos;
                insBullet.transform.rotation = _firePoint.rotation;

                // 🔹 Balistik hız vektörü helper’dan
                Vector3 v0 = ProjectileHelper.ComputeBallisticVelocity(
                    origin: spawnPos,
                    target: _controller.target.transform.position,
                    angleDeg: context.Angle,
                    gravityPos: -Physics.gravity.y
                );

                insBullet.bombRigid.linearVelocity = v0;

                var runtime = new BulletRuntime
                {
                    bounceLeft = context.BounceCount,
                    burnDuration = context.BurnDuration,
                    burnDamagePerSecond = context.BurnDps,
                    currentOrigin = _firePoint,
                    rigidbody = insBullet.bombRigid,
                    collider = insBullet.bombCollider,
                    angle = _angle
                };
                
                insBullet.Setup(context.BaseDamage, runtime, context.BulletEffects);
            }
        }
    }
}