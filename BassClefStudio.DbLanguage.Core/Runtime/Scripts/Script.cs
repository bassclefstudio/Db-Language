using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Commands;
using BassClefStudio.DbLanguage.Core.Runtime.Info;
using BassClefStudio.DbLanguage.Core.Runtime.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Runtime.Scripts
{
    /// <summary>
    /// Represents a method that takes dynamic <see cref="DataObject"/>s of specified types as input, does a task, and returns a <see cref="DataObject"/> result.
    /// </summary>
    public abstract class Script
    {
        /// <summary>
        /// The <see cref="DataObject"/> that this <see cref="Script"/>, functioning as the <see cref="Script"/>'s context.
        /// </summary>
        public DataObject ParentObject { get; }

        /// <summary>
        /// Contains information and documentation about the script, including its name and inputs.
        /// </summary>
        public ScriptInfo ScriptInfo { get; }

        /// <summary>
        /// Creates a new <see cref="Script"/>.
        /// </summary>
        /// <param name="parent">The <see cref="DataObject"/> that this <see cref="Script"/>, functioning as the <see cref="Script"/>'s context.</param>
        /// <param name="info">Contains information and documentation about the script, including its name and inputs.</param>
        protected Script(DataObject parent, ScriptInfo info)
        {
            ParentObject = parent;
            ScriptInfo = info;
        }

        /// <summary>
        /// Calls and executes a given <see cref="Script"/> and returns the <see cref="DataObject"/> result.
        /// </summary>
        /// <param name="inputs">A collection of <see cref="DataObject"/>s which will be checked and stored in an <see cref="IWritableMemoryGroup"/> to be sent to the <see cref="Thread"/>'s <see cref="IMemoryStack"/>.</param>
        /// <param name="inheritedcapabilities">The <see cref="CapabilitiesCollection"/> from the <see cref="ICommand"/> or other source that called the <see cref="Script"/>.</param>
        public async Task<DataObject> CallScriptAsync(IEnumerable<DataObject> inputs, CapabilitiesCollection inheritedcapabilities)
        {
            IWritableMemoryGroup inputMemory = ScriptInfo.CreateMemoryFromInputs(inputs.ToArray());
            return await RunScriptInternalAsync(inputMemory, inheritedcapabilities);
        }

        /// <summary>
        /// Internal - runs the <see cref="Script"/> with the provided memory context and returns a <see cref="DataObject"/> result.
        /// </summary>
        /// <param name="inputs">An <see cref="IWritableMemoryGroup"/> containing the named <see cref="Script"/> inputs.</param>
        /// <param name="capabilities">The <see cref="CapabilitiesCollection"/> from the <see cref="ICommand"/> or other source that called the <see cref="Script"/>.</param>
        protected abstract Task<DataObject> RunScriptInternalAsync(IWritableMemoryGroup inputs, CapabilitiesCollection capabilities);
    }
}
