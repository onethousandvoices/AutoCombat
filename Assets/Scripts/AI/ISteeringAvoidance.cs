using System.Collections.Generic;
using AutoCombat.Core.Models;
using UnityEngine;

namespace AutoCombat.AI
{
    public interface ISteeringAvoidance
    {
        void ComputeVelocities(List<EnemyModel> agents, float avoidanceRadius, Vector3 playerPosition);
    }
}
