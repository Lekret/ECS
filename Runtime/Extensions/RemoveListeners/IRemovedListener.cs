namespace Lekret.Ecs.Extensions
{
    public interface IRemovedListener<T>
    {
        void OnRemoved(Entity entity, T value);
    }
}