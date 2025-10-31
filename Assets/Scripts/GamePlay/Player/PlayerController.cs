using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils.Attribute;
using UnityUtils.BaseClasses;
using UnityUtils.Extensions;

namespace GamePlay.Player
{
    public class PlayerController : AutoAssignBehaviour
    {
        public float idleCounter;
        [SerializeField, AutoAssign] private Rigidbody _rigidbody;
        [SerializeField] private FloatingJoystick _joystick;
        [SerializeField] private float _speed = 10;
        [SerializeField] private float _smoothMoveMultiplier = 2;
        public Transform target;
        
        private void Update()
        {
            var vertical = _joystick.Vertical;
            var horizontal = _joystick.Horizontal;

            Vector3 inputVector = new Vector3(horizontal, 0, vertical);
            Vector3 movement = inputVector * Time.deltaTime * _speed;
            transform.position += movement;
            _rigidbody.AddForce(movement * _smoothMoveMultiplier, ForceMode.Impulse);
            
            idleCounter = movement.sqrMagnitude == 0 ? idleCounter + Time.deltaTime : 0;
            
            //var target = References.TargetIndicator.GetTarget(); //TODO buraya target eklenecek
            
            if (target != null)
            {
                if (idleCounter > 0.1f)
                    transform.LookAtWithoutY(target);
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