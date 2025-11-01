using System;
using Core;
using GamePlay.Enemy;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils.Attribute;
using UnityUtils.BaseClasses;
using UnityUtils.Extensions;

namespace GamePlay.Player
{
    public class PlayerController : AutoAssignBehaviour
    {
        public EnemyController target;
        [SerializeField, AutoAssign] private Rigidbody _rigidbody;
        [SerializeField] private FloatingJoystick _joystick;
        [SerializeField] private float _speed = 10;
        [SerializeField] private float _smoothMoveMultiplier = 2;
        [SerializeField] private float _idleLimit = 0.2f;
        
        private float _idleCounter;
        private bool _lastFireCondition;

        public bool FireCondition
        {
            get
            {
                var condition = _idleCounter > _idleLimit;
                if (_lastFireCondition == condition) 
                    return condition;
                
                _lastFireCondition = condition;
                target = EnemySpawner.GetClosestEnemy(transform.position);

                return condition;
            }
        }

        private void Start()
        {
            EventDispatcher.OnEnemyDie.AddListener(() =>
            {
                target = EnemySpawner.GetClosestEnemy(transform.position);
            });
        }

        private void Update()
        {
            var vertical = _joystick.Vertical;
            var horizontal = _joystick.Horizontal;

            Vector3 inputVector = new Vector3(horizontal, 0, vertical);
            Vector3 movement = inputVector * Time.deltaTime * _speed;
            transform.position += movement;
            _rigidbody.AddForce(movement * _smoothMoveMultiplier, ForceMode.Impulse);
            
            _idleCounter = movement.sqrMagnitude == 0 ? _idleCounter + Time.deltaTime : 0;

            if (!FireCondition && inputVector.magnitude == 0)
                target = EnemySpawner.GetClosestEnemy(transform.position);
            
            if (target != null)
            {
                if (_idleCounter > 0.1f)
                    transform.LookAtWithoutY(target.transform);
                else
                    LookAtInputVector(inputVector);
            }
            else
            {
                LookAtInputVector(inputVector);   
            }
        }
        
        private void LookAtInputVector(Vector3 inputVector)
        {
            if (inputVector == Vector3.zero)
                return;

            Quaternion toRotation = Quaternion.LookRotation(inputVector.normalized, Vector3.up);
            transform.rotation = toRotation;
        }
    }
}