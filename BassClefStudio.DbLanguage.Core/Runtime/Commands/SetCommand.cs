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
    /// Represents a memory SET command that writes a value to the <see cref="IWritableMemoryStack"/>.
    /// </summary>
    public class SetCommand : ICommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection RequiredCapabilities { get; }

        /// <summary>
        /// The path to the <see cref="DataObject"/> to be retrieved from memory.
        /// </summary>
        public string VarPath { get; }

        /// <summary>
        /// The <see cref="ICommand"/> value which retreives the value to set the item in memory. Must match the <see cref="MemoryProperty.Type"/> of the item.
        /// </summary>
        public ICommand Value { get; }

        /// <summary>
        /// Creates a new memory SET command that sets the <see cref="MemoryItem.Value"/> of an item in memory.
        /// </summary>
        /// <param name="path">The path to the <see cref="MemoryItem"/>.</param>
        /// <param name="value">The <see cref="ICommand"/> value which retreives the value to set.</param>
        public SetCommand(string path, ICommand value)
        {
            VarPath = path;
            Value = value;
            RequiredCapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public async Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread)
        {
            thread.MemoryStack.SetPath(VarPath, await Value.ExecuteCommandAsync(me, thread));
            return null;
        }
    }
}
