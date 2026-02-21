using AutoCombat.Core.Config;
using UnityEngine;
using VContainer;

namespace AutoCombat.AI
{
    public sealed class PatrolWaypointProvider : IPatrolWaypointProvider
    {
        private const int WAYPOINT_COUNT = 6;

        [Inject] private RoomConfig _roomConfig;

        public Vector3[] GenerateWaypoints(Vector3 center, float radius)
        {
            var waypoints = new Vector3[WAYPOINT_COUNT];
            const float angleStep = 360f / WAYPOINT_COUNT;
            var baseAngle = Random.Range(0f, 360f);

            for (var i = 0; i < WAYPOINT_COUNT; i++)
            {
                var angle = (baseAngle + angleStep * i + Random.Range(-25f, 25f)) * Mathf.Deg2Rad;
                var dist = radius * Random.Range(0.4f, 1f);
                var offset = new Vector3(Mathf.Cos(angle) * dist, 0f, Mathf.Sin(angle) * dist);
                waypoints[i] = _roomConfig.Clamp(center + offset);
            }

            return waypoints;
        }
    }
}
