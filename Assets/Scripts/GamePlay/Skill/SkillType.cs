using System;

namespace GamePlay.Skill
{
    [Flags]
    public enum SkillType
    {
        None = 0,
        MultiShot = 1 << 0,
        ReflectiveShot = 1 << 1,
        FireShot = 1 << 2,
        FireRate = 1 << 3,
        Rage = 1 << 4,
    }
}