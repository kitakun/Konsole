using UnityEngine;

namespace Bagheads.UnityConsole.Data
{
    public class IntegrationOptions
    {
        /// <summary>
        /// If True - will found NewInputSystem component and will try to inject action to toggle console
        /// </summary>
        public bool UseNewInputSystem = false;

        /// <summary>
        /// Action name to be injected into
        /// </summary>
        public string NewInputSystemToggleAction;

        /// <summary>
        /// Use TextMeshPro instead UnityEngine.Text
        /// </summary>
        public bool UseTextMeshPro = false;

        /// <summary>
        /// Font for text
        /// </summary>
        public Font DefaultTextFont;

        /// <summary>
        /// Font size for logs
        /// </summary>
        public int FontSize = 14;

        /// <summary>
        /// Write time in log, for example <c>[10:30:07] [Log] HelloWorld</c>
        /// </summary>
        public bool WriteLogTime = true;
        
        /// <summary>
        /// Colors in console
        /// </summary>
        public ThemeOptions Theme;

        /// <summary>
        /// How much lines will be under InputFields with founded similar commands
        /// </summary>
        public int AutoCompleteLines = 3;
    }
}