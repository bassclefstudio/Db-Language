using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Scripts.Info;
using BassClefStudio.DbLanguage.Core.Scripts.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Scripts.Commands
{
    /// <summary>
    /// Represents one command that a <see cref="Thread"/> will run.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// A <see cref="CapabilitiesCollection"/> which contains all of the reuqired <see cref="Capability"/> objects a <see cref="Thread"/>'s <see cref="CapabilitiesCollection"/> object must have in order to run the command.
        /// </summary>
        CapabilitiesCollection RequiredCapabilities { get; }
    }

    public interface IActionCommand : ICommand
    { 
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="me">The owning <see cref="DataObject"/> making the call to the <see cref="ICommand"/>.</param>
        /// <param name="thread">The owning <see cref="Thread"/> object, which manages the memory and <see cref="CapabilitiesCollection"/> for the <see cref="ICommand"/>.</param>
        DataObject Execute(DataObject me, Thread thread);
    }

    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="me">The owning <see cref="DataObject"/> making the call to the <see cref="ICommand"/>.</param>
        /// <param name="thread">The owning <see cref="Thread"/> object, which manages the memory and <see cref="CapabilitiesCollection"/> for the <see cref="ICommand"/>.</param>
        Task<DataObject> Execute(DataObject me, Thread thread);
    }

    public class CommandException : Exception
    {
        public CommandException() { }
        public CommandException(string message) : base(message) { }
        public CommandException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Provides extension methods that support running different types of <see cref="ICommand"/> instances through one method.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Executes an <see cref="ICommand"/> asynchronously.
        /// </summary>
        /// <param name="me">The owning <see cref="DataObject"/> making the call to the <see cref="ICommand"/>.</param>
        /// <param name="thread">The owning <see cref="Thread"/> object, which manages the memory and <see cref="CapabilitiesCollection"/> for the <see cref="ICommand"/>.</param>
        public static async Task<DataObject> ExecuteCommandAsync(this ICommand command, DataObject me, Thread thread)
        {
            if (command is IActionCommand actionCommand)
            {
                return actionCommand.Execute(me, thread);
            }
            else if (command is IAsyncCommand asyncCommand)
            {
                return await asyncCommand.Execute(me, thread);
            }
            else
            {
                throw new CommandException($"Cannot execute commands of type {command?.GetType()}");
            }
        }
    }
}
