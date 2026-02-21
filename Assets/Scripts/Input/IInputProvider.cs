using UnityEngine;

namespace AutoCombat.Input
{
    public interface IInputProvider
    {
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool CursorLocked { get; }
    }
}
