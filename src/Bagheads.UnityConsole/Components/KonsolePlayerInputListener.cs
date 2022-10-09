using Bahgeads.UnityConsole.Components;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Bagheads.UnityConsole.Components
{
    public class KonsolePlayerInputListener : MonoBehaviour
    {
        /// <summary>
        /// Catch SendMessage or BroadcastMessage from PlayerInput
        /// </summary>
        /// <param name="input">call params</param>
        protected void OnToggleConsole(InputValue input)
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
            var rawInput = input.Get<float>();
            if (rawInput != 0
                && Konsole.ConsoleInstance != null
                && Konsole.ConsoleInstance.TryGetInternalComponent<TypeaheadComponent>(out var typeahead))
            {
                var isDown = input.Get<float>() < 0.1f;

                typeahead.OnInput_Direction(isDown);
            }
        }
    }
}