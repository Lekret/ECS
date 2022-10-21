namespace Lekret.Ecs.Extensions
{
    public interface ISetListener<T>
    {
        void OnSet(Entity entity, T value);
    }
}