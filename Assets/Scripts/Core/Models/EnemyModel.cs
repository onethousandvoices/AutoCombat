using UniRx;
using UnityEngine;

namespace AutoCombat.Core.Models
{
    public sealed class EnemyModel
    {
        public readonly ReactiveProperty<float> Hp = new();
        public readonly ReactiveProperty<bool> IsDead = new();

        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 PreferredVelocity;
        public int CurrentWaypointIndex;
        public Vector3[] Waypoints;

        public void Reset(float maxHp, Vector3 spawnPosition, Vector3[] waypoints)
        {
            Hp.Value = maxHp;
            IsDead.Value = false;
            Position = spawnPosition;
            Velocity = Vector3.zero;
            PreferredVelocity = Vector3.zero;
            CurrentWaypointIndex = 0;
            Waypoints = waypoints;
        }
    }
}
