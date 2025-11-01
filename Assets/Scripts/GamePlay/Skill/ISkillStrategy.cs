using System;
using Data;

namespace GamePlay.Skill
{
    public interface ISkillStrategy
    {
        SkillType Type { get; }
        void Apply(AttackContext ctx, RageSnapshot rage); // Rage bilgisi de verelim
    }

    // Rage’in anlık “çarpan” durumu
    public readonly struct RageSnapshot
    {
        public readonly bool Active;
        public readonly float MultiShotMul;  // örn 2f
        public readonly float BounceMul;     // örn 2f
        public readonly float BurnDurationMul;
        public readonly float AttackSpeedMul;

        public RageSnapshot(bool active, float ms, float bc, float bd, float asMul)
        {
            Active = active; MultiShotMul = ms; BounceMul = bc; BurnDurationMul = bd; AttackSpeedMul = asMul;
        }
    }

    public sealed class MultiShotStrategy : ISkillStrategy
    {
        public SkillType Type => SkillType.MultiShot;
        public void Apply(AttackContext ctx, RageSnapshot rage)
        {
            int baseShots = 2; // “iki ok” kuralı
            if (rage.Active) baseShots *= 2; // Rage: dört ok
            ctx.Shots = Math.Max(ctx.Shots, baseShots);
        }
    }

    public sealed class ReflectiveStrategy : ISkillStrategy
    {
        public SkillType Type => SkillType.ReflectiveShot;
        public void Apply(AttackContext ctx, RageSnapshot rage)
        {
            int baseBounce = 1; // bir ekstra hedef
            if (rage.Active) baseBounce = 2; // Rage: iki ekstra hedef
            ctx.BounceCount = Math.Max(ctx.BounceCount, baseBounce);
            if (!ctx.BulletEffects.Exists(e => e is BounceEffect))
                ctx.BulletEffects.Add(new BounceEffect());
        }
    }

    public sealed class FireShotStrategy : ISkillStrategy
    {
        public SkillType Type => SkillType.FireShot;
        public void Apply(AttackContext ctx, RageSnapshot rage)
        {
            float dur = rage.Active ? 6f : 3f;
            ctx.BurnDuration = Math.Max(ctx.BurnDuration, dur);
            ctx.BurnDps = Math.Max(ctx.BurnDps, ctx.BaseDamage * 0.4f); // örnek oran
            if (!ctx.BulletEffects.Exists(e => e is BurnEffect))
                ctx.BulletEffects.Add(new BurnEffect());
        }
    }

    public sealed class FireRateStrategy : ISkillStrategy
    {
        public SkillType Type => SkillType.FireRate;
        public void Apply(AttackContext ctx, RageSnapshot rage)
        {
            // FireDelay küçülür (hız artışı)
            float mul = rage.Active ? 0.25f : 0.5f; // 4x hız => 0.25 gecikme; 2x hız => 0.5 gecikme
            ctx.FireDelay = Math.Min(ctx.FireDelay, ctx.BaseFireDelay * mul);
        }
    }

    public sealed class RageStrategy : ISkillStrategy
    {
        public SkillType Type => SkillType.Rage;
        public void Apply(AttackContext ctx, RageSnapshot rage)
        {
            // Burada doğrudan bir şey yapmıyoruz; diğerleri RageSnapshot’a göre davranıyor.
        }
    }
}
