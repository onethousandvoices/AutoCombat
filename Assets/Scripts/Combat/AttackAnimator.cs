using System;
using AutoCombat.Player;
using DG.Tweening;
using UnityEngine;
using VContainer;

namespace AutoCombat.Combat
{
    public sealed class AttackAnimator : IAttackAnimator
    {
        [Inject] private PlayerView _playerView;

        private Sequence _attackSequence;

        public void PlayAttack(Func<Vector3> getTargetPos, Action onHit, Action onComplete)
        {
            Stop();
            var t = _playerView.transform;
            var origin = t.position;

            _attackSequence = DOTween.Sequence()
                .Append(t.DOScaleY(0.85f, 0.06f).SetEase(Ease.InQuad))
                .OnComplete(() => ExecuteLunge(t, origin, getTargetPos, onHit, onComplete));
        }

        private void ExecuteLunge(Transform t, Vector3 origin, Func<Vector3> getTargetPos, Action onHit, Action onComplete)
        {
            var targetPos = getTargetPos();
            var dir = targetPos - origin;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.25f)
                t.rotation = Quaternion.LookRotation(dir);

            var lungeTarget = origin + dir.normalized * (dir.magnitude * 0.75f);

            _attackSequence = DOTween.Sequence()
                .Append(t.DOScaleY(1.15f, 0.04f).SetEase(Ease.OutQuad))
                .Join(t.DOMove(lungeTarget, 0.1f).SetEase(Ease.OutExpo))
                .Append(t.DOScale(Vector3.one, 0.04f))
                .AppendCallback(() => onHit?.Invoke())
                .Append(t.DOPunchScale(new(0.15f, -0.1f, 0.15f), 0.15f, 8, 0.5f))
                .Append(t.DOMove(origin, 0.15f).SetEase(Ease.InOutQuad))
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Stop()
        {
            _attackSequence?.Kill();
            _attackSequence = null;
        }
    }
}
