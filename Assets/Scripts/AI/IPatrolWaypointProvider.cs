using UnityEngine;

namespace AutoCombat.AI
{
    public interface IPatrolWaypointProvider
    {
        Vector3[] GenerateWaypoints(Vector3 center, float radius);
    }
}
