using UnityEngine;

namespace AutoCombat.Core.Config
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "AutoCombat/CameraConfig")]
    public sealed class CameraConfig : ScriptableObject
    {
        [field: SerializeField] public float Distance { get; private set; } = 12f;
        [field: SerializeField] public float MinPitch { get; private set; } = 10f;
        [field: SerializeField] public float MaxPitch { get; private set; } = 80f;
        [field: SerializeField] public float Sensitivity { get; private set; } = 0.15f;
        [field: SerializeField] public float FollowSmoothing { get; private set; } = 10f;
    }
}
