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
    /// Represents a memory GET command that returns value from the <see cref="IWritableMemoryStack"/>.
    /// </summary>
    public class GetCommand : IActionCommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection Requiredcapabilities { get; }

        /// <summary>
        /// The path to the <see cref="DataObject"/> to be retrieved from memory.
        /// </summary>
        public string VarPath { get; }

        /// <summary>
        /// Creates a new memory GET command that gets a <see cref="DataObject"/> from a specified path memory.
        /// </summary>
        /// <param name="path">The path to the <see cref="MemoryItem"/>.</param>
        public GetCommand(string path)
        {
            VarPath = path;
            Requiredcapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public DataObject Execute(DataObject me, IWritableMemoryStack myStack, CapabilitiesCollection capabilities)
        {
            //// TODO: Come back and fix this!!
            ////return myStack.GetPath(VarPath).Value;
            throw new NotImplementedException();
        }
    }
}
