using System.Collections.Generic;
using ECS.Runtime.Core;

namespace ECS.Runtime.Extensions.SetListeners
{
    public delegate void ComponentSet<T>(Entity entity, T component);
    
    public struct SetListeners<T>
    {
        public List<ComponentSet<T>> Value;
    }
}