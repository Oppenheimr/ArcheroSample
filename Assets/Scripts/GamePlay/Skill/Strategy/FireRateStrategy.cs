using System;
using GamePlay.Attack;

namespace GamePlay.Skill.Strategy
{
    public sealed class FireRateStrategy : ISkillStrategy
    {
        public SkillType Type => SkillType.FireRate;
        public void Apply(AttackContext context, bool isRage)
        {
            // FireDelay küçülür (hız artışı)
            float mul = isRage ? 0.25f : 0.5f; // 4x hız => 0.25 gecikme; 2x hız => 0.5 gecikme
            context.fireDelay = Math.Min(context.fireDelay, context.baseFireDelay * mul);
        }
    }
}