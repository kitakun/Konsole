namespace Bagheads.UnityConsole.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description how to use this command
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Run this command
        /// </summary>
        /// <param name="context">caller context</param>
        void Launch(CommandContext context);
    }
}