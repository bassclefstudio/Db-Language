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
    /// Represents an abstract GET command which can retreive a property from another returned <see cref="DataObject"/>.
    /// </summary>
    public class GetOfCommand : ICommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection RequiredCapabilities { get; }

        /// <summary>
        /// An <see cref="ICommand"/> to retreive the initial object.
        /// </summary>
        public ICommand GetObjectCommand { get; }

        /// <summary>
        /// The relative path to the <see cref="MemoryItem"/> on the parent <see cref="DataObject"/>.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Creates a new abstract GET command that gets a <see cref="DataObject"/> from a specified path on another <see cref="DataObject"/>.
        /// </summary>
        /// <param name="getCommand">An <see cref="ICommand"/> to retreive the initial object.</param>
        /// <param name="path">The path to the <see cref="MemoryItem"/>.</param>
        public GetOfCommand(ICommand getCommand, string path)
        {
            GetObjectCommand = getCommand;
            Path = path;
            RequiredCapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public async Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread)
        {
            var o = await GetObjectCommand.ExecuteCommandAsync(me, thread);
            return o.GetPath(Path).Value;
        }
    }
}
