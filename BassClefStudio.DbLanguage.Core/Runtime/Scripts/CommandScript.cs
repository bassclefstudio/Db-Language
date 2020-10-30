using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Commands;
using BassClefStudio.DbLanguage.Core.Runtime.Core;
using BassClefStudio.DbLanguage.Core.Runtime.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Runtime.Scripts
{
    /// <summary>
    /// A <see cref="Script"/> built on a collection of <see cref="ICommand"/>s that are run on a <see cref="Thread"/>.
    /// </summary>
    public class CommandScript : Script
    {
        /// <summary>
        /// A collection of <see cref="ICommand"/> objects that make up a <see cref="Script"/>. They are run in this order on the <see cref="Thread"/> when the <see cref="Script"/> is called.
        /// </summary>
        public IEnumerable<ICommand> Commands { get; }

        /// <summary>
        /// A collection of any user-defined <see cref="Capability"/> values required to execute this <see cref="Script"/>.
        /// </summary>
        public CapabilitiesCollection AdditionalCapabilities { get; }

        /// <summary>
        /// Creates a script inside of its parent <see cref="DataObject"/>, starting at a specific <see cref="ICommand"/> and containing script information and documentation.
        /// </summary>
        /// <param name="parent">The owning object of the script. This <see cref="DataObject"/> provides the memory context for the <see cref="Script"/>'s <see cref="Thread"/>.</param>
        /// <param name="info">Contains information about the name and inputs of the script.</param>
        /// <param name="commands">A collection of <see cref="ICommand"/> objects that make up a <see cref="Script"/>. They are run in this order on the <see cref="Thread"/> when the <see cref="Script"/> is called.</param>
        /// <param name="addedCapabilities">A collection of any user-defined <see cref="Capability"/> values required to execute this <see cref="Script"/>.</param>
        public CommandScript(DataObject parent, ScriptInfo info, IEnumerable<ICommand> commands, CapabilitiesCollection addedCapabilities = null) : base(parent, info)
        {
            Commands = commands;
            AdditionalCapabilities = addedCapabilities;
        }

        /// <inheritdoc/>
        protected override async Task<DataObject> RunScriptInternalAsync(IWritableMemoryGroup inputs, CapabilitiesCollection capabilities)
        {
            var thread = new Thread(
                ScriptInfo.GetUniqueId(),
                capabilities,
                new ThreadPointer(Commands.ToArray()),
                null);

            await thread.RunThreadAsync(ParentObject, inputs);

            //// TODO: Make sure that this is the correct way to get the output of a script; handle flags such as no return and exception.
            if (thread.Pointer.Flags.HasFlag(CommandPointerFlags.Returned))
            {
                throw new NotImplementedException();
                //return inputs.Get("return").Value;
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public override CapabilitiesCollection GetCapabilities()
        {
            if (AdditionalCapabilities != null)
            {
                return new CapabilitiesCollection(Commands.Select(c => c.GetCapabilities()).Concat(new CapabilitiesCollection[] { AdditionalCapabilities }));
            }
            else
            {
                return new CapabilitiesCollection(Commands.Select(c => c.GetCapabilities()));
            }
        }
    }
}
