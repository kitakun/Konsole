using System;

using UnityEngine;

namespace Bagheads.UnityConsole.Components
{
    public static partial class PreventFromEditor
    {
        internal class ControlContainerHeight : MonoBehaviour
        {
            public int HeightValue { get; internal set; }
            public bool IsPercents { get; internal set; }

            // private state
            private Rect _knownResolution;
            private RectTransform _rect;

            // unity

            protected void Awake()
            {
                _rect = transform as RectTransform;
            }

            protected void Start()
            {
                _knownResolution = Screen.safeArea;
            }

            protected void Update()
            {
                var curResolution = Screen.safeArea;
                if (Math.Abs(_knownResolution.width - curResolution.width) > Mathf.Epsilon
                    || Math.Abs(_knownResolution.height - curResolution.height) > Mathf.Epsilon)
                {
                    OnScreenSizeChanged();
                }
            }

            // private

            private void OnScreenSizeChanged()
            {
                _knownResolution = Screen.safeArea;
                var wantedToSetHeight = !IsPercents
                    ? HeightValue
                    : (HeightValue / 100f) * _knownResolution.height;

                if (wantedToSetHeight > _knownResolution.height)
                {
                    // protect yourself and make console fit in screen
                    wantedToSetHeight = _knownResolution.height;
                }
                else if (wantedToSetHeight < 50)
                {
                    // protect yourself from minimum unusable size
                    wantedToSetHeight = 50;
                }

                _rect.sizeDelta = new Vector2(0, wantedToSetHeight);
            }

            // public APIs

            public void SetHeight(int targetHeight)
            {
                HeightValue = targetHeight;
                IsPercents = false;
                OnScreenSizeChanged();
            }

            public void SetHeightPercent(int percentValue)
            {
                HeightValue = percentValue;
                IsPercents = true;
                OnScreenSizeChanged();
            }
        }
    }
}