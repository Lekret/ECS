using ECS.Runtime.Core;
using ECS.Runtime.Extensions.Listeners;
using ECS.Runtime.Extensions.Listeners.Global;
using ECS.Runtime.Extensions.Listeners.Self;
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

        public new EcsSystemsExtended Add(ISystem system)
        {
            return (EcsSystemsExtended) base.Add(system);
        }
        
        public EcsSystemsExtended NotifyChanged<T>()
        {
            Add(new ComponentChangedReactiveSystem<T>(_world));
            return this;
        }

        public EcsSystemsExtended NotifyChangedGlobal<T>()
        {
            Add(new ComponentChangedGlobalReactiveSystem<T>(_world));
            return this;
        }

        public EcsSystemsExtended Remove<T>()
        {
            Add(new RemoveComponentSystem<T>(_world));
            return this;
        }
    }
}