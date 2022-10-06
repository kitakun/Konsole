using UnityEngine;

namespace Bagheads.UnityConsole.Commands
{
    public class Command_HelloWorld : ICommand
    {
        public string Name => "hello";
        public string Description => "Console testing command. Do nothing important.";

        public void Launch(CommandContext context)
        {
            var textResponse = context.Parameters is {Count: > 0}
                ? $"{TextTags.Bold("world")} with params ={string.Join(",", context.Parameters)}"
                : $"{TextTags.Bold("world")} with {TextTags.Italian("no params")}";

            context.Log(textResponse);
        }
    }
}