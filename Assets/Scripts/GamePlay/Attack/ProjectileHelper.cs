using System;
using UnityEngine;

namespace GamePlay.Attack
{
    public static class ProjectileHelper
    {
        /// <summary>
        /// Balistik hız büyüklüğünü hesaplar (yerçekimi + açı ile).
        /// height: originY - targetY
        /// horiz:  hedefe yatay mesafe (XZ düzlemi)
        /// angleDeg: tepe açısı (derece)
        /// gravity: pozitif skaler (örn: -Physics.gravity.y)
        /// </summary>
        public static double CalculateSpeed(double height, double horiz, double angleDeg, double gravity)
        {
            double angle = angleDeg * Mathf.Deg2Rad;
            // Senin kullandığın formül:
            return (Math.Sqrt(gravity) * horiz * (1 / Math.Cos(angle)))
                   / Math.Sqrt(2 * height + 2 * horiz * Math.Tan(angle));
        }

        /// <summary>
        /// Origin -> Target arası yön vektörünü, verilen açıyla (angleDeg) yukarı kaldırarak döndürür.
        /// (Senin Direction fonksiyonunun genellenmiş hali; Transform yerine pozisyonlar alır.)
        /// </summary>
        public static Vector3 DirectionWithAngle(Vector3 origin, Vector3 target, float angleDeg)
        {
            // Hedefe “düz” XZ yönü:
            Vector3 flatDir = target - new Vector3(origin.x, target.y, origin.z);

            if (flatDir.sqrMagnitude <= float.Epsilon)
                return Vector3.forward; // güvenlik

            // Dönüş ekseni: flatDir'i Y etrafında 270° döndürdüğün eksene karşılık gelen mantık
            Quaternion axisRot = Quaternion.AngleAxis(270f, Vector3.up);
            Vector3 rotateAxis = (axisRot * flatDir).normalized;

            // Açıyı yukarı kaldır
            Quaternion lift = Quaternion.AngleAxis(angleDeg, rotateAxis);
            Vector3 rotated = (lift * flatDir).normalized;

            return rotated;
        }

        /// <summary>
        /// Origin/Target/Angle verildiğinde doğrudan balistik hız vektörünü verir.
        /// </summary>
        public static Vector3 ComputeBallisticVelocity(Vector3 origin, Vector3 target, float angleDeg, float gravityPos)
        {
            float height = origin.y - target.y;
            float horiz = Vector3.Distance(target, new Vector3(origin.x, target.y, origin.z));

            var dir = DirectionWithAngle(origin, target, angleDeg);
            float speed = (float)CalculateSpeed(height, horiz, angleDeg, gravityPos);

            return dir * speed;
        }
    }
}
