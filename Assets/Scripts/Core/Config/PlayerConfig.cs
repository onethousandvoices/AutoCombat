using UnityEngine;

namespace AutoCombat.Core.Config
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "AutoCombat/PlayerConfig")]
    public sealed class PlayerConfig : ScriptableObject
    {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 8f;
        [field: SerializeField] public float AttackRadius { get; private set; } = 5f;
        [field: SerializeField] public float AttackCooldown { get; private set; } = 1.2f;
        [field: SerializeField] public float AttackDamage { get; private set; } = 34f;
        [field: SerializeField] public float RotationSpeed { get; private set; } = 720f;
    }
}
