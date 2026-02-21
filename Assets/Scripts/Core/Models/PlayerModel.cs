using UniRx;
using UnityEngine;

namespace AutoCombat.Core.Models
{
    public sealed class PlayerModel
    {
        public readonly ReactiveProperty<Vector3> Position = new();
        public readonly ReactiveProperty<bool> IsMoving = new();
        public readonly ReactiveProperty<bool> IsAttacking = new();
    }
}
