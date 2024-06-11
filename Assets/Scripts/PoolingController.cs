using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Scripts
{
    [Serializable]
    public class PoolingData
    {
        public MonoBehaviour prefab;
        public int defaultCapacity;
        public int maxSize;
    }

    public class PoolingController : MonoBehaviour
    {
        [SerializeField] PoolingData[] poolingDates;
        private readonly Dictionary<Type, ObjectPool<MonoBehaviour>> poolItems = new();

        private void Awake()
        {
            foreach (PoolingData pooling in poolingDates)
                CreatePooling(pooling.prefab, pooling.defaultCapacity, pooling.maxSize);
        }
        private void OnEnable()
        {
            Bullet.OnRelease += Bullet_OnRelease;
            Cube.OnRelease += Cube_OnRelease;
        }

        private void OnDisable()
        {
            Bullet.OnRelease -= Bullet_OnRelease;
            Cube.OnRelease -= Cube_OnRelease;
        }

        private void Cube_OnRelease(Cube cube)
        {
            Release(cube);
        }

        private void Bullet_OnRelease(Bullet bullet)
        {
            Release(bullet);
        }


        public T Get<T>() where T : MonoBehaviour
        {
            if (poolItems.TryGetValue(typeof(T), out ObjectPool<MonoBehaviour> pool))
                return (T)pool.Get();

            return null;
        }

        public void Release<T>(T poolItem) where T : MonoBehaviour
        {
            Type type = typeof(T);
            if (poolItems.TryGetValue(type, out ObjectPool<MonoBehaviour> item))
                item.Release(poolItem);
        }

        void CreatePooling<T>(T prefab, int defaultCapacity, int maxSize) where T : MonoBehaviour
        {
            Transform poolTransform = CreatePoolPatent(prefab.name + "Pool");

            ObjectPool<MonoBehaviour> pool = new ObjectPool<MonoBehaviour>(() => Instantiate(prefab, poolTransform),
                t => t.gameObject.SetActive(true), t => t.gameObject.SetActive(false), t => Destroy(t.gameObject),
                false, defaultCapacity, maxSize);

            poolItems.Add(prefab.GetType(), pool);
        }

        private Transform CreatePoolPatent(string poolName)
        {
            Transform poolTransform = new GameObject(poolName).transform;
            poolTransform.SetParent(transform);
            return poolTransform;
        }
    }
}