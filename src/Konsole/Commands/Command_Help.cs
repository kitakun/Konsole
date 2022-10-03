using UnityEngine;

namespace Konsole.Commands
{
    public class Command_Help : ICommand
    {
        public string Name => "help";
        public string Description => "Show all existing command and their description";

        public void Launch(CommandContext context)
        {
            var allCommands = Konsole.CommandsList;
            if (!context.IsMultilineSupported)
            {
                context.Log($"All available commands (Count={TextTags.Bold(allCommands.Count.ToString())})");

                for (var i = 0; i < allCommands.Count; i++)
                {
                    var commandDescription = !string.IsNullOrEmpty(allCommands[i].Description)
                        ? allCommands[i].Description
                        : "No description for this command";
                    context.Log($" - {TextTags.WithColor("#30C000", allCommands[i].Name)} : {commandDescription}");
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