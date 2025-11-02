using System;
using GamePlay.Attack;
using GamePlay.Skill.Effect;

namespace GamePlay.Skill.Strategy
{
    public sealed class ReflectiveStrategy : ISkillStrategy
    {
        public SkillType Type => SkillType.ReflectiveShot;
        public void Apply(AttackContext context, bool isRage)
        {
            var baseBounce = 1; // bir ekstra hedef
            if (isRage) baseBounce = 2; // Rage: iki ekstra hedef
            context.bounceCount = Math.Max(context.bounceCount, baseBounce);
            if (!context.projectileEffects.Exists(e => e is ReflectEffect))
                context.projectileEffects.Add(new ReflectEffect());
        }
    }
}