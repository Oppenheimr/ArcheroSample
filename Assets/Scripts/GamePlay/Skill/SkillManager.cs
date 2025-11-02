using System;
using System.Collections.Generic;
using Core;
using GamePlay.Attack;
using GamePlay.Skill.Strategy;

namespace GamePlay.Skill
{
    public static class SkillManager
    {
        private static readonly Dictionary<SkillType, ISkillStrategy> _all = new();
        private static SkillType _active = SkillType.None;
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            _all[SkillType.MultiShot] = new MultiShotStrategy();
            _all[SkillType.ReflectiveShot] = new ReflectiveStrategy();
            _all[SkillType.FireShot] = new FireShotStrategy();
            _all[SkillType.FireRate] = new FireRateStrategy();
            _all[SkillType.Rage] = new RageStrategy();
            EventDispatcher.OnSkillSelected.AddListener(Toggle);
        }

        public static void Dispose()
        {
            if (!_initialized) return;
            _all.Clear();
            _active = SkillType.None;
            _initialized = false;
        }

        public static void Toggle(int skillId, bool active)
        {
            var skillType = (SkillType)skillId;
            if (active) _active |= skillType;
            else _active &= ~skillType;
        }
        
        private static bool Has(SkillType st) => (_active & st) != 0;

        public static AttackContext CreateContext(float baseDamage, float baseDelay, float angle)
        {
            var ctx = new AttackContext
            {
                baseDamage = baseDamage,
                baseFireDelay = baseDelay,
                fireDelay = baseDelay,
                angle = angle,
            };

            var rage = Has(SkillType.Rage);
            // Deterministik sırada uygula:
            ApplyIfActive(SkillType.MultiShot, ctx, rage);
            ApplyIfActive(SkillType.ReflectiveShot, ctx, rage);
            ApplyIfActive(SkillType.FireShot, ctx, rage);
            ApplyIfActive(SkillType.FireRate, ctx, rage);
            ApplyIfActive(SkillType.Rage, ctx, rage);

            return ctx;
        }

        private static void ApplyIfActive(SkillType type, AttackContext context, bool isRage)
        {
            if ((_active & type) == 0) return;
            if (_all.TryGetValue(type, out var strategy))
                strategy.Apply(context, isRage);
        }
    }
}