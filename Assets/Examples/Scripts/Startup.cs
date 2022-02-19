using EcsLib.Api;
using EcsLib.Api.Invariants.Impl;
using UnityEngine;

namespace Examples.Scripts
{
    public struct Item
    {
        
    }

    public struct Count
    {
        
    }
    
    public class Startup : MonoBehaviour
    {
        private EcsSystems _systems;

        private void Awake()
        {
            _systems = new EcsSystems();
            var manager = new EcsManager();
            manager.Set<ILogger>(new Logger());
            _systems.Add(new TestSystem(manager));
            
            manager.Invariant(new AlwaysTogether()
                .Inc<Item>()
                .Inc<Count>());

            var entity = Entity.Create();
            entity.Set(new Item());
            entity.Set(new Count());
            entity.Remove<Count>();
            Debug.LogError(entity.Has<int>());
        }

        private void Start()
        {
            _systems.Init();
        }

        private void Update()
        {
            _systems.Tick();
        }

        private void FixedUpdate()
        {
            _systems.FixedTick();
        }

        private void OnDestroy()
        {
            _systems.Destroy();
        }
    }
}