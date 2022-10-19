namespace SimpleEcs
{
    public interface ISystem { }

    public interface IInitSystem : ISystem
    {
        void Init();
    }
    
    public interface IUpdateSystem : ISystem
    {
        void Update();
    }
    
    public interface IFixedUpdateSystem : ISystem
    {
        void FixedUpdate();
    }
    
    public interface ILateUpdateSystem : ISystem
    {
        void LateUpdate();
    }

    public interface IDestroySystem : ISystem
    {
        void Destroy();
    }
}