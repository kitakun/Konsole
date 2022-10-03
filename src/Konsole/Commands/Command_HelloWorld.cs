using UnityEngine;

namespace Konsole.Commands
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

            Debug.Log($"Default lot");
            Debug.LogWarning($"Warning log");
            Debug.LogError($"Error log");
            
            context.Log(textResponse);
        }
    }
}