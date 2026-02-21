using UnityEngine;
using UnityEngine.Rendering;

namespace AutoCombat.Player
{
    public sealed class PlayerView : MonoBehaviour
    {
        private GameObject _rangeIndicator;
        private MeshRenderer _rangeRenderer;
        private Material _greenMat;
        private Material _redMat;
        private float _currentRadius;

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
            if (_rangeIndicator && _rangeIndicator.activeSelf)
                _rangeIndicator.transform.position = new(position.x, 0.02f, position.z);
        }

        public void SetRotation(Quaternion rotation) => transform.rotation = rotation;

        public void ShowAttackRange(float radius)
        {
            if (!_rangeIndicator)
                CreateRangeIndicator();

            if (!Mathf.Approximately(_currentRadius, radius))
            {
                _currentRadius = radius;
                _rangeIndicator.transform.localScale = new(radius * 2f, 0.02f, radius * 2f);
            }

            if (!_rangeIndicator.activeSelf)
                _rangeIndicator.SetActive(true);

            _rangeIndicator.transform.position = new(transform.position.x, 0.02f, transform.position.z);
        }

        public void HideAttackRange()
        {
            if (_rangeIndicator && _rangeIndicator.activeSelf)
                _rangeIndicator.SetActive(false);
        }

        public void SetAttackRangeHostile(bool hostile)
        {
            if (_rangeRenderer != null)
                _rangeRenderer.sharedMaterial = hostile ? _redMat : _greenMat;
        }

        private void CreateRangeIndicator()
        {
            _rangeIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(_rangeIndicator.GetComponent<CapsuleCollider>());
            _rangeRenderer = _rangeIndicator.GetComponent<MeshRenderer>();
            _rangeRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _rangeRenderer.receiveShadows = false;
            _greenMat = Resources.Load<Material>("Materials/Green");
            _redMat = Resources.Load<Material>("Materials/Red");
            _rangeRenderer.sharedMaterial = _greenMat;
        }

        private void OnDestroy()
        {
            if (_rangeIndicator)
                Destroy(_rangeIndicator);
        }
    }
}
