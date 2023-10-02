using Bahgeads.UnityConsole.Components;

using UnityEngine;
#if KONSOLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Bagheads.UnityConsole.Components
{
    public class KonsolePlayerInputListener : MonoBehaviour
    {
#if KONSOLE_INPUT_SYSTEM
        /// <summary>
        /// Catch SendMessage or BroadcastMessage from PlayerInput
        /// </summary>
        /// <param name="input">call params</param>
        protected void OnToggleConsole(InputValue input)
        {
            ApplyToggleConsole();
        }

        public static void ApplyToggleConsole()
        {
            if (Konsole.ConsoleInstance != null)
            {
                Konsole.ConsoleInstance.ToggleConsole();
            }
        }

        /// <summary>
        /// Tab is pressed
        /// </summary>
        protected void OnTab(InputValue input)
        {
            ApplyTab();
        }

        public static void ApplyTab()
        {
            if (Konsole.ConsoleInstance != null
                && Konsole.ConsoleInstance.TryGetInternalComponent<TypeaheadComponent>(out var typeahead))
            {
                typeahead.OnInput_Tab();
            }
        }

        /// <summary>
        /// Arrow input
        /// </summary>
        protected void OnSelectDirection(InputValue input)
        {
            var rawValue = input.Get<float>();
            if (rawValue != 0)
            {
                var isDown = rawValue < 0.1f;

                ApplySelectDirection(isDown);
            }
        }

        public static void ApplySelectDirection(bool isDown)
        {
            if (Konsole.ConsoleInstance != null
                && Konsole.ConsoleInstance.TryGetInternalComponent<TypeaheadComponent>(out var typeahead))
            {

                typeahead.OnInput_Direction(isDown);
            }
        }
#endif
    }
}