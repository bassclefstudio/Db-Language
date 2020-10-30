using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Core;
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
    public interface ICommand : ICapable
    {
        /// <summary>
        /// Executes the command asynchronously, returning a <see cref="DataObject"/> representing the output or new context.
        /// </summary>
        /// <param name="thread">The owning <see cref="Thread"/> object, which manages the memory and <see cref="CapabilitiesCollection"/> for the <see cref="ICommand"/>.</param>
        /// <param name="me">The <see cref="DataObject"/> caller of the <see cref="ICommand"/>.</param>
        /// <param name="context">The current <see cref="IMemoryGroup"/> memory context at the point this <see cref="ICommand"/> is called.</param>
        Task<DataObject> ExecuteCommandAsync(Thread thread, DataObject me, IMemoryGroup context);
    }

    /// <summary>
    /// Represents a basic <see cref="ICommand"/> that requires no <see cref="Capability"/>s to execute.
    /// </summary>
    public abstract class BaseCommand : ICommand
    {
        /// <inheritdoc/>
        public abstract Task<DataObject> ExecuteCommandAsync(Thread thread, DataObject me, IMemoryGroup context);

        /// <inheritdoc/>
        public CapabilitiesCollection GetCapabilities()
        {
            return new CapabilitiesCollection();
        }
    }

    /// <summary>
    /// An <see cref="Exception"/> thrown on the unexpected termination or failure of an <see cref="ICommand"/>.
    /// </summary>
    [Serializable]
    public class CommandException : Exception
    {
        /// <inheritdoc/>
        public CommandException() { }
        /// <inheritdoc/>
        public CommandException(string message) : base(message) { }
        /// <inheritdoc/>
        public CommandException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected CommandException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}