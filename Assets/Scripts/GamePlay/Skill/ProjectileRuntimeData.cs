using UnityEngine;

namespace GamePlay.Skill
{
    public sealed class ProjectileRuntimeData
    {
        public int bounceLeft;
        public float burnDuration;
        public float burnDamagePerSecond;
        public float angle;
        public Rigidbody rigidbody;
    }
}