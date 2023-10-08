using System.Collections.Generic;

namespace ECS.Runtime.Extensions.Listeners.Self
{
    public struct ComponentChangedListeners<T>
    {
        public List<ComponentChanged<T>> Value;
    }
}