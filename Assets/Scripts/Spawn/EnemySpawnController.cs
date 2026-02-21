using AutoCombat.AI;
using AutoCombat.Core.Config;
using AutoCombat.Core.Models;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace AutoCombat.Spawn
{
    public sealed class EnemySpawnController : IStartable, ITickable
    {
        [Inject] private CombatModel _combatModel;
        [Inject] private EnemyConfig _config;
        [Inject] private RoomConfig _roomConfig;
        [Inject] private IEnemyRegistry _enemyRegistry;
        [Inject] private IPatrolWaypointProvider _waypointProvider;

        public void Start()
        {
            _combatModel.MaxEnemies.Value = _config.MaxEnemies;

            _combatModel.MaxEnemies
                .Pairwise()
                .Subscribe(pair => OnMaxEnemiesChanged(pair.Current));

            SpawnInitialWave();
        }

        public void Tick()
        {
            _combatModel.MaxEnemies.Value = _config.MaxEnemies;

            var queue = _combatModel.RespawnQueue;
            var now = Time.time;

            while (queue.Count > 0 &&
                   queue.Peek() <= now &&
                   _combatModel.ActiveEnemies.Count < _combatModel.MaxEnemies.Value)
            {
                queue.Dequeue();
                SpawnEnemy();
            }
        }

        private void OnMaxEnemiesChanged(int newMax)
        {
            while (_combatModel.ActiveEnemies.Count < newMax)
                SpawnEnemy();
        }

        private void SpawnInitialWave()
        {
            var max = _combatModel.MaxEnemies.Value;
            for (var i = 0; i < max; i++)
                SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            var half = _roomConfig.HalfExtents;
            var spawnPos = new Vector3(Random.Range(-half.x, half.x), 1f, Random.Range(-half.y, half.y));
            var waypoints = _waypointProvider.GenerateWaypoints(spawnPos, _config.PatrolRadius);

            var model = new EnemyModel();
            model.Reset(_config.MaxHp, spawnPos, waypoints);
            model.CurrentWaypointIndex = Random.Range(0, waypoints.Length);

            _combatModel.ActiveEnemies.Add(model);
            _enemyRegistry.RegisterEnemy(model);
        }
    }
}
