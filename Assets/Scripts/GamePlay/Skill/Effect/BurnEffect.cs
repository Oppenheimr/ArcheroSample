using GamePlay.Enemy;

namespace GamePlay.Skill.Effect
{
    public sealed class BurnEffect : IProjectileEffect
    {
        public void OnHit(EnemyController enemy, ProjectileRuntimeData data)
        {
            if (data.burnDuration <= 0f) return;
            //hedefte bir BurnComponent olsun, stacklenebilir
            var burn = enemy.GetComponent<Attack.BurnEffectComponent>();
            if (!burn) burn = enemy.gameObject.AddComponent<Attack.BurnEffectComponent>();
            // stacking içeride ele alınır
            burn.Apply(data.burnDamagePerSecond, data.burnDuration); 
        }
    }
}