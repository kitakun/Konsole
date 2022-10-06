using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bagheads.UnityConsole.Utils
{
    internal static class GoUtils
    {
        internal static bool TryFindComponentInActiveScene<T>(out T component) where T : Component
        {
            var currentScene = SceneManager.GetActiveScene();
            var sceneRootGos = currentScene.GetRootGameObjects();
            var existingCanvas = default(T);

            for (var i = 0; i < sceneRootGos.Length; i++)
            {
                existingCanvas = sceneRootGos[i].GetComponentInChildren<T>();
                if (existingCanvas != null)
                {
                    break;
                }
            }

            component = existingCanvas != null
                ? existingCanvas
                : default;
            return existingCanvas != null;
        }

        internal static RectTransform CreateRectGameObject(string name, RectTransform parent)
        {
            var newGo = new GameObject(name, typeof(RectTransform));
            var newGoTra = (RectTransform) newGo.transform;
            newGoTra.SetParent(parent);
            newGoTra.localScale = Vector3.one;
            return newGoTra;
        }

        internal static bool TryAddComponentAsChild<T>(RectTransform owner, out T component)
        {
            component = default;
            try
            {
                var textGo = new GameObject("Text", typeof(RectTransform), typeof(T));
                var textRect = textGo.GetComponent<RectTransform>();
                textRect.SetParent(owner);
                textRect.localScale = Vector3.one;

                component = textGo.GetComponent<T>();
            }
            catch (System.Exception es)
            {
                Debug.LogException(es);
            }

            return component != null;
        }
    }
}