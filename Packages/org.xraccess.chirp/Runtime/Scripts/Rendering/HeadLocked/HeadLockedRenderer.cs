using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;
using TMPro;

namespace XRAccess.Chirp
{
    public class HeadLockedRenderer : CaptionRenderer
    {
        public GameObject captionCanvasPrefab;

        private Camera _mainCamera;
        [SerializeField] private GameObject _captionsContainer;
        [SerializeField] private HeadLockedPositioner _positioner;

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

        private void Awake()
        {
            _captionObjectMap = new List<(uint, GameObject)>();
        }

        private void Start()
        {
            _mainCamera = CaptionSystem.Instance.mainCamera;

            InitCaptionCamera();

            // init dependent scripts
            _captionsContainer.GetComponent<HeadLockedPositioner>().Init();
            GetComponent<SafeArea>().Init();

            SetOptions();
            RefreshCaptions(CaptionRenderManager.Instance.currentCaptions);
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

            SetOptions();
        }

        private GameObject CreateCaptionObject()
        {
            GameObject captionObject = Instantiate(captionCanvasPrefab);

            captionObject.transform.localScale = Vector3.one * _options.canvasScale;

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
            TMPText.outlineWidth = options.outlineWidth;
            TMPText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, options.outlineWidth);
            TMPText.outlineColor = options.outlineColor;
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

            if (_options.showIndicatorArrows == true && caption.audioSource.spatialBlend > 0f)
            {
                var arrowsController = captionObject.GetComponentInChildren<IndicatorArrowsController>();
                arrowsController.enabled = true;
                arrowsController.audioSource = caption.audioSource;
            }
        }

        private void SetOptions()
        {
            _captionsContainer.transform.localRotation = Quaternion.Euler(_options.xAxisTilt, 0f, 0f);
            _captionsContainer.transform.localPosition = new Vector3(0f, 0f, _options.defaultCaptionDistance);
        }

        private void InitCaptionCamera()
        {
            GameObject captionCameraObj = new GameObject("CaptionCamera");
            Camera captionCamera = captionCameraObj.AddComponent<Camera>();
            captionCameraObj.transform.SetParent(_mainCamera.transform, false);

            captionCamera.CopyFrom(_mainCamera);
            captionCamera.clearFlags = CameraClearFlags.Depth;
            captionCamera.depth = _mainCamera.depth + 1f;
            captionCamera.cullingMask = _options.layersOnTop;
            _mainCamera.cullingMask = ~_options.layersOnTop; // inverse of layermask

            /*
            if (GraphicsSettings.defaultRenderPipeline.name.Contains("UniversalRenderPipeline"))
            {
                var captionsCameraData = captionCamera.GetUniversalAdditionalCameraData();
                var mainCameraData = _mainCamera.GetUniversalAdditionalCameraData();

                captionsCameraData.renderType = CameraRenderType.Overlay;
                mainCameraData.cameraStack.Add(captionCamera);
            }
            */
        }

    }
}
