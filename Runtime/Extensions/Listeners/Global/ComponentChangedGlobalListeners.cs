using System.Collections.Generic;

namespace ECS.Runtime.Extensions.Listeners.Global
{
    public struct ComponentChangedGlobalListeners<T>
    {
        public List<ComponentChanged<T>> Value;
    }
}