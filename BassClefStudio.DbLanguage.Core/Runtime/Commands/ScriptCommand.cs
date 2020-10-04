using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Info;
using BassClefStudio.DbLanguage.Core.Runtime.Scripts;
using BassClefStudio.DbLanguage.Core.Runtime.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Runtime.Commands
{
    public class ScriptCommand : ICommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection RequiredCapabilities { get; }

        public ICommand GetScriptCommand { get; }
        public IEnumerable<ICommand> GetInputCommands { get; }

        /// <summary>
        /// Creates a new script command, that creates a new <see cref="Thread"/> with a parent <see cref="DataObject"/>'s context and starts the thread on a specific <see cref="ICommand"/>.
        /// </summary>
        /// <param name="getScript">An <see cref="ICommand"/> instance that executes to return the script to run (often a <see cref="GetCommand"/> to retrieve items from memory.)</param>
        /// <param name="getInputs">A collection of <see cref="ICommand"/>s that execute in parallel to retrieve the inputs of the <see cref="Script"/>.</param>
        public ScriptCommand(ICommand getScript, IEnumerable<ICommand> getInputs)
        {
            GetScriptCommand = getScript;
            GetInputCommands = getInputs;
            RequiredCapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public async Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread)
        {
            ////Gets the script (likely from memory).
            DataObject owningObject = await GetScriptCommand.ExecuteCommandAsync(me, thread);
            Script script = owningObject.GetObject<Script>();

            ////Resolves all inputs asynchronously and in parallel, often calling other scripts or getting values from memory (because scripts can be layered and asynchronous, the inputs are asynchronous).
            List<Task<DataObject>> inputTasks = new List<Task<DataObject>>();
            foreach (var getInput in GetInputCommands)
            {
                inputTasks.Add(getInput.ExecuteCommandAsync(me, thread));
            }

            List<DataObject> inputs = new List<DataObject>();
            foreach (var task in inputTasks)
            {
                inputs.Add(await task);
            }

            return await script.CallScriptAsync(inputs, thread.Capabilities);
        }
    }
}
