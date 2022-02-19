using System;
using System.Collections.Generic;
using System.Reflection;
using EcsLib.Api;

namespace EcsLib.Internal
{
    internal class EcsSingletons
    {
        private readonly Dictionary<Type, object> _typeToInstance = new Dictionary<Type, object>();

        internal EcsSingletons()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsDefined(typeof(EcsSingletonAttribute)))
                {
                    _typeToInstance.Add(type, null);
                }
            }
        }
        
        internal T Get<T>() where T : class
        {
            if (CanHandleType(typeof(T)))
            {
                _typeToInstance.TryGetValue(typeof(T), out var singleton);
                if (singleton == null)
                    EcsError.Handle($"[{nameof(Get)}<{typeof(T)}>] Singleton is null");
                return (T) singleton;
            }
            EcsError.Handle($"[{nameof(Get)}<{typeof(T)}>] Isn't singleton");
            return default;
        }

        internal void Set<T>(T value) where T : class
        {
            if (CanHandleType(typeof(T)))
            {
                _typeToInstance[typeof(T)] = value;
            }
            else
            {
                EcsError.Handle($"[{nameof(Set)}<{typeof(T)}>] Isn't singleton");
            }
        }

        private bool CanHandleType(Type type)
        {
            return _typeToInstance.ContainsKey(type);
        }
        
        internal void EnsureInitialized()
        {
            foreach (var singleton in _typeToInstance)
            {
                if (singleton.Value == null)
                {
                    EcsError.Handle($"Singleton of type {singleton.Key} is null");
                }
            }
        }
    }
}