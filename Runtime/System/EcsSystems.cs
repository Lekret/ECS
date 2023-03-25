using System.Collections.Generic;

namespace Lekret.Ecs
{
    public class EcsSystems :
        IInitSystem,
        IUpdateSystem,
        IFixedUpdateSystem,
        ILateUpdateSystem,
        ITerminateSystem
    {
        protected List<IInitSystem> InitSystems = new List<IInitSystem>();
        protected List<IUpdateSystem> UpdateSystem = new List<IUpdateSystem>();
        protected List<IFixedUpdateSystem> FixedUpdateSystems = new List<IFixedUpdateSystem>();
        protected List<ILateUpdateSystem> LateUpdateSystems = new List<ILateUpdateSystem>();
        protected List<ITerminateSystem> TerminateSystems = new List<ITerminateSystem>();

        public virtual EcsSystems Add(ISystem system)
        {
            if (system is IInitSystem initSystem)
                InitSystems.Add(initSystem);

            if (system is IFixedUpdateSystem fixedUpdateSystem)
                FixedUpdateSystems.Add(fixedUpdateSystem);

            if (system is IUpdateSystem updateSystem)
                UpdateSystem.Add(updateSystem);

            if (system is ILateUpdateSystem lateUpdateSystem)
                LateUpdateSystems.Add(lateUpdateSystem);

            if (system is ITerminateSystem terminateSystem)
                TerminateSystems.Add(terminateSystem);

            return this;
        }

        public virtual void Init()
        {
            for (var i = 0; i < InitSystems.Count; i++)
            {
                InitSystems[i].Init();
            }
        }

        public virtual void Update()
        {
            for (var i = 0; i < UpdateSystem.Count; i++)
            {
                UpdateSystem[i].Update();
            }
        }

        public virtual void FixedUpdate()
        {
            for (var i = 0; i < FixedUpdateSystems.Count; i++)
            {
                FixedUpdateSystems[i].FixedUpdate();
            }
        }

        public virtual void LateUpdate()
        {
            for (var i = 0; i < LateUpdateSystems.Count; i++)
            {
                LateUpdateSystems[i].LateUpdate();
            }
        }

        public virtual void Terminate()
        {
            for (var i = 0; i < TerminateSystems.Count; i++)
            {
                TerminateSystems[i].Terminate();
            }
        }
    }
}