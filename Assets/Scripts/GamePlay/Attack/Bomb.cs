using System;
using Core;
using GamePlay.Enemy;
using UnityEngine;
using UnityUtils.Attribute;
using UnityUtils.Extensions;

namespace GamePlay.Attack
{
    public class Bomb : MonoBehaviour
    {
        [AutoAssign] public Rigidbody rigid; 
        private float _damage;
        
        private void OnCollisionEnter(Collision other)
        {
            Debug.Log(other.gameObject.name);
            if (!other.gameObject.CompareTag("Enemy"))
                return;
            
            Debug.Log(other.gameObject.name + " hit");
            if (!other.gameObject.TryGetComponentInParent(out EnemyController enemy))
                return;
            
            Debug.Log(other.gameObject.name + " hit1");
            enemy.TakeDamage(_damage);
            
            ObjectPooler.PutPoolObject(SimplePlayerGun.BulletPoolKey, this);
            // Reset rigidbody
            rigid.linearVelocity = Vector3.zero;
        }

        public void Setup(float damage)
        {
            _damage = damage;
        }
    }
}