using System;
using UnityEngine;

namespace GamePlay.Attack
{
    public static class ProjectileHelper
    {
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
        
        /// <summary>
        /// Balistik hız büyüklüğünü hesaplar (yerçekimi + açı ile).
        /// height: originY - targetY
        /// horizontalDistance:  hedefe yatay mesafe (XZ düzlemi)
        /// angleDeg: tepe açısı (derece)
        /// gravity: pozitif skaler (örn: -Physics.gravity.y)
        /// </summary>
        private static double CalculateSpeed(double height, double horizontalDistance, double angleDeg, double gravity)
        {
            double angle = angleDeg * Mathf.Deg2Rad;
            return (Math.Sqrt(gravity) * horizontalDistance * (1 / Math.Cos(angle)))
                   / Math.Sqrt(2 * height + 2 * horizontalDistance * Math.Tan(angle));
        }

        /// <summary>
        /// Origin -> Target arası yön vektörünü, verilen açıyla (angleDeg) yukarı kaldırarak döndürür.
        /// </summary>
        private static Vector3 DirectionWithAngle(Vector3 origin, Vector3 target, float angleDeg)
        {
            // Hedefe “düz” XZ yönü:
            var direction = target - new Vector3(origin.x, target.y, origin.z);
            if (direction.sqrMagnitude <= float.Epsilon)
                return Vector3.forward; // güvenlik
            
            var rotateAxis = (Quaternion.AngleAxis(270f, Vector3.up) * direction).normalized;

            // Açıyı yukarı kaldır
            Quaternion lift = Quaternion.AngleAxis(angleDeg, rotateAxis);
            Vector3 rotated = (lift * direction).normalized;

            return rotated;
        }
    }
}
