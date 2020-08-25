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
    public class ReturnCommand : ICommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection RequiredCapabilities { get; }

        public ICommand GetCommand { get; }

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
