namespace EcsLib.Api
{
    public static class EcsUtils
    {
        public static Entity ToEntity(this int id, EcsManager manager = null)
        {
            if (manager == null)
                manager = EcsManager.Instance;
            return manager.GetEntityById(id);
        }
    }
}