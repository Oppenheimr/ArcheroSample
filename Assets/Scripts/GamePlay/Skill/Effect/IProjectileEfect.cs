using GamePlay.Enemy;

namespace GamePlay.Skill.Effect
{
    public interface IProjectileEffect
    {
        void OnHit(EnemyController enemy, ProjectileRuntimeData data);
    }
}