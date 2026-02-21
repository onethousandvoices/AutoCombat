using AutoCombat.Core.Config;
using AutoCombat.Core.Models;
using AutoCombat.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AutoCombat.Camera
{
    public sealed class OrbitalCameraController : IStartable, ILateTickable, ICameraProvider
    {
        [Inject] private InputController _input;
        [Inject] private PlayerModel _playerModel;
        [Inject] private CameraConfig _config;

        private UnityEngine.Camera _camera;
        private float _pitch = 30f;

        public float Yaw { get; private set; }

        public void Start()
        {
            _camera = UnityEngine.Camera.main;
        }

        public void LateTick()
        {
            if (_input.CursorLocked)
            {
                var look = _input.LookInput;
                Yaw += look.x * _config.Sensitivity;
                _pitch -= look.y * _config.Sensitivity;
                _pitch = Mathf.Clamp(_pitch, _config.MinPitch, _config.MaxPitch);
            }

            var rotation = Quaternion.Euler(_pitch, Yaw, 0f);
            var targetPosition = _playerModel.Position.Value;
            var offset = rotation * new Vector3(0f, 0f, -_config.Distance);

            var cameraTransform = _camera.transform;
            var desiredPos = targetPosition + offset;
            var dt = Time.deltaTime * _config.FollowSmoothing;

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPos, dt);
            cameraTransform.LookAt(targetPosition);
        }
    }
}
