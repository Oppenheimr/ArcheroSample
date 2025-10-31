using System;
using System.Collections;
using Core;
using GamePlay.Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils.Attribute;

namespace GamePlay.Gun
{
    public class SimplePlayerGun : MonoBehaviour
    {
        public bool fire = true;
        public float baseDamage = 15;
        public float baseBulletSpeed = 30;
        
        [SerializeField, AutoAssign] private PlayerController _controller;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody _bulletPrefab;
        [SerializeField] private float _idleLimit = 1.5f;
        [SerializeField] private float _fireDelay = 2.3f;
        [SerializeField] private float _angle = 45;
        
        protected virtual void OnEnable()
        {
            StartCoroutine(ShootLoop());
        }
        
        private IEnumerator ShootLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(_fireDelay);
                if (!FireCondition)
                    continue;
                
                Shoot();
            }
        }

        private void Shoot()
        {
            if (!fire)
                return;
            
            var insBullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
            
            var position = insBullet.transform.position;
            var targetPosition = _controller.target.position;
            float height = position.y - targetPosition.y;
            float horizontalDistance = Vector3.Distance(targetPosition, new Vector3(position.x, targetPosition.y, position.z));

            insBullet.linearVelocity = Direction(targetPosition) * (float)CalculateSpeed(height, horizontalDistance, _angle, -Physics.gravity.y);
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
        
        private bool FireCondition
        {
            get
            {
                if (!_controller)
                    return false;
                
                return _controller.idleCounter > _idleLimit;
            }
        }
    }
}