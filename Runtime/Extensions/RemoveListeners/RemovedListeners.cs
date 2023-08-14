using System.Collections.Generic;

namespace ECS.Runtime.Extensions.RemoveListeners
{
    public class RemovedListeners<T>
    {
        public List<IRemovedListener<T>> Value;
    }
}