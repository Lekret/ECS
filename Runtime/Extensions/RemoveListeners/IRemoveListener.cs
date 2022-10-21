namespace Lekret.Ecs.Extensions
{
    public interface IRemoveListener<T>
    {
        void OnRemove(Entity entity, T value);
    }
}