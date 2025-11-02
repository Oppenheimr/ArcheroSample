using System.Collections;
using System.Collections.Generic;
using Core;
using GamePlay.Enemy;
using GamePlay.Skill;
using GamePlay.Skill.Effect;
using UnityEngine;
using UnityUtils.Attribute;
using UnityUtils.Extensions;

namespace GamePlay.Attack
{
    public class Projectile : MonoBehaviour
    {
        [AutoAssign] public Rigidbody projectileRigid;
        [AutoAssign] public Collider projectileCollider;
        
        private float _damage;
        private ProjectileRuntimeData _runtimeData;
        private List<IProjectileEffect> _effects;
        private const float PoolDelay = 2f;
        
        private const float MinOrientSpeedSqr = 0.01f;
        private const float RotationSlerpSpeed = 30f;
        private readonly Vector3 _modelRotationOffset = Vector3.zero;
        private bool _movementCompleted;
        
        private void OnEnable()
        {
            // Havuzdan alınınca rotation'ı sıfırla/veya önceki spinin etkisini sil
            if (projectileRigid == null) 
                return;
            projectileRigid.angularVelocity = Vector3.zero;
            projectileRigid.WakeUp();
        }
        
        private void OnCollisionEnter(Collision other) => OnEnter(other.collider);
        private void OnTriggerEnter(Collider other) => OnEnter(other);

        private void FixedUpdate()
        {
            if (projectileRigid == null) return;
            if (_movementCompleted) return;
        
            if (!(projectileRigid.linearVelocity.sqrMagnitude > MinOrientSpeedSqr)) return;
            // hedef rotasyon: hız yönünü forward olarak al
            var targetRot = Quaternion.LookRotation(projectileRigid.linearVelocity.normalized, Vector3.up);
        
            // model offset varsa uygula (model farklı eksene bakıyorsa)
            if (_modelRotationOffset != Vector3.zero)
                targetRot = targetRot * Quaternion.Euler(_modelRotationOffset);
        
            // yumuşak slerp kullan (MoveRotation fiziksel olarak daha güvenli)
            var time = 1f - Mathf.Exp(-RotationSlerpSpeed * Time.fixedDeltaTime);
            var newRot = Quaternion.Slerp(projectileRigid.rotation, targetRot, time);
            projectileRigid.MoveRotation(newRot);
        }
        
        public void Setup(float damage, ProjectileRuntimeData runtimeData, List<IProjectileEffect> effects)
        {
            _damage = damage;
            _runtimeData = runtimeData;
            _effects = effects;
            projectileCollider.isTrigger = true;
            _movementCompleted = false;
        }

        
        private void OnEnter(Component other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;
            if (!other.gameObject.TryGetComponentInParent(out EnemyController enemy)) return;

            enemy.TakeDamage(_damage);

            //Hit öncesi kalan bounce sayısını alalım
            var bounceLeft = _runtimeData.bounceLeft;
            
            // Bounce ve diğer efektleri tetikle
            foreach (var effect in _effects)
                effect.OnHit(enemy, _runtimeData);
            
            // Eğer bounce kalmadıysa havuza geri koy
            if (bounceLeft <= 0)
                StartCoroutine(PutPoolDelayed());
        }
        
        private IEnumerator PutPoolDelayed()
        {
            projectileCollider.isTrigger = false;
            _movementCompleted = true;
            yield return new WaitForSeconds(PoolDelay);
            projectileCollider.isTrigger = true;
            ObjectPooler.PutPoolObject(SimplePlayerGun.ProjectilePoolKey, this);
        }
    }
}