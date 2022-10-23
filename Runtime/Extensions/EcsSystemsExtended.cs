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

        public EcsSystemsExtended NotifyOnSet<T>()
        {
            Add(new SetSelfEventSystem<T>(_manager));
            return this;
        }

        public EcsSystemsExtended NotifyAllOnSet<T>()
        {
            Add(new SetAllEventSystem<T>(_manager));
            return this;
        }
        
        public EcsSystemsExtended NotifyOnRemove<T>()
        {
            Add(new RemoveSelfEventSystem<T>(_manager));
            return this;
        }
        
        public EcsSystemsExtended NotifyAllOnRemove<T>()
        {
            Add(new RemoveAllEventSystem<T>(_manager));
            return this;
        }

        public EcsSystemsExtended Remove<T>()
        {
            Add(new RemoveComponentSystem<T>(_manager));
            return this;
        }
    }
}