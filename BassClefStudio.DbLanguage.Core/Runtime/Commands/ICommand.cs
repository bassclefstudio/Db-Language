using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Info;
using BassClefStudio.DbLanguage.Core.Runtime.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Runtime.Commands
{
    /// <summary>
    /// Represents a piece of code that has hard-coded native inputs when constructed. The most basic operation the Db runtime can evaluate. <see cref="ICommand"/>s are run directly on a <see cref="Thread"/>.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="me">The current <see cref="DataObject"/> context at the point this <see cref="ICommand"/> is called.</param>
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

        /// <inheritdoc/>
        public CommandException(string message, DataObject exceptionObject  = null) : base(message) => ExceptionObject = exceptionObject;
        /// <inheritdoc/>
        public CommandException(string message, Exception innerException, DataObject exceptionObject = null) : base(message, innerException) => ExceptionObject = exceptionObject;
    }
}
