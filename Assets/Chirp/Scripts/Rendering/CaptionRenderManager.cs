/*
 * Render captions
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRAccess.Chirp
{
    public class CaptionRenderManager : MonoBehaviour
    {
        public static CaptionRenderManager Instance;

        public GameObject HeadLockedPrefab;

        private uint nextCaptionID = 0;

        private List<(uint, Caption)> _currentCaptions = new List<(uint, Caption)>();
        private GameObject _currentRenderer;

        public List<(uint, Caption)> currentCaptions
        {
            get { return _currentCaptions; }
        }

        public CaptionRenderer currentRenderer
        {
            get { return _currentRenderer.GetComponent<CaptionRenderer>(); }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            EnableRenderer(CaptionSystem.Instance.options.positioningMode);
        }

        private uint GenerateCaptionID()
        {
            return nextCaptionID++;
        }

        public void AddTimedCaption(TimedCaption caption)
        {
            uint newID = GenerateCaptionID();
            _currentCaptions.Add((newID, caption));
            _currentRenderer.GetComponent<CaptionRenderer>().AddCaption(newID, caption);

            StartCoroutine(RemoveAfterDuration(newID, caption.duration));
        }

        private IEnumerator RemoveAfterDuration(uint captionID, float duration)
        {
            yield return new WaitForSeconds(duration);

            _currentRenderer.GetComponent<CaptionRenderer>().RemoveCaption(captionID);
            _currentCaptions.RemoveAll(caption => caption.Item1 == captionID);

        }

        public void EnableRenderer(PositioningMode mode)
        {
            if (_currentRenderer != null) { Destroy(_currentRenderer); }

            switch (mode)
            {
                case PositioningMode.HeadLocked:
                    _currentRenderer = Instantiate(HeadLockedPrefab, CaptionSystem.Instance.transform);
                    break;
                default:
                    break;
            }

            _currentRenderer.GetComponent<CaptionRenderer>().RefreshCaptions(_currentCaptions);
        }
    }
}
