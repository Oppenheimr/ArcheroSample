using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils.Attribute;
using UnityUtils.BaseClasses;
using AutoAssignScope = UnityUtils.Attribute.AutoAssignAttribute.AutoAssignScope;

namespace GamePlay
{
    public class PlayerController : AutoAssignBehaviour
    {
        [SerializeField, AutoAssign] private Rigidbody _rigidbody;
        [SerializeField] private FloatingJoystick _joystick;
        [SerializeField] private float _speed = 10;
        [SerializeField] private float _smoothMoveMultiplier = 2;
        
        private void Update()
        {
            var vertical = _joystick.Vertical;
            var horizontal = _joystick.Horizontal;

            Vector3 inputVector = new Vector3(horizontal, 0, vertical);
            Vector3 movement = inputVector * Time.deltaTime * _speed;
            transform.position += movement;
            _rigidbody.AddForce(movement * _smoothMoveMultiplier, ForceMode.Impulse);
            
        }
    }
}