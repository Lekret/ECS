using System.Collections.Generic;
using UnityEngine;

namespace Lekret.Ecs.Editor
{
    public class EcsDebugger : MonoBehaviour
    {
        private static EcsDebugger Instance;
        private readonly Dictionary<Entity, EcsEntityDebugView> _entityViews = new();
        private readonly List<Entity> _entitiesBuffer = new();
        private readonly Queue<EcsEntityDebugView> _viewsToDelete = new();
        private Transform _transform;
        private EcsManager _manager;

        public EcsManager Manager => _manager;

        public static void Setup(EcsManager manager)
        {
            if (Instance)
            {
                Debug.LogWarning("Ecs Debugger is already created");
                return;
            }

            var go = new GameObject("[Ecs Debugger]");
            var debugger = go.AddComponent<EcsDebugger>();
            debugger._manager = manager;
        }

        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            _transform = transform;
            DontDestroyOnLoad(gameObject);
        }

        private void LateUpdate()
        {
            Manager.GetEntities(_entitiesBuffer);
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
                var entityDebugView = entityView.AddComponent<EcsEntityDebugView>();
                entityDebugView.Entity = entity;
                _entityViews.Add(entity, entityDebugView);
            }
        }
    }
}