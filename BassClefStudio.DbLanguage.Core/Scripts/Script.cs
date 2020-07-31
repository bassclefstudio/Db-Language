using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Scripts.Commands;
using BassClefStudio.DbLanguage.Core.Scripts.Info;
using BassClefStudio.DbLanguage.Core.Scripts.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Scripts
{
    /// <summary>
    /// Represents a script, a collection of <see cref="ICommand"/>s with information on the type and number of inputs, documentation, and a starting point for the <see cref="Thread"/>.
    /// </summary>
    public class Script
    {
        /// <summary>
        /// The <see cref="DataObject"/> that provides the memory context for the script's <see cref="Thread"/> objects.
        /// </summary>
        public DataObject ParentObject { get; }

        /// <summary>
        /// Contains information and documentation about the script, including its name and inputs.
        /// </summary>
        public ScriptInfo ScriptInfo { get; }

        /// <summary>
        /// A collection of <see cref="ICommand"/> objects that make up a <see cref="Script"/>. They are run in this order on the <see cref="Thread"/> when the <see cref="Script"/> is called.
        /// </summary>
        public IEnumerable<ICommand> Commands { get; }

        /// <summary>
        /// Creates a script inside of its parent <see cref="DataObject"/>, starting at a specific <see cref="ICommand"/> and containing script information and documentation.
        /// </summary>
        /// <param name="parent">The owning object of the script. This <see cref="DataObject"/> provides the memory context for the <see cref="Script"/>'s <see cref="Thread"/>.</param>
        /// <param name="info">Contains information about the name and inputs of the script.</param>
        public Script(DataObject parent, ScriptInfo info, IEnumerable<ICommand> commands)
        {
            ParentObject = parent;
            ScriptInfo = info;
            Commands = commands;
        }

        /// <summary>
        /// Calls and executes a given <see cref="Script"/> on a new <see cref="Thread"/> and returns the <see cref="Thread"/>'s output object (if applicable).
        /// </summary>
        /// <param name="inputs">A collection of <see cref="DataObject"/>s which will be checked and stored in an <see cref="IWritableMemoryGroup"/> to be sent to the <see cref="Thread"/>'s <see cref="IMemoryStack"/>.</param>
        /// <param name="inheritedcapabilities">The <see cref="CapabilitiesCollection"/> from the <see cref="ICommand"/> or other source that called the <see cref="Script"/>, which will be used for the <see cref="CapabilitiesCollection"/> of child <see cref="Thread"/>s.</param>
        public async Task<DataObject> CallScript(IEnumerable<DataObject> inputs, CapabilitiesCollection inheritedcapabilities)
        {
            var thread = new Thread(
                ScriptInfo.GetUniqueId(),
                inheritedcapabilities,
                new ThreadPointer(Commands.ToArray()),
                null);
            //// TODO: Add context

            IWritableMemoryGroup inputMemory = ScriptInfo.CreateMemoryFromInputs(inputs.ToArray());
            await thread.RunThreadAsync(ParentObject, inputMemory);

            //// TODO: Make sure that this is the correct way to get the output of a script; handle flags such as no return and exception.
            return inputMemory.Get("return").Value;
        }
    }
}
