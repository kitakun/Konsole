#if KONSOLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Bagheads.UnityConsole.Utils
{
    internal static class PlayerInputUtils
    {
        public const string KONSOLE_ACTION_NAME = "Bagheads.Konsole";
#if KONSOLE_INPUT_SYSTEM
        public static string SwitchOn(this PlayerInput input, string name)
        {
            var nameBefore = input.currentActionMap.name;
            input.currentActionMap.Disable();
            input.SwitchCurrentActionMap(name);
            input.currentActionMap.Enable();
            return nameBefore;
        }
#endif
    }
}