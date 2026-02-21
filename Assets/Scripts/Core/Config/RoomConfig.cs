using UnityEngine;

namespace AutoCombat.Core.Config
{
    [CreateAssetMenu(fileName = "RoomConfig", menuName = "AutoCombat/RoomConfig")]
    public sealed class RoomConfig : ScriptableObject
    {
        [field: SerializeField] public Vector2 HalfExtents { get; private set; } = new(45f, 45f);

        public Vector3 Clamp(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, -HalfExtents.x, HalfExtents.x);
            position.z = Mathf.Clamp(position.z, -HalfExtents.y, HalfExtents.y);
            return position;
        }
    }
}
