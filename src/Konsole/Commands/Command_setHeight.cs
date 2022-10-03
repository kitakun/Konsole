using UnityEngine;

namespace Konsole.Commands
{
    public class Command_setHeight : ICommand
    {
        public string Name => "set_height";
        public string Description => "Set console height (default: 300. percents could be used: 100%)";

        public void Launch(CommandContext context)
        {
            if (context.Parameters.Count == 1)
            {
                var ownerTransform = context.Owner.transform as RectTransform;
                var rawNewSize = context.Parameters[0];
                if (rawNewSize.EndsWith("%"))
                {
                    var rawNewSizeSliced = rawNewSize.Remove(rawNewSize.Length - 1, 1);
                    if (int.TryParse(rawNewSizeSliced, out var pixels))
                    {
                        var percentVal = Screen.safeArea.height;
                        ownerTransform.sizeDelta = new Vector2(0, Mathf.Min(Mathf.Max(pixels, percentVal), 100));
                    }
                    else
                    {
                        context.Log($"Can't get number from \"{TextTags.Bold(rawNewSizeSliced)}\"");
                    }
                }
                else
                {
                    // direct
                    if (int.TryParse(rawNewSize, out var pixels))
                    {
                        ownerTransform.sizeDelta = new Vector2(0, Mathf.Max(pixels, 100));
                    }
                    else
                    {
                        context.Log($"Can't get number from \"{TextTags.Bold(rawNewSize)}\"");
                    }
                }

                return;
            }

            context.Log($"Please, provide new value like \"{TextTags.Bold("set_height 400")}\"");
        }
    }
}