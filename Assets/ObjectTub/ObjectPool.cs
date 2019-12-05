using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectTub
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private bool DebugMessages = true;

        private static ObjectPool instance;
        private static ObjectPool Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new System.Exception("Object pool singleton has not been initialized. Make sure there is an object pool in the scene");
                }
                return instance;
            }
        }

        [System.Serializable]
        private struct PrewarmObject
        {
            [SerializeField] public GameObject prefab;
            [SerializeField] public int numberOfPreparedInstances;

            public PrewarmObject(int dummyConstructorForCompilerWarning)
            {
                prefab = default;
                numberOfPreparedInstances = 0;
            }
        }
        [SerializeField]
        private PrewarmObject[] prewarmObjects = default;

        private Dictionary<GameObject, List<GameObject>> pool;

        void Awake()
        {
            if (instance != null)
            {
                DebugError("There cannot be more than one object pool. Deleting this one");
                Destroy(this);
                return;
            }
            instance = this;

            pool = new Dictionary<GameObject, List<GameObject>>();
            prewarmThePool();
        }

        private void prewarmThePool()
        {
            foreach (PrewarmObject prewarmObject in prewarmObjects)
            {
                List<GameObject> instances = new List<GameObject>();
                for (int i = 0; i < prewarmObject.numberOfPreparedInstances; i++)
                {
                    GameObject newInstance = instantiateNewInstanceOfObject(prewarmObject.prefab.gameObject);
                    putObjectAway(newInstance);
                    instances.Add(newInstance);
                }
                pool.Add(prewarmObject.prefab.gameObject, instances);
            }
        }

        public static GameObject TakeObjectFromTub(GameObject prefab)
        {
            ObjectPool objectPool = Instance;

            if (objectPool.pool.ContainsKey(prefab) == false)
            {
                // the given object prefab is not being pooled yet, so add the prefab to the dictionary of pooled object
                objectPool.DebugWarning("Object pool adding new prefab to pool: " + prefab.name);
                objectPool.pool.Add(prefab, new List<GameObject>());
            }

            List<GameObject> poolEntry = objectPool.pool[prefab];
            GameObject availableInstance = poolEntry.FirstOrDefault(gameObj => !gameObj.activeInHierarchy);

            if (availableInstance == null)
            {
                // there are no inactive instances of the prefab, so a new one has to be instantiated
                objectPool.DebugWarning("Object pool instantiating a new instance of: " + prefab.name);
                GameObject newInstance = objectPool.instantiateNewInstanceOfObject(prefab);
                poolEntry.Add(newInstance);
                availableInstance = newInstance;
            }

            objectPool.initializeObjectForUse(availableInstance);
            return availableInstance;
        }

        public static void PutObjectBackInTub(GameObject gameObj)
        {
            ObjectPool objectPool = Instance;
            objectPool.putObjectAway(gameObj);
        }

        private GameObject instantiateNewInstanceOfObject(GameObject prefab)
        {
            GameObject newInstance = Instantiate(prefab, transform);
            return newInstance;
        }

        private void initializeObjectForUse(GameObject gameObj)
        {
            gameObj.SetActive(true);

            gameObj.GetComponents<PoolableObject>().ToList()
                .ForEach(poolableObj => poolableObj.InitializeForUse());
        }

        private void putObjectAway(GameObject gameObj)
        {
            gameObj.GetComponents<PoolableObject>().ToList()
                .ForEach(poolableObj => poolableObj.PutAway());

            gameObj.SetActive(false);
        }

        private void DebugWarning(string message)
        {
            if (DebugMessages)
            {
                Debug.LogWarning(message);
            }
        }

        private void DebugError(string message)
        {
            if (DebugMessages)
            {
                Debug.LogError(message);
            }
        }
    }
}
