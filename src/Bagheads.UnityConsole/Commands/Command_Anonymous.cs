using System;

namespace Bagheads.UnityConsole.Commands
{
    internal sealed class Command_Anonymous : ICommand
    {
        public string Name { get; }
        public string Description { get; }
        private readonly Action<CommandContext> _action;

        public Command_Anonymous(string name, Action<CommandContext> action, string description = null)
        {
            Name = name;
            Description = description;
            _action = action;
        }
        
        public void Launch(CommandContext context)
        {
            _action(context);
        }
    }
}