using System;
using System.Collections;
using System.Collections.Generic;
using SummerBuster.Interfaces;
using UnityEngine;

namespace SummerBuster
{
    public sealed class ObjectPooler : Singleton<ObjectPooler>
    {
        [Serializable]
        public class PoolInfo
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }
        
        public List<PoolInfo> pools;

        private Dictionary<string, Queue<GameObject>> _poolDictionary;
        
        private void Start()
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (var pool in pools)
            {
                var objPool = new Queue<GameObject>();
                for (int i = 0; i < pool.size; i++)
                {
                    var obj = Instantiate(pool.prefab, transform);
                    obj.SetActive(false);

                    objPool.Enqueue(obj);
                }

                _poolDictionary.Add(pool.tag, objPool);
            }
        }

        public static GameObject Spawn(string poolTag, Vector3 position, Quaternion? rotation = null, Transform parent = null)
        {
            return Instance.SpawnFromPool(poolTag, position, rotation, parent);
        }
        
        public static T Spawn<T>(string poolTag, Vector3 position, Quaternion? rotation = null, Transform parent = null)
        {
            return Instance.SpawnFromPool<T>(poolTag, position, rotation);
        }

        public static void Destroy(GameObject poolObject)
        {
            Instance.DestroyFromPool(poolObject);
        }
        
        public static void Destroy(GameObject poolObject, float t)
        {
            Instance.DestroyFromPool(poolObject, t);
        }

        private GameObject SpawnFromPool(string poolTag, Vector3 position, Quaternion? rotation = null, Transform parent = null)
        {
            if (!_poolDictionary.ContainsKey(poolTag))
                return null;

            var objectToSpawn = _poolDictionary[poolTag].Dequeue();

            objectToSpawn.transform.SetParent(parent ? parent : transform);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation ?? objectToSpawn.transform.rotation;
            objectToSpawn.SetActive(true);

            if (objectToSpawn.TryGetComponent<IPooledObject>(out var pooledObject))
            {
                pooledObject.OnSpawn();
            }

            _poolDictionary[poolTag].Enqueue(objectToSpawn);
            return objectToSpawn;
        }

        private T SpawnFromPool<T>(string poolTag, Vector3 position, Quaternion? rotation = null, Transform parent = null)
        {
            return SpawnFromPool(poolTag, position, rotation, parent).GetComponent<T>();
        }
        
        private void DestroyFromPool(GameObject poolObject)
        {
            poolObject.SetActive(false);
            poolObject.transform.SetParent(transform);
        }

        private void DestroyFromPool(GameObject poolObject, float t)
        {
            StartCoroutine(DestroyFromPoolCoroutine(poolObject, t));
        }

        private IEnumerator DestroyFromPoolCoroutine(GameObject poolObject, float t)
        {
            yield return new WaitForSeconds(t);
            DestroyFromPool(poolObject);
        }
    }
}
