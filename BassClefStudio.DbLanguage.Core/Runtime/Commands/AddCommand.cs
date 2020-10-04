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
    /// Represents a memory ADD command that adds a new item to the <see cref="IWritableMemoryStack"/>.
    /// </summary>
    public class AddCommand : ICommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection RequiredCapabilities { get; }

        /// <summary>
        /// The name of the variable to be created in memory.
        /// </summary>
        public string VarName { get; }

        /// <summary>
        /// The <see cref="DataType"/> of the variable. All <see cref="DataObject"/> items in this memory location must inherit from this type. (see <seealso cref="DataType.Is(DataType)"/>)
        /// </summary>
        public DataType VarType { get; }

        /// <summary>
        /// Creates a new memory ADD command that adds a new <see cref="MemoryItem"/> to the <see cref="Thread"/>'s <see cref="IWritableMemoryStack"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MemoryItem"/>.</param>
        /// <param name="varType">The <see cref="DataType"/> of the <see cref="MemoryItem"/>.</param>
        public AddCommand(string name, DataType varType)
        {
            VarName = name;
            VarType = varType;
            RequiredCapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public async Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread)
        {
            thread.MemoryStack.Add(new MemoryItem(new MemoryProperty(VarName, VarType)));
            return null;
        }
    }
}
