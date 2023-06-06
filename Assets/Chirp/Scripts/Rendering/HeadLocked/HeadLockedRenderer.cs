using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace XRAccess.Chirp
{
    public class HeadLockedRenderer : CaptionRenderer
    {
        public GameObject captionCanvasPrefab;

        private Camera _mainCamera;
        [SerializeField] private GameObject _captionsContainer;

        [SerializeField] private HeadLockedOptions _options;
        public override RendererOptions options
        {
            get { return _options; }
            set { }
        }

        private List<(uint, GameObject)> _captionObjectMap;
        public List<(uint, GameObject)> captionObjectMap
        {
            get { return _captionObjectMap; }
        }

        private HeadLockedPositioner _positioner;

        private void Awake()
        {
            _captionObjectMap = new List<(uint, GameObject)>();
        }

        private void Start()
        {
            _mainCamera = CaptionSystem.Instance.mainCamera;

            GameObject captionsCameraObj = new GameObject("CaptionsCamera");
            Camera captionsCamera = captionsCameraObj.AddComponent<Camera>();
            captionsCameraObj.transform.SetParent(_mainCamera.transform, false);

            captionsCamera.CopyFrom(_mainCamera);
            captionsCamera.clearFlags = CameraClearFlags.Depth;
            captionsCamera.depth = _mainCamera.depth + 1f;
            captionsCamera.cullingMask = _options.layersOnTop;
            _mainCamera.cullingMask = ~_options.layersOnTop; // inverse of layermask

            _positioner = _captionsContainer.AddComponent<HeadLockedPositioner>();
        }

        private void Update()
        {
            Vector3 targetPosition = _mainCamera.transform.position;
            Quaternion targetRotation = _mainCamera.transform.rotation;

            // Disable rotation on z-axis to keep captions level with horizon
            if (_options.lockZRotation)
            {
                Vector3 euler = targetRotation.eulerAngles;
                euler.z = 0f;
                targetRotation = Quaternion.Euler(euler);
            }

            transform.position = targetPosition;

            float t = (_options.lag == 0f) ? 1f : Mathf.Clamp01(Time.deltaTime / _options.lag);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);
        }

        public override void AddCaption(uint captionID, Caption caption)
        {
            GameObject captionObject = CreateCaptionObject();
            captionObject.transform.SetParent(_captionsContainer.transform, false);

            _captionObjectMap.Add((captionID, captionObject));

            SetCaptionAttributes(captionObject, caption);
            _positioner.PositionLastCaption();
        }

        public override void RemoveCaption(uint id)
        {
            int index = _captionObjectMap.FindIndex(c => c.Item1 == id);
            if (index >= 0 && _captionObjectMap[index].Item2 != null)
            {
                Destroy(_captionObjectMap[index].Item2);
                _captionObjectMap.RemoveAt(index);
                _positioner.RepositionCaptions(0f, index); // reposition captions below the removed caption
            }
        }

        public override void RefreshCaptions(List<(uint, Caption)> currentCaptions)
        {
            _captionObjectMap.Clear();
            foreach (Transform child in _captionsContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach ((uint captionID, Caption caption) in currentCaptions)
            {
                AddCaption(captionID, caption);
            }
        }

        private GameObject CreateCaptionObject()
        {
            GameObject captionObject = Instantiate(captionCanvasPrefab);

            Canvas canvas = captionObject.GetComponent<Canvas>();
            canvas.transform.localScale = Vector3.one * _options.canvasScale;

            // set layer names for caption object and its children
            int captionLayer = LayerMask.NameToLayer(_options.captionLayerName);
            var transforms = captionObject.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var t in transforms)
            {
                t.gameObject.layer = captionLayer;
            }

            return captionObject;
        }

        private void SetCaptionAttributes(GameObject captionObject, Caption caption)
        {
            TMP_Text TMPText = captionObject.GetComponentInChildren<TMP_Text>();

            var options = CaptionSystem.Instance.options;
            TMPText.font = options.fontAsset;
            TMPText.fontSize = options.fontSize;
            TMPText.color = options.fontColor;
            TMPText.outlineWidth = options.OutlineWidth;
            TMPText.outlineColor = options.OutlineColor;
            TMPText.alignment = TextAlignmentOptions.Top; // TODO: check caption type before setting this

            TMPText.GetComponent<RectTransform>().sizeDelta = new Vector2(400f, 0f); // TODO set text width here

            if (caption is TimedCaption)
            {
                var c = (TimedCaption)caption;
                TMPText.SetText(c.captionText);
            }
            else if (caption is RealtimeCaption)
            {
                // implement later
            }

            captionObject.GetComponentInChildren<IndicatorArrowsController>().audioSource = caption.audioSource;
        }

    }
}
