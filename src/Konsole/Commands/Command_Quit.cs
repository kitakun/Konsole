using UnityEngine;

namespace Konsole.Commands
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