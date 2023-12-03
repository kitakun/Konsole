using UnityEngine;

namespace Bagheads.UnityConsole.Commands
{
    public class Command_Help : ICommand
    {
        public string Name => "help";
        public string Description => "Show all existing command and their description";

        public void Launch(CommandContext context)
        {
            var allCommands = Konsole.CommandsDictionary;
            if (!context.IsMultilineSupported)
            {
                context.Log($"All available commands (Count={TextTags.Bold(allCommands.Count.ToString())})");

                foreach (var pair in allCommands)
                {
                    var commandDescription = !string.IsNullOrEmpty(pair.Value.Description)
                        ? pair.Value.Description
                        : "No description for this command";
                    context.Log($" - {TextTags.WithColor("#30C000", pair.Value.Name)} : {commandDescription}");
                }
            }
            else
            {
                // TODO
                Debug.LogWarning($"Konsole.{nameof(Command_Help)} multiline not implemented yet.");
            }
        }
    }
}