namespace Konsole.Commands
{
    public class Command_Clear : ICommand
    {
        public string Name => "clear";
        public string Description => "Clear console";

        public void Launch(CommandContext context)
        {
            context.Owner.Clear();
        }
    }
}