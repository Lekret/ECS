using ECS.Runtime.Access;
using ECS.Runtime.Core;
using ECS.Runtime.System;

namespace ECS.Runtime.Extensions
{
    public class RemoveComponentSystem<T> : IUpdateSystem
    {
        private readonly Filter _filter;

        public RemoveComponentSystem(World world)
        {
            _filter = world.Filter(Mask.With<T>());
        }

        public void Update()
        {
            foreach (var entity in _filter)
            {
                entity.Remove<T>();
            }
        }
    }
}