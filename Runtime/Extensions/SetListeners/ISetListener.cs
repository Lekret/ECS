using ECS.Runtime.Core;

namespace ECS.Runtime.Extensions.SetListeners
{
    public interface ISetListener<T>
    {
        void OnSet(Entity entity, T value);
    }
}