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
    public class ScriptCommand : IAsyncCommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection Requiredcapabilities { get; }

        public ICommand GetScriptCommand { get; }
        public IEnumerable<ICommand> GetInputCommands { get; }

        public string ThreadName { get; set; }

        /// <summary>
        /// Creates a new script command, that creates a new <see cref="Thread"/> with a parent <see cref="DataObject"/>'s context and starts the thread on a specific <see cref="ICommand"/>.
        /// </summary>
        /// <param name="getScript">An <see cref="ICommand"/> instance that executes to return the script to run (often a <see cref="GetCommand"/> to retrieve items from memory.)</param>
        /// <param name="getInputs">A collection of <see cref="ICommand"/>s that execute in parallel to retrieve the inputs of the <see cref="Script"/>.</param>
        public ScriptCommand(ICommand getScript, IEnumerable<ICommand> getInputs)
        {
            GetScriptCommand = getScript;
            GetInputCommands = getInputs;
            Requiredcapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public async Task<DataObject> Execute(DataObject me, IWritableMemoryStack myStack, CapabilitiesCollection capabilities)
        {
            ////Gets the script (likely from memory).
            DataObject owningObject = await GetScriptCommand.ExecuteCommandAsync(me, myStack, capabilities);
            Script script = owningObject.GetObject<Script>();

            ////Resolves all inputs asynchronously and in parallel, often calling other scripts or getting values from memory (because scripts can be layered and asynchronous, the inputs are asynchronous).
            List<Task<DataObject>> inputTasks = new List<Task<DataObject>>();
            foreach (var getInput in GetInputCommands)
            {
                inputTasks.Add(getInput.ExecuteCommandAsync(me, myStack, capabilities));
            }

            List<DataObject> inputs = new List<DataObject>();
            foreach (var task in inputTasks)
            {
                inputs.Add(await task);
            }

            return await script.CallScript(inputs, capabilities);
        }
    }
}
