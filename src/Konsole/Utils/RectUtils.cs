using UnityEngine;

namespace Konsole.Utils
{
    internal static class RectUtils
    {
        /// <summary>
        /// |[==]|
        /// ||==||
        /// |[==]|
        /// </summary>
        internal static void SetUseAllSpace(RectTransform transform, Vector2 sizeDelta, Vector2? anchorPos = null, Vector2? pivot = null)
        {
            anchorPos ??= new Vector2(0, -0.5f);
            pivot ??= new Vector2(0.5f, 0.5f);

            transform.anchorMin = new Vector2(0, 0);
            transform.anchorMax = new Vector2(1, 1);
            transform.anchoredPosition = anchorPos.Value;
            transform.pivot = pivot.Value;
            transform.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// |    |
        /// |    |
        /// |[==]|
        /// </summary>
        internal static void SetUseBottomLine(RectTransform transform, Vector2 sizeDelta)
        {
            transform.anchorMin = new Vector2(0, 0);
            transform.anchorMax = new Vector2(1, 0);
            transform.anchoredPosition = new Vector2(0, 0);
            transform.pivot = new Vector2(0, 0);
            transform.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// |[==]|
        /// |    |
        /// |    |
        /// </summary>
        internal static void SetUseTopLine(RectTransform transform, Vector2 sizeDelta)
        {
            transform.anchorMin = new Vector2(0, 1);
            transform.anchorMax = new Vector2(1, 1);
            transform.anchoredPosition = new Vector2(0, 0);
            transform.pivot = new Vector2(0.5f, 1);
            transform.sizeDelta = sizeDelta;
        }

        /// <summary>
        /// |   [|
        /// |   [|
        /// |   [|
        /// </summary>
        internal static void SetUseRightLine(RectTransform transform, Vector2 sizeDelta)
        {
            transform.anchorMin = new Vector2(1, 0);
            transform.anchorMax = new Vector2(1, 1);
            transform.anchoredPosition = new Vector2(0, 0);
            transform.pivot = new Vector2(1, 1);
            transform.sizeDelta = sizeDelta;
        }
    }
}