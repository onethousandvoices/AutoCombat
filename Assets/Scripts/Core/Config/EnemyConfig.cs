using UnityEngine;

namespace AutoCombat.Core.Config
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "AutoCombat/EnemyConfig")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [field: SerializeField] public float MaxHp { get; private set; } = 100f;
        [field: SerializeField] public float MoveSpeed { get; private set; } = 3.5f;
        [field: SerializeField] public float RespawnDelay { get; private set; } = 3f;
        [field: SerializeField] public int MaxEnemies { get; private set; } = 5;
        [field: SerializeField] public float PatrolRadius { get; private set; } = 15f;
        [field: SerializeField] public float AvoidanceRadius { get; private set; } = 2.5f;
    }
}
