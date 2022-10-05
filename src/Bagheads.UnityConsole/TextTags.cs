using UnityEngine;

namespace Bagheads.UnityConsole
{
    public static class TextTags
    {
        internal const string BoldTag = "<b>";
        internal const string BoldCloseTag = "</b>";
        public static string Bold(string src) => $"{BoldTag}{src}{BoldCloseTag}";

        internal const string ItalianTag = "<i>";
        internal const string ItalianCloseTag = "</i>";
        public static string Italian(string src) => $"{ItalianTag}{src}{ItalianCloseTag}";

        public static string WithColor(LogType type, string message)
        {
            if (Konsole.ConsoleInstance != null)
            {
                switch (type)
                {
                    case LogType.Error when Konsole.ConsoleInstance.Theme.StatusError.HasValue:
                    case LogType.Exception when Konsole.ConsoleInstance.Theme.StatusError.HasValue:
                        return $"<color=#{ColorUtility.ToHtmlStringRGBA(Konsole.ConsoleInstance.Theme.StatusError.Value)}>{message}</color>";

                    case LogType.Assert when Konsole.ConsoleInstance.Theme.StatusWarning.HasValue:
                    case LogType.Warning when Konsole.ConsoleInstance.Theme.StatusWarning.HasValue:
                        return $"<color=#{ColorUtility.ToHtmlStringRGBA(Konsole.ConsoleInstance.Theme.StatusWarning.Value)}>{message}</color>";

                    case LogType.Log when Konsole.ConsoleInstance.Theme.StatusLog.HasValue:
                        return $"<color=#{ColorUtility.ToHtmlStringRGBA(Konsole.ConsoleInstance.Theme.StatusLog.Value)}>{message}</color>";

                    default:
                        Debug.LogError($"Type {type} not implemented in colors");
                        return message;
                }
            }

            return message;
        }

        public static string WithColor(string colorHex, string message) => $"<color={(colorHex.StartsWith("#") ? colorHex : ($"#{colorHex}"))}>{message}</color>";
        public static string WithColor(Color color, string message) => $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{message}</color>";

        public static string TextSize(int size, string message) => $"<size={size}>{message}</size>";
    }
}