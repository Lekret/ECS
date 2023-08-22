using System.Collections.Generic;
using ECS.Runtime.Core;
using UnityEngine;

namespace ECS.Editor
{
    public class EcsDebugger : MonoBehaviour
    {
        private readonly Dictionary<Entity, EntityDebugView> _entityViews = new();
        private readonly List<Entity> _entitiesBuffer = new();
        private readonly Queue<EntityDebugView> _viewsToDelete = new();
        private Transform _transform;
        private World _world;

        public World World => _world;

        public static void Create(
            World world,
            bool allowCopies = false,
            bool dontDestroyOnLoad = false)
        {
            if (!allowCopies && FindObjectOfType<EcsDebugger>())
                return;

            var debuggerObject = new GameObject("[Ecs Debugger]");
            var debugger = debuggerObject.AddComponent<EcsDebugger>();
            debugger._world = world;
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(debuggerObject);
        }

        private void Awake()
        {
            _transform = transform;
        }

        private void LateUpdate()
        {
            _world.GetEntities(_entitiesBuffer);
            DeleteViewsForDestroyedEntity();
            EnsureViewsForExistingEntity();
        }

        private void DeleteViewsForDestroyedEntity()
        {
            foreach (var (entity, view) in _entityViews)
            {
                if (!_entitiesBuffer.Contains(entity))
                    _viewsToDelete.Enqueue(view);
            }

            while (_viewsToDelete.TryDequeue(out var view))
            {
                _entityViews.Remove(view.Entity);
                DestroyImmediate(view.gameObject);
            }
        }

        private void EnsureViewsForExistingEntity()
        {
            foreach (var entity in _entitiesBuffer)
            {
                if (_entityViews.ContainsKey(entity))
                    continue;

                var entityView = new GameObject(entity.ToString());
                entityView.transform.SetParent(_transform);
                var entityDebugView = entityView.AddComponent<EntityDebugView>();
                entityDebugView.Entity = entity;
                _entityViews.Add(entity, entityDebugView);
            }
        }
    }
}