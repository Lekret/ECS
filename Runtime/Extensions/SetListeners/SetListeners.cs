using System.Collections.Generic;

namespace ECS.Runtime.Extensions.SetListeners
{
    public struct SetListeners<T>
    {
        public List<ISetListener<T>> Value;
    }
}