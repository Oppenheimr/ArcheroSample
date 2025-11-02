using System;
using GamePlay.Attack;
using GamePlay.Skill.Effect;

namespace GamePlay.Skill.Strategy
{
    public sealed class FireShotStrategy : ISkillStrategy
    {
        public SkillType Type => SkillType.FireShot;
        public void Apply(AttackContext context, bool isRage)
        {
            var dur = isRage ? 6f : 3f;
            context.burnDuration = Math.Max(context.burnDuration, dur);
            context.burnDamagePerSecond = Math.Max(context.burnDamagePerSecond, context.baseDamage * 0.4f);
            if (!context.projectileEffects.Exists(e => e is BurnEffect))
                context.projectileEffects.Add(new BurnEffect());
        }
    }
}