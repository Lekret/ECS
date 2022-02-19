namespace EcsLib.Api.Invariants
{
    public interface IEcsRemoveInvariant
    {
        bool CanRemove(Entity entity, int componentIndex);
    }
}