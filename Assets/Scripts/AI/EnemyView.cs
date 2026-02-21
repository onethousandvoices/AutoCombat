using AutoCombat.Core;
using DG.Tweening;
using UnityEngine;

namespace AutoCombat.AI
{
    public sealed class EnemyView : MonoBehaviour, IPoolable
    {
        private Sequence _deathSequence;
        private Tween _hitTween;
        private MeshRenderer _renderer;
        private Material _originalMat;
        private Vector3 _basePosition;
        private Vector3 _hitOffset;

        private static Material _redMat;

        private void Awake()
        {
            _renderer = GetComponentInChildren<MeshRenderer>();
            _originalMat = _renderer.sharedMaterial;
            _redMat ??= Resources.Load<Material>("Materials/Red");
        }

        public void SetPosition(Vector3 position)
        {
            _basePosition = position;
            transform.position = position + _hitOffset;
        }

        public void PlayHitFeedback()
        {
            _hitTween?.Kill(true);
            _hitOffset = Vector3.zero;
            _renderer.sharedMaterial = _redMat;

            _hitTween = DOTween.Sequence()
                .Append(DOTween.Punch(() => _hitOffset, v =>
                {
                    _hitOffset = v;
                    transform.position = _basePosition + v;
                }, new(0.3f, 0f, 0.3f), 0.2f, 10, 0.5f))
                .AppendInterval(0.1f)
                .OnComplete(() =>
                {
                    _hitOffset = Vector3.zero;
                    transform.position = _basePosition;
                    _renderer.sharedMaterial = _originalMat;
                });
        }

        public void PlayDeathAnimation(System.Action onComplete)
        {
            _hitTween?.Kill(true);
            _hitOffset = Vector3.zero;
            _renderer.sharedMaterial = _redMat;
            _deathSequence?.Kill();
            _deathSequence = DOTween.Sequence()
                .Append(transform.DORotate(new(90f, 0f, 0f), 0.4f).SetEase(Ease.OutBounce))
                .Append(transform.DOMoveY(-10f, 0.8f).SetEase(Ease.InQuad))
                .OnComplete(() => onComplete?.Invoke());
        }

        public void OnSpawn()
        {
            _hitOffset = Vector3.zero;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localScale = Vector3.one;
            if (_renderer)
                _renderer.sharedMaterial = _originalMat;
        }

        public void OnRecycle()
        {
            _hitTween?.Kill();
            _hitTween = null;
            _deathSequence?.Kill();
            _deathSequence = null;
            _hitOffset = Vector3.zero;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.localScale = Vector3.one;
            if (_renderer != null)
                _renderer.sharedMaterial = _originalMat;
        }

        private void OnDestroy()
        {
            _hitTween?.Kill();
            _deathSequence?.Kill();
        }
    }
}
