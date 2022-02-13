using EcsLib.Api;
using UnityEngine;

namespace Example
{
    public class Startup : MonoBehaviour
    {
        private EcsSystems _systems;

        private void Awake()
        {
            _systems = new EcsSystems();
            var entityManager = new EcsManager();
            entityManager.Set<ILogger>(new Logger());
            _systems.Add(new TestSystem(entityManager));
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