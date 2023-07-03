using System;
using UnityEngine;
using TMPro;

namespace XRAccess.Chirp
{
    public enum PositioningMode
    {
        HeadLocked
    }

    [Serializable]
    public class CaptionOptions
    {
        public bool enableCaptions;

        public PositioningMode positioningMode;

        [Header("Font Options")]
        public TMP_FontAsset fontAsset;
        public float fontSize;
        public Color fontColor;
        public float outlineWidth;
        public Color outlineColor;

        [Header("Background Options")]
        public Material backgroundMaterial;

        [Header("Other Options")]
        public bool reducedMotion;
        public float extendDuration;
    }

    [Serializable]
    public abstract class RendererOptions { }

    [Serializable]
    public class HeadLockedOptions : RendererOptions
    {
        public float lag = 0.3f;
        public float xAxisTilt = 0f;
        public float defaultCaptionDistance = 1.6f;
        public float canvasScale = 0.003f;
        public bool lockZRotation = true;
        public string captionLayerName = "Captions";
        public LayerMask layersOnTop;
        public float repositionDelay = 1f;
        public float captionMoveDuration = 0.2f;
        public float captionGap = 0.02f;
        public int reservedSpaceLines = 2;
        public bool showIndicatorArrows = true;
    }
}