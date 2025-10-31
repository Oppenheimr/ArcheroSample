using UnityEngine;
using UnityUtils.Attribute;
using UnityUtils.BaseClasses;
using AutoAssignScope = UnityUtils.Attribute.AutoAssignAttribute.AutoAssignScope;

namespace GamePlay
{
    public class PlayerController : AutoAssignBehaviour
    {
        [SerializeField, AutoAssign] private Rigidbody _rb;           // aynı GO’dan
        [SerializeField, AutoAssign(scope: AutoAssignScope.Children)]
        private Animator _anim;                                                        // child’tan
    }
}