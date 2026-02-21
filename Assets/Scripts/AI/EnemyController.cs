using System;
using System.Collections.Generic;
using AutoCombat.Core;
using AutoCombat.Core.Config;
using AutoCombat.Core.Models;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AutoCombat.AI
{
    public sealed class EnemyController : ITickable, IEnemyRegistry
    {
        [Inject] private CombatModel _combatModel;
        [Inject] private PlayerModel _playerModel;
        [Inject] private EnemyConfig _config;
        [Inject] private RoomConfig _roomConfig;
        [Inject] private ISteeringAvoidance _steering;
        [Inject] private ComponentPool<EnemyView> _pool;

        private readonly Dictionary<EnemyModel, EnemyView> _viewMap = new();
        private readonly List<IDisposable> _deathSubs = new();

        public void Tick()
        {
            var enemies = _combatModel.ActiveEnemies;
            var dt = Time.deltaTime;
            var speed = _config.MoveSpeed;

            for (var i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (enemy.IsDead.Value) continue;

                UpdatePatrol(enemy, speed);
            }

            _steering.ComputeVelocities(enemies, _config.AvoidanceRadius, _playerModel.Position.Value);

            for (var i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (enemy.IsDead.Value) continue;

                var vel = enemy.Velocity;
                if (vel.sqrMagnitude > speed * speed)
                    vel = vel.normalized * speed;

                enemy.Position = _roomConfig.Clamp(enemy.Position + vel * dt);

                if (_viewMap.TryGetValue(enemy, out var view))
                    view.SetPosition(enemy.Position);
            }
        }

        private static void UpdatePatrol(EnemyModel enemy, float speed)
        {
            if (enemy.Waypoints == null || enemy.Waypoints.Length == 0) return;

            var target = enemy.Waypoints[enemy.CurrentWaypointIndex];
            var diff = target - enemy.Position;
            diff.y = 0f;

            if (diff.sqrMagnitude < 4f)
            {
                enemy.CurrentWaypointIndex = (enemy.CurrentWaypointIndex + 1) % enemy.Waypoints.Length;
                target = enemy.Waypoints[enemy.CurrentWaypointIndex];
                diff = target - enemy.Position;
                diff.y = 0f;
            }

            enemy.PreferredVelocity = diff.sqrMagnitude > 0.01f
                ? diff.normalized * speed
                : Vector3.zero;
        }

        public void RegisterEnemy(EnemyModel model)
        {
            var view = _pool.Rent();
            view.SetPosition(model.Position);
            _viewMap[model] = view;

            var deathSub = model.IsDead
                .Where(dead => dead)
                .Take(1)
                .Subscribe(_ => HandleDeath(model));
            _deathSubs.Add(deathSub);

            var hitSub = model.Hp
                .Pairwise()
                .Where(pair => pair.Current < pair.Previous && !model.IsDead.Value)
                .Subscribe(_ =>
                {
                    if (_viewMap.TryGetValue(model, out var v))
                        v.PlayHitFeedback();
                });
            _deathSubs.Add(hitSub);
        }

        private void HandleDeath(EnemyModel model)
        {
            if (!_viewMap.TryGetValue(model, out var view)) return;

            view.PlayDeathAnimation(() =>
            {
                _viewMap.Remove(model);
                _pool.Return(view);
                _combatModel.ActiveEnemies.Remove(model);
                _combatModel.RespawnQueue.Enqueue(Time.time + _config.RespawnDelay);
            });
        }
    }
}
