using System;
using GamePlay.Attack;
using GamePlay.Skill.Effect;
using BurnEffect = GamePlay.Skill.Effect.BurnEffect;

namespace GamePlay.Skill.Strategy
{
    public interface ISkillStrategy
    {
        SkillType Type { get; }
        void Apply(AttackContext context, bool isRage);
    }

    // Geliştirme örneği bool isRange yerine:
    // // Rage’in anlık “çarpan” durumu
    // public readonly struct RageSnapshot
    // {
    //     public readonly bool Active;
    //     public readonly float MultiShotMul;  // örn 2f
    //     public readonly float BounceMul;     // örn 2f
    //     public readonly float BurnDurationMul;
    //     public readonly float AttackSpeedMul;
    //
    //     public RageSnapshot(bool active, float ms, float bc, float bd, float asMul)
    //     {
    //         Active = active; MultiShotMul = ms; BounceMul = bc; BurnDurationMul = bd; AttackSpeedMul = asMul;
    //     }
    // }
}
