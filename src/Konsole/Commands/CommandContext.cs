using Konsole.Components;

using System.Collections.Generic;

namespace Konsole.Commands
{
    public sealed class CommandContext
    {
        public ICommand Caller { get; private set; }
        public KonsoleComponent Owner { get; }
        public IReadOnlyList<string> Parameters { get; private set; }

        public bool IsMultilineSupported => false;

        public CommandContext(KonsoleComponent owner)
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