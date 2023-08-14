using ECS.Runtime.Core;

namespace ECS.Runtime.Extensions.RemoveListeners
{
    public interface IRemovedListener<T>
    {
        void OnRemoved(Entity entity, T value);
    }
}