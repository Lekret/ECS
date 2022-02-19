using System.Collections.Generic;
using EcsLib.Api;
using EcsLib.Api.Invariants;

namespace EcsLib.Internal
{
    internal class EcsInvariance
    {
        private readonly List<IEcsAddInvariant> _addInvariants = new List<IEcsAddInvariant>();
        private readonly List<IEcsRemoveInvariant> _removeInvariants = new List<IEcsRemoveInvariant>();
        
        internal void AddInvariant(IEcsAddInvariant invariant)
        {
            _addInvariants.Add(invariant);
        }
        
        internal void AddInvariant(IEcsRemoveInvariant invariant)
        {
            _removeInvariants.Add(invariant);
        }

        internal bool CanAddComponent(Entity entity, int componentIndex)
        {
            foreach (var inv in _addInvariants)
            {
                if (!inv.CanAdd(entity, componentIndex))
                    return false;
            }
            return true;
        }
        
        internal bool CanRemoveComponent(Entity entity, int componentIndex)
        {
            foreach (var inv in _removeInvariants)
            {
                if (!inv.CanRemove(entity, componentIndex))
                    return false;
            }
            return true;
        }
    }
}