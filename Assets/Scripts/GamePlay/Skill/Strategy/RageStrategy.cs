using GamePlay.Attack;

namespace GamePlay.Skill.Strategy
{
    public sealed class RageStrategy : ISkillStrategy
    {
        public SkillType Type => SkillType.Rage;
        public void Apply(AttackContext context, bool isRage)
        {
            // Burada doğrudan bir şey yapmıyoruz; diğerleri RageSnapshot’a göre davranıyor.
        }
    }
}