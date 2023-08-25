using ECS.Runtime.Core;
using ECS.Runtime.Extensions.RemoveListeners;
using ECS.Runtime.Extensions.SetListeners;
using ECS.Runtime.Systems;

namespace ECS.Runtime.Extensions
{
    public class EcsSystemsExtended : EcsSystems
    {
        private readonly World _world;

        public EcsSystemsExtended(World world)
        {
            _world = world;
        }

        public EcsSystemsExtended NotifySet<T>(EventTarget eventTarget)
        {
            switch (eventTarget)
            {
                case EventTarget.Self:
                    Add(new SelfSetEventSystem<T>(_world));
                    break;
                case EventTarget.Any:
                    Add(new AnySetEventSystem<T>(_world));
                    break;
            }

            return this;
        }

        public EcsSystemsExtended NotifyRemoved<T>(EventTarget eventTarget)
        {
            switch (eventTarget)
            {
                case EventTarget.Self:
                    Add(new SelfRemovedEventSystem<T>(_world));
                    break;
                case EventTarget.Any:
                    Add(new AnyRemovedEventSystem<T>(_world));
                    break;
            }

            return this;
        }

        public EcsSystemsExtended Remove<T>()
        {
            Add(new RemoveComponentSystem<T>(_world));
            return this;
        }
    }
}