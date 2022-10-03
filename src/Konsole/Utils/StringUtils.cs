using Konsole.Data;

using System;
using System.Collections.Generic;
using System.Text;

namespace Konsole.Utils
{
    internal static class StringUtils
    {
        private static readonly StringBuilder Builder = new();
        private static readonly List<string> ClosingTags = new(4);
        private static readonly List<string> ArgumentList = new(4);

        internal static string Build(string format, IReadOnlyList<string> parameters, bool logTime)
        {
            Builder.Clear();
            if (logTime)
            {
                Builder.AppendFormat("[{0:T}] ", DateTime.Now);
            }

            Builder.Append(format);
            for (var i = 0; i < parameters.Count; i++)
            {
                Builder.Replace($"{{{i}}}", parameters[i]);
            }

            return Builder.ToString();
        }

        internal static string BuildRich(string format, IReadOnlyList<string> parameters, List<RichParameter> richParameters, bool logTime)
        {
            ClosingTags.Clear();
            Builder.Clear();
            if (logTime)
            {
                Builder.AppendFormat("[{0:T}] ", DateTime.Now);
            }

            Builder.Append(format);

            

            for (var i = 0; i < parameters.Count; i++)
            {
                var key = $"{{{i}}}";
                var keyLength = key.Length;
                var offset = 0;

                do
                {
                    var startIndexToInsert = Builder.IndexOf(key);
                    if (startIndexToInsert >= 0)
                    {
                        // remove {0} or {1} ... {N}
                        Builder.Remove(startIndexToInsert, keyLength);

                        var currentParameter = richParameters.Count > i
                            ? richParameters[i]
                            : default;

                        if (currentParameter.IsBold)
                        {
                            Builder.Insert(startIndexToInsert, TextTags.BoldTag);
                            offset += TextTags.BoldTag.Length;
                            ClosingTags.Add(TextTags.BoldCloseTag);
                        }

                        Builder.Insert(startIndexToInsert + offset, parameters[i]);
                        offset += parameters[i].Length;
                    }

                    if (ClosingTags.Count > 0)
                    {
                        for (var j = 0; j < ClosingTags.Count; j++)
                        {
                            var closingTag = ClosingTags[j];
                            Builder.Insert(startIndexToInsert + offset, closingTag);
                            offset += closingTag.Length;
                        }

                        ClosingTags.Clear();
                    }
                } while (Builder.IndexOf(key) >= 0);
            }

            return Builder.ToString();
        }

        internal static IReadOnlyList<string> GetParametersFromCommand(string data)
        {
            ArgumentList.Clear();

            var spanString = data.AsSpan();
            var isCommand = true;
            var protector = 50;

            do
            {
                protector--;
                if (protector < 0)
                    break;

                var spaceIndex = spanString.IndexOf(' ');
                if (spaceIndex < 0 && isCommand)
                    break;

                if (spaceIndex < 0 && !isCommand)
                {
                    ArgumentList.Add(spanString.ToString());
                    break;
                }

                if (isCommand)
                {
                    spanString = spanString[(spaceIndex + 1)..];

                    isCommand = false;
                    continue;
                }

                var argumentSpan = spanString[..spaceIndex];
                ArgumentList.Add(argumentSpan.ToString());

                spanString = spanString[(spaceIndex + 1)..];
            } while (true);

            return ArgumentList;
        }
    }
}