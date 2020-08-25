using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Scripts.Info;
using BassClefStudio.DbLanguage.Core.Scripts.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Scripts.Commands
{
    /// <summary>
    /// Represents a THROW command which throws a <see cref="CommandException"/> with an attached Db exception.
    /// </summary>
    public class ExceptionCommand : ICommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection RequiredCapabilities { get; }

        /// <summary>
        /// An <see cref="ICommand"/> which provides the exception <see cref="DataObject"/>.
        /// </summary>
        public ICommand ExceptionInfo { get; }

        /// <summary>
        /// Creates a new THROW command from the given Db exception.
        /// </summary>
        /// <param name="getExceptionInfo">An <see cref="ICommand"/> which provides the exception <see cref="DataObject"/>.</param>
        public ExceptionCommand(ICommand getExceptionInfo)
        {
            ExceptionInfo = getExceptionInfo;
            RequiredCapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public async Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread)
        {
            DataObject exObject;
            try
            {
                exObject = await ExceptionInfo.ExecuteCommandAsync(me, thread);
            }
            catch(Exception ex)
            {
                throw new CommandException($"The thread {thread.Name} threw a Db exception at runtime but the exception object could not be resolved", ex);
            }

            throw new CommandException($"The thread {thread.Name} threw a Db exception at runtime.", exObject);
        }
    }
}
