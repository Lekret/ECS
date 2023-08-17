using System.Collections.Generic;
using ECS.Runtime.Core;

namespace ECS.Runtime.Extensions.RemoveListeners
{
    public delegate void ComponentRemoved<T>(Entity entity);
    
    public class RemovedListeners<T>
    {
        public List<ComponentRemoved<T>> Value;
    }
}