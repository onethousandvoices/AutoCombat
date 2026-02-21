using System.Collections.Generic;
using UnityEngine;

namespace AutoCombat.Core
{
    public sealed class ComponentPool<T> where T : Component, IPoolable
    {
        private readonly Stack<T> _stack = new();
        private readonly T _prefab;
        private readonly Transform _parent;

        public ComponentPool(T prefab, int preWarm = 0)
        {
            _prefab = prefab;
            _parent = new GameObject($"Pool<{typeof(T).Name}>").transform;

            for (var i = 0; i < preWarm; i++)
            {
                var instance = Object.Instantiate(_prefab, _parent);
                instance.gameObject.SetActive(false);
                _stack.Push(instance);
            }
        }

        public T Rent()
        {
            var instance = _stack.Count > 0
                ? _stack.Pop()
                : Object.Instantiate(_prefab, _parent);

            instance.gameObject.SetActive(true);
            instance.OnSpawn();
            return instance;
        }

        public void Return(T instance)
        {
            instance.OnRecycle();
            instance.gameObject.SetActive(false);
            _stack.Push(instance);
        }
    }
}
