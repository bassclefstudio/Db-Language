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
        /// <param name="myStack">The stack of memory available to the command, including the ability to create script memory in the topmost layer.</param>
        /// <param name="pointer">The pointer that controls movement to each <see cref="ICommand"/>, This allows for a <see cref="ICommand"/> to change at runtime which <see cref="ICommand"/> follows it. This parameter should only be included when the command is being run by a <see cref="Thread"/>.</param>
        /// <param name="capabilities">The capabilities of the thread that called the <see cref="ICommand"/> - a script cannot declare more capabilities than what the calling object passes to it.</param>
        DataObject Execute(DataObject me, IWritableMemoryStack myStack, CapabilitiesCollection capabilities);
    }

    public interface IAsyncCommand : ICommand
    {
        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="myStack">The stack of memory available to the command, including the ability to create script memory in the topmost layer.</param>
        /// <param name="pointer">The pointer that controls movement to each <see cref="ICommand"/>, This allows for a <see cref="ICommand"/> to change at runtime which <see cref="ICommand"/> follows it. This parameter should only be included when the command is being run by a <see cref="Thread"/>.</param>
        /// <param name="capabilities">The capabilities of the thread that called the <see cref="ICommand"/> - a script cannot declare more capabilities than what the calling object passes to it.</param>
        Task<DataObject> Execute(DataObject me, IWritableMemoryStack myStack, CapabilitiesCollection capabilities);
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
        public static async Task<DataObject> ExecuteCommandAsync(this ICommand command, DataObject me, IWritableMemoryStack myStack, CapabilitiesCollection capabilities)
        {
            if (command is IActionCommand actionCommand)
            {
                return actionCommand.Execute(me, myStack, capabilities);
            }
            else if (command is IAsyncCommand asyncCommand)
            {
                return await asyncCommand.Execute(me, myStack, capabilities);
            }
            else
            {
                throw new CommandException($"Cannot execute commands of type {command?.GetType()}");
            }
        }
    }
}
