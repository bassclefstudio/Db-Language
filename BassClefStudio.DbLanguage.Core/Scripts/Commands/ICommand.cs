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

        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="me">The owning <see cref="DataObject"/> making the call to the <see cref="ICommand"/>.</param>
        /// <param name="thread">The owning <see cref="Thread"/> object, which manages the memory and <see cref="CapabilitiesCollection"/> for the <see cref="ICommand"/>.</param>
        Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread);
    }

    /// <summary>
    /// Represents an <see cref="Exception"/> thrown from within the Db language, by an <see cref="ICommand"/>.
    /// </summary>
    public class CommandException : Exception
    {
        /// <summary>
        /// The Db <see cref="DataObject"/> provided as the exception information object.
        /// </summary>
        public DataObject ExceptionObject { get; }

        public CommandException(string message, DataObject exceptionObject  = null) : base(message) => ExceptionObject = exceptionObject;
        public CommandException(string message, Exception innerException, DataObject exceptionObject = null) : base(message, innerException) => ExceptionObject = exceptionObject;
    }
}
