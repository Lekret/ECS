using ECSimplicity;
using UnityEngine;

namespace Examples.Scripts
{
    public class Startup : MonoBehaviour
    {
        private EcsSystems _systems;

        private void Awake()
        {
            var manager = new EcsAdmin();
            _systems = new EcsSystems();
            _systems.Add(new TestSystem(manager));
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