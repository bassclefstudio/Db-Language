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
    /// Represents a RETURN command which adds a returned object to the <see cref="IWritableMemoryStack"/> and sets <see cref="CommandPointerFlags.Returned"/>.
    /// </summary>
    public class ReturnCommand : ICommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection RequiredCapabilities { get; }

        public ICommand GetCommand { get; }

        /// <summary>
        /// Creates a new RETURN command that sets flags on the running <see cref="ThreadPointer"/> and returns the value from the given <see cref="ICommand"/>.
        /// </summary>
        /// <param name="getCommand">An <see cref="ICommand"/> returning the value </param>
        public ReturnCommand(ICommand getCommand)
        {
            GetCommand = getCommand;
            RequiredCapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public async Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread)
        {
            DataObject value = await GetCommand.ExecuteCommandAsync(me, thread);
            thread.MemoryStack.Add(
                new MemoryItem(
                    new MemoryProperty(
                        "return",
                        value.DataType)));
            thread.MemoryStack.Set("return", value);
            thread.Pointer.AddFlags(CommandPointerFlags.Returned);
            return null;
        }
    }
}
