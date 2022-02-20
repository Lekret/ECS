using EcsLib.Api;
using UnityEngine;

namespace Examples.Scripts
{
    public struct Item { }
    public struct Count { }
    public struct ItemType { }
    
    public class Startup : MonoBehaviour
    {
        private EcsSystems _systems;

        private void Awake()
        {
            _systems = new EcsSystems();
            var manager = new EcsManager();
            manager.Set<ILogger>(new Logger());
            _systems.Add(new TestSystem(manager));
            manager.EnsureSingletonsInitialized();

            var entity = Entity.Create();
            entity.Set(new Item());
            entity.Set(new Count());
            entity.Remove<Count>();
            entity.Remove<Item>();

            var fi = manager.Filter().Inc<int>();
            fi.End();
            fi.End();
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