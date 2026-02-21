using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace AutoCombat.Input
{
    public sealed class InputController : IStartable, ITickable, IInputProvider
    {
        [Inject] private InputActionAsset _asset;

        private InputAction _moveAction;
        private InputAction _lookAction;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool CursorLocked { get; private set; } = true;

        public void Start()
        {
            var playerMap = _asset.FindActionMap("Player");
            _moveAction = playerMap.FindAction("Move");
            _lookAction = playerMap.FindAction("Look");
            playerMap.Enable();
            SetCursorLocked(true);
        }

        public void Tick()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                SetCursorLocked(!CursorLocked);

            MoveInput = CursorLocked ? _moveAction.ReadValue<Vector2>() : Vector2.zero;
            LookInput = CursorLocked ? _lookAction.ReadValue<Vector2>() : Vector2.zero;
        }

        private void SetCursorLocked(bool locked)
        {
            CursorLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
