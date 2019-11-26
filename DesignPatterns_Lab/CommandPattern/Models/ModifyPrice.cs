namespace CommandPattern.Models
{
    using System.Collections.Generic;
    using Contracts;

    public class ModifyPrice
    {
        private readonly List<ICommand> commands;
        private ICommand command;

        public ModifyPrice()
        {
            commands = new List<ICommand>();
        }

        public void SetCommand(ICommand command) => this.command = command;

        public void Invoke()
        {
            commands.Add(this.command);
            command.ExecuteAction();
        }
    }
}