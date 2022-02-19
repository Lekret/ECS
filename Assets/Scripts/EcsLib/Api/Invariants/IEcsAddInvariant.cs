namespace EcsLib.Api.Invariants
{
    public interface IEcsAddInvariant
    {
        bool CanAdd(Entity entity, int componentIndex);
    }
}