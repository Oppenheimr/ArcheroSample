using System;
using Core;
using GamePlay.Skill;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils.Attribute;

namespace UI
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField, AutoAssign] private Button _button;
        [SerializeField] private Image _buttonBorder;
        [SerializeField] private SkillType _skillType;
        
        private bool _isSelected;

        private void Start()
        {
            SetSelected(false);
            _button.onClick.AddListener(() => { SetSelected(!_isSelected); });
        }

        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
            _buttonBorder.enabled = isSelected;
            EventDispatcher.OnSkillSelectedEvent((int)_skillType, isSelected);
        }
        
        
    }
}