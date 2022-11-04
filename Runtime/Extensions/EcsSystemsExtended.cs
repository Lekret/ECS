using Lekret.Ecs;
using Lekret.Ecs.Extensions;

namespace SimpleEcs.Runtime
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

        public EcsSystemsExtended NotifyRemove<T>(EventTarget eventTarget)
        {
            switch (eventTarget)
            {
                case EventTarget.Self:
                    Add(new SelfRemoveEventSystem<T>(_manager));
                    break;
                case EventTarget.Any:
                    Add(new AnyRemoveEventSystem<T>(_manager));
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