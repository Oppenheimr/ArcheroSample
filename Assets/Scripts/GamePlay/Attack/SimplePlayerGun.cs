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
            BulletPoolKey = ObjectPooler.CreatePool(_bomb, 50);
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

                insBullet.bombCollider.isTrigger = false;
                insBullet.Setup(context.BaseDamage, runtime, context.BulletEffects);
            }
        }
        
        public static double CalculateSpeed(double height, double horizontalDistance, double angle, double gravity)
        {
            angle = angle * Mathf.Deg2Rad; // Açıyı dereceden radyana çevir
            return (Math.Sqrt(gravity) * horizontalDistance * (1 / Math.Cos(angle))) / Math.Sqrt(2 * height + 2 * horizontalDistance * Math.Tan(angle));
        }
        
        private Vector3 Direction(Vector3 target)
        {
            var position = _firePoint.position;
            var targetPosition = target;
                
            // Hedef ile mevcut konum arasındaki vektörü hesapla
            Vector3 targetDirection = targetPosition - new Vector3(position.x, targetPosition.y, position.z);

            // Yönü yukarı doğru _angle kadar döndür
            Quaternion rotation = Quaternion.AngleAxis(_angle, RotateVectorAroundY(targetDirection, 270).normalized);
            Vector3 rotatedDirection = rotation * targetDirection;

            return rotatedDirection.normalized;
        }
        
        public static Vector3 RotateVectorAroundY(Vector3 vector, float degree)
        {
            // Y ekseninde 90 derece döndürme için birim bir dönüş quaternion'i oluştur
            Quaternion rotation = Quaternion.Euler(0, degree, 0);

            // Vektörü dönüştür
            Vector3 rotatedVector = rotation * vector;

            return rotatedVector;
        }
    }
}