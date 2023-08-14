using ECS.Runtime.Core;
using ECS.Runtime.Extensions.RemoveListeners;
using ECS.Runtime.Extensions.SetListeners;
using ECS.Runtime.System;

namespace ECS.Runtime.Extensions
{
    public class EcsSystemsExtended : EcsSystems
    {
        private readonly EcsManager _manager;

        public EcsSystemsExtended(EcsManager manager)
        {
            _manager = manager;
        }

        public EcsSystemsExtended NotifySet<T>(EventTarget eventTarget)
        {
            switch (eventTarget)
            {
                case EventTarget.Self:
                    Add(new SelfSetEventSystem<T>(_manager));
                    break;
                case EventTarget.Any:
                    Add(new AnySetEventSystem<T>(_manager));
                    break;
            }

            return this;
        }

        public EcsSystemsExtended NotifyRemoved<T>(EventTarget eventTarget)
        {
            switch (eventTarget)
            {
                case EventTarget.Self:
                    Add(new SelfRemovedEventSystem<T>(_manager));
                    break;
                case EventTarget.Any:
                    Add(new AnyRemovedEventSystem<T>(_manager));
                    break;
            }

            return this;
        }

        public EcsSystemsExtended Remove<T>()
        {
            Add(new RemoveComponentSystem<T>(_manager));
            return this;
        }
    }
}