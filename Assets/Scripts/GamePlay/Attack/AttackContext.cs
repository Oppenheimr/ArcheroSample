using System.Collections.Generic;
using GamePlay.Skill;
using GamePlay.Skill.Effect;

namespace GamePlay.Attack
{
    public sealed class AttackContext
    {
        // Silah taban değerleri
        public float baseDamage;
        public float baseFireDelay;
        public float angle;

        // Stratejiler tarafından ayarlananlar
        public int shots = 1;
        public int bounceCount = 0;
        public float burnDuration = 0f;
        public float burnDamagePerSecond = 0f;
        public float fireDelay;
        public readonly List<IProjectileEffect> projectileEffects = new();
    }
}