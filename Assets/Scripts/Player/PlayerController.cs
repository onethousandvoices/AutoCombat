using AutoCombat.Camera;
using AutoCombat.Core.Config;
using AutoCombat.Core.Models;
using AutoCombat.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AutoCombat.Player
{
    public sealed class PlayerController : IStartable, ITickable
    {
        [Inject] private IInputProvider _input;
        [Inject] private ICameraProvider _camera;
        [Inject] private PlayerModel _model;
        [Inject] private PlayerView _view;
        [Inject] private PlayerConfig _config;
        [Inject] private RoomConfig _roomConfig;

        public void Start()
        {
            _model.Position.Value = _view.transform.position;
        }

        public void Tick()
        {
            if (_model.IsAttacking.Value)
            {
                _model.IsMoving.Value = false;
                return;
            }

            var raw = _input.MoveInput;
            var sqr = raw.sqrMagnitude;

            if (sqr < 0.01f)
            {
                _model.IsMoving.Value = false;
                return;
            }

            var yawRad = _camera.Yaw * Mathf.Deg2Rad;
            var sinYaw = Mathf.Sin(yawRad);
            var cosYaw = Mathf.Cos(yawRad);

            var forward = new Vector3(sinYaw, 0f, cosYaw);
            var right = new Vector3(cosYaw, 0f, -sinYaw);

            var moveDir = forward * raw.y + right * raw.x;
            if (sqr > 1f) moveDir.Normalize();

            var dt = Time.deltaTime;
            var newPos = _model.Position.Value + moveDir * (_config.MoveSpeed * dt);
            newPos = _roomConfig.Clamp(newPos);

            _model.Position.Value = newPos;
            _model.IsMoving.Value = true;

            _view.SetPosition(newPos);
            var targetRot = Quaternion.LookRotation(moveDir);
            var currentRot = _view.transform.rotation;
            _view.SetRotation(Quaternion.RotateTowards(currentRot, targetRot, _config.RotationSpeed * dt));
        }
    }
}
