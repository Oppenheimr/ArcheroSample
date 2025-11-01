using System.Collections.Generic;
using Core;
using GamePlay.Enemy;
using GamePlay.Skill;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils.Attribute;
using UnityUtils.Extensions;

namespace GamePlay.Attack
{
    public class Bomb : MonoBehaviour
    {
        [AutoAssign] public Rigidbody bombRigid;
        [AutoAssign] public Collider bombCollider;

        private float _damage;
        private BulletRuntime _runtime;
        private List<IBulletEffect> _effects;

        private void OnCollisionEnter(Collision other) => OnEnter(other.collider);
        private void OnTriggerEnter(Collider other) => OnEnter(other);

        private void OnEnter(Component other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;
            if (!other.gameObject.TryGetComponentInParent(out EnemyController enemy)) return;

            enemy.TakeDamage(_damage);

            //Hit öncesi kalan bounce sayısını alalım
            var bounceLeft = _runtime.bounceLeft;
            
            // Bounce ve diğer efektleri tetikle
            foreach (var effect in _effects)
                effect.OnHit(enemy, _runtime);
            
            // Eğer bounce kalmadıysa havuza geri koy
            if (bounceLeft <= 0)
                ObjectPooler.PutPoolObject(SimplePlayerGun.BulletPoolKey, this);
        }

        public void Setup(float damage, BulletRuntime runtime, List<IBulletEffect> effects)
        {
            _damage = damage;
            _runtime = runtime;
            _effects = effects; // referans tutuyoruz; istersen kopya al
        }
    }
}