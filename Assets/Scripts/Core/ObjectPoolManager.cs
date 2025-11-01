using Data;
using UnityEngine;
using UnityUtils.BaseClasses;

namespace Core
{
    public class ObjectPoolManager : SingletonBehavior<ObjectPoolManager>
    {
        #region Key Fields
        private int _bulletPoolKey;
        #endregion
        
        private void OnEnable()
        {
            if (InstanceIsAvailable)
            {
                Destroy(gameObject);
                return;
            }
            
            SetInstance(this);
            ObjectPooler.PoolsParent = gameObject;
            
            //Creations...

            #region Pool Create
            _bulletPoolKey = ObjectPooler.CreatePool(GameData.Instance.defaultBullet, 50);
            #endregion
        }

        #region Pool Getters
        
        public static Component GetBulletPool(Vector3 position, Quaternion rotation)
        {
            var bullet = ObjectPooler.GetPoolObject(Instance._bulletPoolKey);
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            return bullet;
        }
        
        public static void PutPoolBullet(Component reference) => 
            ObjectPooler.PutPoolObject(Instance._bulletPoolKey, reference);
        
        #endregion
        
        private void OnDisable()
        {
            ObjectPooler.Reset();
        }
        
        public static void Initialize(Transform parent)
        {
            if (InstanceIsAvailable)
                return;
            
            // Create an empty GameObject
            var gameObject = new GameObject("Object Pool Manager");
            // Add this Component
            var result = gameObject.AddComponent<ObjectPoolManager>();
            
            Instance.SetInstance(result);
            gameObject.transform.SetParent(parent);
        }
    }
}