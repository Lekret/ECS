namespace Lekret.Ecs.Extensions
{
    public interface IRemoveListener<T>
    {
        void OnRemoved(Entity entity, T value);
    }
}