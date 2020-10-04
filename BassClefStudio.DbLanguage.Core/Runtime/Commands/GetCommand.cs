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
    /// Represents a memory GET command that returns value from the <see cref="IWritableMemoryStack"/>.
    /// </summary>
    public class GetCommand : ICommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection RequiredCapabilities { get; }

        /// <summary>
        /// The path to the <see cref="DataObject"/> to be retrieved from memory.
        /// </summary>
        public string VarPath { get; }

        /// <summary>
        /// Creates a new memory GET command that gets a <see cref="DataObject"/> from a specified path in memory.
        /// </summary>
        /// <param name="path">The path to the <see cref="MemoryItem"/>.</param>
        public GetCommand(string path)
        {
            VarPath = path;
            RequiredCapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public async Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread)
        {
            return thread.MemoryStack.GetPath(VarPath).Value;
        }
    }
}
