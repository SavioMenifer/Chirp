using UnityEngine;

namespace XRAccess.Chirp
{
    public class SafeArea : MonoBehaviour
    {
        public GameObject safeAreaVisualPrefab;
        [Range(0f, 100f)] public float safeAreaPercent;
        public bool showSafeAreaVisual = false;
        public float defaultDistance;

        private Camera _mainCamera;
        private GameObject _safeAreaVisual;

        private void Start()
        {
            _mainCamera = CaptionSystem.Instance.mainCamera;

            _safeAreaVisual = Instantiate(safeAreaVisualPrefab, this.transform);
            _safeAreaVisual.transform.localPosition = new Vector3(0f, 0f, defaultDistance);
            _safeAreaVisual.SetActive(false);
        }

        private void Update()
        {
            if (showSafeAreaVisual)
            {
                if (!_safeAreaVisual.activeSelf) { _safeAreaVisual.SetActive(true); };
                var SafeAreaSize = GetSize(_safeAreaVisual.transform.localPosition.z);
                _safeAreaVisual.transform.localScale = new Vector3(SafeAreaSize.x, SafeAreaSize.y, 1f);
            }
        }

        public Vector2 GetSize(float distance)
        {
            float height = 2.0f * distance * Mathf.Tan(_mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * (safeAreaPercent / 100f);
            float width = height * _mainCamera.aspect;

            return new Vector2(width, height);
        }

        public Vector2 GetAngles()
        {
            float verticalAngle = _mainCamera.fieldOfView * (safeAreaPercent / 100f);
            float horizontalAngle = Camera.VerticalToHorizontalFieldOfView(_mainCamera.fieldOfView, _mainCamera.aspect) * (safeAreaPercent / 100f);

            return new Vector2(horizontalAngle, verticalAngle);
        }
    }
}
