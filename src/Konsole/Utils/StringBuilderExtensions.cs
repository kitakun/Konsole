using System.Text;

using static System.Char;

namespace Konsole.Utils
{
    internal static class StringBuilderExtensions
    {
        internal static int IndexOf(this StringBuilder sb, string value, int startIndex = 0, bool ignoreCase = true)
        {
            int index;
            var length = value.Length;
            var maxSearchLength = (sb.Length - length) + 1;

            if (ignoreCase)
            {
                for (var i = startIndex; i < maxSearchLength; ++i)
                {
                    if (ToLower(sb[i]) == ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (ToLower(sb[i + index]) == ToLower(value[index])))
                        {
                            ++index;
                        }

                        if (index == length)
                        {
                            return i;
                        }
                    }
                }

                return -1;
            }

            for (var i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                    {
                        ++index;
                    }

                    if (index == length)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}