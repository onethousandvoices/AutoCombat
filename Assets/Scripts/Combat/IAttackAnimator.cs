using System;
using UnityEngine;

namespace AutoCombat.Combat
{
    public interface IAttackAnimator
    {
        void PlayAttack(Func<Vector3> getTargetPosition, Action onHit, Action onComplete);
        void Stop();
    }
}
