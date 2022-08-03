using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common
{
    public class Pool<T> where T : MonoBehaviour, IPoolable
    {
        private readonly T _prefab;
        private readonly Queue<T> _queue;
        private readonly List<T> _createdObjects;

        public Pool(T prefab)
        {
            _prefab = prefab;
            _queue = new Queue<T>();
            _createdObjects = new List<T>();
        }

        public T Instantiate()
        {
            T instance;
            if (_queue.Count == 0)
            {
                instance = Object.Instantiate(_prefab);
                _createdObjects.Add(instance);
            }
            else
            {
                instance = _queue.Dequeue();
            }

            instance.Spawn();
            return instance;
        }

        public void Destroy(T gameObject)
        {
            gameObject.DeSpawn();
            _queue.Enqueue(gameObject);
        }

        public void Clear()
        {
            foreach (var createdObject in _createdObjects)
            {
                Object.Destroy(createdObject.gameObject);
            }

            _createdObjects.Clear();
            _queue.Clear();
        }
    }
}