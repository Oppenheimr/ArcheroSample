using System.Collections.Generic;
using GamePlay.Skill;

namespace Data
{
    public sealed class AttackContext
    {
        // Silah taban değerleri
        public float BaseDamage;
        public float BaseFireDelay;   // saniye
        public float Angle;           // tepe açısı
        public float BaseSpeed;

        // Stratejiler tarafından ayarlananlar
        public int Shots = 1;                      // kaç mermi
        public int BounceCount = 0;                // sekme sayısı
        public float BurnDuration = 0f;            // DOT süresi
        public float BurnDps = 0f;                 // saniyelik hasar
        public float FireDelay;                    // uygulanmış atış gecikmesi
        public readonly List<IBulletEffect> BulletEffects = new();
    }
}