using ECS.Runtime.Access;
using ECS.Runtime.Core;
using ECS.Runtime.System;

namespace ECS.Runtime.Extensions
{
    public class RemoveComponentSystem<T> : IUpdateSystem
    {
        private readonly Filter _filter;

        public RemoveComponentSystem(EcsManager manager)
        {
            _filter = manager.Filter(Mask.With<T>());
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