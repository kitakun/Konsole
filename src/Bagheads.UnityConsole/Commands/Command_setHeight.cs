using Bagheads.UnityConsole.Components;

namespace Bagheads.UnityConsole.Commands
{
    public class Command_setHeight : ICommand
    {
        public string Name => "set_height";
        public string Description => "Set console height (default: 300. percents could be used: 100%)";

        public void Launch(CommandContext context)
        {
            if (context.Parameters.Count == 1)
            {
                var rawNewSize = context.Parameters[0];
                if (rawNewSize.EndsWith("%"))
                {
                    var rawNewSizeSliced = rawNewSize.Remove(rawNewSize.Length - 1, 1);
                    if (int.TryParse(rawNewSizeSliced, out var percentValue))
                    {
                        if (context.Owner.TryGetInternalComponent<PreventFromEditor.ControlContainerHeight>(out var heightController))
                        {
                            heightController.SetHeightPercent(percentValue);
                        }
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
                        if (context.Owner.TryGetInternalComponent<PreventFromEditor.ControlContainerHeight>(out var heightController))
                        {
                            heightController.SetHeight(pixels);
                        }
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