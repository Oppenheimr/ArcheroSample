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
                var ctx = SkillManager.Instance.CreateContext(baseDamage, _fireDelay, baseBulletSpeed, _angle);

                yield return new WaitForSeconds(ctx.FireDelay);

                if (!_controller.FireCondition || !fire)
                    continue;

                ShootWithContext(ctx);
            }
        }

        private void ShootWithContext(AttackContext ctx)
        {
            // ctx.Shots kadar mermi; istersen hafif açı sapması verebilirsin
            for (int i = 0; i < ctx.Shots; i++)
            {
                var insBullet = (Bomb)ObjectPooler.GetPoolObject(BulletPoolKey);
                insBullet.transform.position = _firePoint.position;
                insBullet.transform.rotation = _firePoint.rotation;

                var position = insBullet.transform.position;
                var targetPos = _controller.target.transform.position;

                float height = position.y - targetPos.y;
                float horiz = Vector3.Distance(targetPos, new Vector3(position.x, targetPos.y, position.z));

                var dir = Direction(targetPos);
                float speed = (float)CalculateSpeed(height, horiz, ctx.Angle, -Physics.gravity.y);
                insBullet.rigid.linearVelocity = dir * speed;

                // Bomb’a runtime efektleri ver
                var runtime = new BulletRuntime
                {
                    BounceLeft = ctx.BounceCount,
                    BurnDuration = ctx.BurnDuration,
                    BurnDps = ctx.BurnDps,
                    CurrentOrigin = _firePoint,
                    Rigidbody = insBullet.rigid
                };
                insBullet.Setup(ctx.BaseDamage, runtime, ctx.BulletEffects);
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