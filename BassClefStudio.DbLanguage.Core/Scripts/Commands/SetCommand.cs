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
    /// Represents a memory SET command that writes a value to the <see cref="IWritableMemoryStack"/>.
    /// </summary>
    public class SetCommand : IAsyncCommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection Requiredcapabilities { get; }

        /// <summary>
        /// The path to the <see cref="DataObject"/> to be retrieved from memory.
        /// </summary>
        public string VarPath { get; }

        /// <summary>
        /// The <see cref="DataObject"/> value to set the item in memory. Must match the <see cref="MemoryItem.Type"/>
        /// </summary>
        public Func<Task<DataObject>> Value { get; }

        /// <summary>
        /// Creates a new memory SET command that sets the <see cref="MemoryItem.Value"/> of an item in memory.
        /// </summary>
        /// <param name="path">The path to the <see cref="MemoryItem"/>.</param>
        /// <param name="value">The <see cref="DataObject"/> value to set.</param>
        public SetCommand(string path, Func<Task<DataObject>> value)
        {
            VarPath = path;
            Value = value;
            Requiredcapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public async Task<DataObject> Execute(DataObject me, IWritableMemoryStack myStack, CapabilitiesCollection capabilities)
        {
            //// TODO: Come back and fix this!!
            //myStack.SetPath(VarPath, await Value());
            return null;
        }
    }
}
