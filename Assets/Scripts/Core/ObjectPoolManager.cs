using Data;
using UnityEngine;
using UnityUtils.BaseClasses;

namespace Core
{
    public class ObjectPoolManager : SingletonBehavior<ObjectPoolManager>
    {
        #region Key Fields
        private int _examplePoolKey;
        private int _bulletPoolKey;
        private int _enemyBulletPoolKey;
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
            _examplePoolKey = ObjectPooler.CreatePool(GameData.Instance.exampleData);
            _bulletPoolKey = ObjectPooler.CreatePool(GameData.Instance.defaultBullet);
            #endregion
        }

        #region Pool Getters
        
        public static Transform GetExample()
        {
            var exampleComponent = ObjectPooler.GetPoolObject(Instance._examplePoolKey);
            return (Transform)exampleComponent;
        }
        
        public static void PutPoolExample(Transform reference) => 
            ObjectPooler.PutPoolObject(Instance._examplePoolKey, reference);
        
        public static Component GetBulletPool() => ObjectPooler.GetPoolObject(Instance._enemyBulletPoolKey);
        
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