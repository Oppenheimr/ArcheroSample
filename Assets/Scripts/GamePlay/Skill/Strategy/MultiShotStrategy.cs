using System;
using GamePlay.Attack;

namespace GamePlay.Skill.Strategy
{
    public sealed class MultiShotStrategy : ISkillStrategy
    {
        public SkillType Type => SkillType.MultiShot;
        public void Apply(AttackContext context, bool isRage)
        {
            var baseShots = 2; // “iki ok” kuralı
            if (isRage) baseShots *= 2; // Rage: dört ok
            context.shots = Math.Max(context.shots, baseShots);
        }
    }
}