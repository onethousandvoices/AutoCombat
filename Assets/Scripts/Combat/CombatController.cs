using AutoCombat.Core.Config;
using AutoCombat.Core.Models;
using AutoCombat.Player;
using AutoCombat.UI;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AutoCombat.Combat
{
    public sealed class CombatController : IStartable, ITickable
    {
        [Inject] private PlayerModel _playerModel;
        [Inject] private CombatModel _combatModel;
        [Inject] private PlayerConfig _playerConfig;
        [Inject] private IAttackAnimator _attackAnimator;
        [Inject] private KillCounterView _killCounterView;
        [Inject] private PlayerView _playerView;

        private float _cooldownTimer;

        public void Start()
        {
            _combatModel.KillCount
                .Subscribe(count => _killCounterView.SetKillCount(count));
        }

        public void Tick()
        {
            _cooldownTimer -= Time.deltaTime;

            if (_playerModel.IsMoving.Value)
            {
                _playerView.HideAttackRange();
                return;
            }

            var target = FindClosestAliveEnemy();
            _playerView.ShowAttackRange(_playerConfig.AttackRadius);
            _playerView.SetAttackRangeHostile(target != null);

            if (_playerModel.IsAttacking.Value || _cooldownTimer > 0f)
                return;

            if (target == null) return;

            _playerModel.IsAttacking.Value = true;
            _cooldownTimer = _playerConfig.AttackCooldown;
            _attackAnimator.PlayAttack(() => target.Position, () => DealDamage(target), () => _playerModel.IsAttacking.Value = false);
        }

        private void DealDamage(EnemyModel target)
        {
            if (target.IsDead.Value) return;

            target.Hp.Value -= _playerConfig.AttackDamage;
            if (target.Hp.Value > 0f)
                return;
            target.IsDead.Value = true;
            _combatModel.KillCount.Value++;
        }

        private EnemyModel FindClosestAliveEnemy()
        {
            var playerPos = _playerModel.Position.Value;
            var radiusSq = _playerConfig.AttackRadius * _playerConfig.AttackRadius;
            EnemyModel closest = null;
            var closestDistSq = float.MaxValue;

            var enemies = _combatModel.ActiveEnemies;
            for (var i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (enemy.IsDead.Value) continue;

                var diff = enemy.Position - playerPos;
                diff.y = 0f;
                var distSq = diff.sqrMagnitude;

                if (distSq >= radiusSq || distSq >= closestDistSq)
                    continue;
                closestDistSq = distSq;
                closest = enemy;
            }

            return closest;
        }
    }
}
