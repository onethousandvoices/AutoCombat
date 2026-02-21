using System.Collections.Generic;
using AutoCombat.Core.Models;
using UnityEngine;

namespace AutoCombat.AI
{
    public sealed class SteeringAvoidance : ISteeringAvoidance
    {
        private const float TIME_HORIZON = 0.5f;
        private const float INVERSE_TIME_HORIZON = 1f / TIME_HORIZON;

        public void ComputeVelocities(List<EnemyModel> agents, float avoidanceRadius, Vector3 playerPosition)
        {
            for (var i = 0; i < agents.Count; i++)
            {
                var agent = agents[i];
                if (agent.IsDead.Value) continue;

                var adjustedVel = agent.PreferredVelocity;

                for (var j = 0; j < agents.Count; j++)
                {
                    if (i == j) continue;
                    var other = agents[j];
                    if (other.IsDead.Value) continue;

                    adjustedVel = ApplyAvoidance(agent.Position, adjustedVel, other.Position, other.Velocity, avoidanceRadius);
                }

                adjustedVel = ApplyAvoidance(agent.Position, adjustedVel, playerPosition, Vector3.zero, avoidanceRadius);

                agent.Velocity = adjustedVel;
            }
        }

        private static Vector3 ApplyAvoidance(Vector3 posA, Vector3 velA, Vector3 posB, Vector3 velB, float combinedRadius)
        {
            var relPos = posB - posA;
            relPos.y = 0f;
            var distSq = relPos.sqrMagnitude;
            var minDist = combinedRadius * 2f;
            var minDistSq = minDist * minDist;

            if (distSq > minDistSq * 4f) return velA;

            if (distSq < 0.0001f)
            {
                velA.x += posA.x + posA.z * 31f > 0f ? 1f : -1f;
                velA.y = 0f;
                return velA;
            }

            if (distSq < minDistSq)
            {
                var dist = Mathf.Sqrt(distSq);
                var pushDir = (posA - posB) / dist;
                velA += pushDir * ((minDist - dist) * INVERSE_TIME_HORIZON);
                velA.y = 0f;
                return velA;
            }

            var relVel = velA - velB;
            var w = relVel - relPos * INVERSE_TIME_HORIZON;
            var wLenSq = w.sqrMagnitude;
            var voRadius = minDist * INVERSE_TIME_HORIZON;

            if (wLenSq >= voRadius * voRadius) return velA;
            if (wLenSq < 0.0001f) return velA;

            var wLen = Mathf.Sqrt(wLenSq);
            var wNorm = w / wLen;
            velA += wNorm * ((voRadius - wLen) * 0.5f);
            velA.y = 0f;
            return velA;
        }
    }
}
