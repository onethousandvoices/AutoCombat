using AutoCombat.Core.Models;

namespace AutoCombat.AI
{
    public interface IEnemyRegistry
    {
        void RegisterEnemy(EnemyModel model);
    }
}
