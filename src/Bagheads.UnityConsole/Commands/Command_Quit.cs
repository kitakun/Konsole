using UnityEngine;

namespace Bagheads.UnityConsole.Commands
{
    public class Command_Quit : ICommand
    {
        public string Name => "quit";
        public string Description => "Will quit from the Application";
        
        public void Launch(CommandContext context)
        {
            Application.Quit();
        }
    }
}