using System;
using System.Collections.Generic;
using Core;
using Data;
using UnityEngine;
using UnityUtils.BaseClasses;

namespace GamePlay.Skill
{
    public sealed class SkillManager : SingletonBehavior<SkillManager>
    {
        private readonly Dictionary<SkillType, ISkillStrategy> _all = new();
        private readonly HashSet<SkillType> _active = new();

        protected override void Awake()
        {
            base.Awake();
            // Factory Method
            _all[SkillType.MultiShot] = new MultiShotStrategy();
            _all[SkillType.ReflectiveShot] = new ReflectiveStrategy();
            _all[SkillType.FireShot] = new FireShotStrategy();
            _all[SkillType.FireRate] = new FireRateStrategy();
            _all[SkillType.Rage] = new RageStrategy();

            // Observer: UI eventlerini dinle
            EventDispatcher.OnSkillSelected.AddListener(OnSkillToggle);
        }

        private void OnDestroy()
        {
            EventDispatcher.OnSkillSelected.AddListener(OnSkillToggle);
        }

        private void OnSkillToggle(int skillInt, bool isOn)
        {
            var st = (SkillType)skillInt;
            if (isOn) _active.Add(st);
            else _active.Remove(st);
        }

        public AttackContext CreateContext(float baseDamage, float baseDelay, float baseSpeed, float angle)
        {
            var ctx = new AttackContext
            {
                BaseDamage = baseDamage,
                BaseFireDelay = baseDelay,
                FireDelay = baseDelay,
                BaseSpeed = baseSpeed,
                Angle = angle
            };

            var rage = new RageSnapshot(
                _active.Contains(SkillType.Rage),
                2f, 2f, 2f, 2f // “ikiye katla” olarak düşünülebilir; örnek alanlar
            );

            // Stratejileri deterministik bir sırada uygula
            ApplyIfActive(SkillType.MultiShot, ctx, rage);
            ApplyIfActive(SkillType.ReflectiveShot, ctx, rage);
            ApplyIfActive(SkillType.FireShot, ctx, rage);
            ApplyIfActive(SkillType.FireRate, ctx, rage);
            ApplyIfActive(SkillType.Rage, ctx, rage); // Rage en sonda/no-op

            return ctx;
        }

        private void ApplyIfActive(SkillType type, AttackContext ctx, RageSnapshot rage)
        {
            if (_active.Contains(type))
                _all[type].Apply(ctx, rage);
        }
    }
}