using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
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
        private const float Lifetime = 5f; 
        
        private const float MinOrientSpeedSqr = 0.01f;
        private const float RotationSlerpSpeed = 30f;
        private readonly Vector3 _modelRotationOffset = Vector3.zero;
        private bool _movementCompleted;
        
        // Her enable'da yeni bir ömür döngüsü token'ı
        private CancellationTokenSource _reuseCts;
        
        private void OnEnable()
        {
            _reuseCts = new CancellationTokenSource();
            
            // Havuzdan alınınca rotation'ı sıfırla/veya önceki spinin etkisini sil
            if (projectileRigid == null) 
                return;
            projectileRigid.angularVelocity = Vector3.zero;
            projectileRigid.WakeUp();
            LifetimeCountdownAsync(_reuseCts.Token).Forget();
        }
        
        private void OnDisable()
        {
            // Bu projectile yeniden havuza dönerken bekleyen async işleri iptal et
            if (_reuseCts == null)
                return;
            _reuseCts.Cancel();
            _reuseCts.Dispose();
            _reuseCts = null;
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
            var bounceLeftBefore = _runtimeData.bounceLeft;
            
            // Bounce ve diğer efektleri tetikle
            foreach (var effect in _effects)
                effect.OnHit(enemy, _runtimeData);
            
            // Eğer bounce kalmadıysa havuza geri koy
            if (bounceLeftBefore <= 0)
                PutPoolDelayedAsync(_reuseCts?.Token ?? this.GetCancellationTokenOnDestroy()).Forget();
        }
        
        private async UniTaskVoid LifetimeCountdownAsync(CancellationToken token)
        {
            try
            {
                // 5 saniye bekle, eğer iptal edilmezse mermiyi havuza gönder
                await UniTask.Delay(System.TimeSpan.FromSeconds(Lifetime), DelayType.DeltaTime, PlayerLoopTiming.Update, token);
            }
            catch (System.OperationCanceledException) { return; }

            if (!token.IsCancellationRequested)
                ObjectPooler.PutPoolObject(SimplePlayerGun.ProjectilePoolKey, this);
        }
        
        private async UniTaskVoid PutPoolDelayedAsync(CancellationToken token)
        {
            if (projectileCollider) projectileCollider.isTrigger = false;
            _movementCompleted = true;
            
            try
            {
                await UniTask.Delay(
                    System.TimeSpan.FromSeconds(PoolDelay),
                    DelayType.DeltaTime,
                    PlayerLoopTiming.Update,
                    token
                );
            }
            catch (System.OperationCanceledException) { return; }

            if (projectileCollider) projectileCollider.isTrigger = true;

            // Token iptal edilmediyse havuza iade et
            if (!token.IsCancellationRequested)
                ObjectPooler.PutPoolObject(SimplePlayerGun.ProjectilePoolKey, this);
        }
    }
}