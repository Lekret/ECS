using ECS.Runtime.Core;

namespace ECS.Runtime.Extensions.Listeners
{
    public delegate void ComponentChanged<T>(Entity entity);
}