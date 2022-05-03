using System.Collections.Generic;

namespace ECSimplicity
{
    public interface IEcsSystem { }
    public interface IEcsInitSystem : IEcsSystem { void Init(); }
    public interface IEcsTickSystem : IEcsSystem { void Tick(); }
    public interface IEcsFixedTickSystem : IEcsSystem { void FixedTick(); }
    public interface IEcsCleanupSystem : IEcsSystem { void Cleanup(); }
    public interface IEcsDestroySystem : IEcsSystem { void Destroy(); }

    public class EcsSystems : IEcsInitSystem, IEcsTickSystem, IEcsFixedTickSystem, IEcsCleanupSystem, IEcsDestroySystem
    {
        private readonly List<IEcsInitSystem> _initSystems = new List<IEcsInitSystem>();
        private readonly List<IEcsTickSystem> _tickSystems = new List<IEcsTickSystem>();
        private readonly List<IEcsFixedTickSystem> _fixedTickSystems = new List<IEcsFixedTickSystem>();
        private readonly List<IEcsCleanupSystem> _cleanupSystems = new List<IEcsCleanupSystem>();
        private readonly List<IEcsDestroySystem> _destroySystems = new List<IEcsDestroySystem>();

        public EcsSystems Add(IEcsSystem system)
        {
            if (system is IEcsInitSystem initSystem)
                _initSystems.Add(initSystem);
            if (system is IEcsTickSystem tickSystem)
                _tickSystems.Add(tickSystem);
            if (system is IEcsFixedTickSystem fixedTickSystem)
                _fixedTickSystems.Add(fixedTickSystem);
            if (system is IEcsCleanupSystem cleanupSystem)
                _cleanupSystems.Add(cleanupSystem);
            if (system is IEcsDestroySystem destroySystem) 
                _destroySystems.Add(destroySystem);
            return this;
        }

        public void Init()
        {
            for (var i = 0; i < _initSystems.Count; i++)
            {
                _initSystems[i].Init();
            }
        }
        
        public void Tick()
        {
            for (var i = 0; i < _tickSystems.Count; i++)
            {
                _tickSystems[i].Tick();
            }
        }
        
        public void FixedTick()
        {
            for (var i = 0; i < _fixedTickSystems.Count; i++)
            {
                _fixedTickSystems[i].FixedTick();
            }
        }
        
        public void Cleanup()
        {
            for (var i = 0; i < _cleanupSystems.Count; i++)
            {
                _cleanupSystems[i].Cleanup();
            }
        }

        public void Destroy()
        {
            for (var i = 0; i < _destroySystems.Count; i++)
            {
                _destroySystems[i].Destroy();
            }
        }
    }
}