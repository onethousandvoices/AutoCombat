namespace AutoCombat.Core
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnRecycle();
    }
}
