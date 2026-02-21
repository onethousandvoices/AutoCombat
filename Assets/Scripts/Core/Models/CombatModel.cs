using System.Collections.Generic;
using UniRx;

namespace AutoCombat.Core.Models
{
    public sealed class CombatModel
    {
        public readonly ReactiveProperty<int> KillCount = new();
        public readonly ReactiveProperty<int> MaxEnemies = new();
        public readonly List<EnemyModel> ActiveEnemies = new();
        public readonly Queue<float> RespawnQueue = new();
    }
}
