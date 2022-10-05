using Bagheads.UnityConsole.Components;

using System.Collections.Generic;

namespace Bagheads.UnityConsole.Commands
{
    public sealed class CommandContext
    {
        public ICommand Caller { get; private set; }
        public PreventFromEditor.KonsoleComponent Owner { get; }
        public IReadOnlyList<string> Parameters { get; private set; }

        public bool IsMultilineSupported => false;

        public CommandContext(PreventFromEditor.KonsoleComponent owner)
        {
            Owner = owner;
        }

        public void Set(ICommand caller, IReadOnlyList<string> parameters)
        {
            Caller = caller;
            Parameters = parameters;
        }

        public void Log(string message)
        {
            if (Owner != null)
            {
                Owner.CommandResponse(Caller, message);
            }
        }
    }
}