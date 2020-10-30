using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Core;
using BassClefStudio.DbLanguage.Core.Runtime.Scripts;
using BassClefStudio.DbLanguage.Core.Runtime.Threading;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Runtime.Commands
{
    /// <summary>
    /// An <see cref="ICommand"/> that returns the calling <see cref="DataObject"/> ('me' or 'this').
    /// </summary>
    public class ThisCommand : BaseCommand
    {
        /// <summary>
        /// Creates a new <see cref="ThisCommand"/>.
        /// </summary>
        public ThisCommand()
        { }

        /// <inheritdoc/>
        public override async Task<DataObject> ExecuteCommandAsync(Thread thread, DataObject me, IMemoryGroup context) 
        {
            return me;
        }
    }

    /// <summary>
    /// An <see cref="ICommand"/> that returns the value located in the specified <see cref="Property"/> of the context.
    /// </summary>
    public class GetCommand : BaseCommand
    {
        /// <summary>
        /// The <see cref="MemoryProperty"/> where the desired <see cref="MemoryItem"/> is located.
        /// </summary>
        public MemoryProperty Property { get; }

        /// <summary>
        /// Creates a new <see cref="GetCommand"/>.
        /// </summary>
        /// <param name="property">The <see cref="MemoryProperty"/> where the desired <see cref="MemoryItem"/> is located.</param>
        public GetCommand(MemoryProperty property)
        {
            Property = property;
        }

        /// <inheritdoc/>
        public override async Task<DataObject> ExecuteCommandAsync(Thread thread, DataObject me, IMemoryGroup context) 
        {
            return context.Get(Property).Value;
        }
    }

    /// <summary>
    /// An <see cref="ICommand"/> that sets the value located in the specified <see cref="Property"/> of the context to a given <see cref="Value"/>.
    /// </summary>
    public class SetCommand : BaseCommand
    {
        /// <summary>
        /// The <see cref="MemoryProperty"/> where the desired <see cref="MemoryItem"/> is located.
        /// </summary>
        public MemoryProperty Property { get; }

        /// <summary>
        /// An <see cref="ICommand"/> that returns the desired <see cref="DataObject"/> value to store in memory.
        /// </summary>
        public ICommand Value { get; }

        /// <summary>
        /// Creates a new <see cref="GetCommand"/>.
        /// </summary>
        /// <param name="property">The <see cref="MemoryProperty"/> where the desired <see cref="MemoryItem"/> is located.</param>
        /// <param name="value">An <see cref="ICommand"/> that returns the desired <see cref="DataObject"/> value to store in memory.</param>
        public SetCommand(MemoryProperty property, ICommand value)
        {
            Property = property;
            Value = value;
        }

        /// <inheritdoc/>
        public override async Task<DataObject> ExecuteCommandAsync(Thread thread, DataObject me, IMemoryGroup context) 
        {
            context.Set(Property, await Value.ExecuteCommandAsync(thread, me, null));
            return null;
        }
    }

    /// <summary>
    /// An <see cref="ICommand"/> that executes the current context as a <see cref="Script"/>.
    /// </summary>
    public class ExecuteCommand : BaseCommand
    {
        /// <summary>
        /// A collection of <see cref="ICommand"/>s that return the inputs to the <see cref="Script"/>.
        /// </summary>
        public IEnumerable<ICommand> Inputs { get; }

        /// <summary>
        /// Creates a new <see cref="ExecuteCommand"/>.
        /// </summary>
        /// <param name="inputs">A collection of <see cref="ICommand"/>s that return the inputs to the <see cref="Script"/>.</param>
        public ExecuteCommand(IEnumerable<ICommand> inputs)
        {
            Inputs = inputs;
        }

        /// <inheritdoc/>
        public override async Task<DataObject> ExecuteCommandAsync(Thread thread, DataObject me, IMemoryGroup context) 
        {
            if (context is DataObject scriptObject)
            {
                IEnumerable<DataObject> inputValues = await Inputs.Select(i => i.ExecuteCommandAsync(thread, me, null)).RunParallelAsync();
                return await scriptObject.GetObject<Script>().CallScriptAsync(inputValues, thread.Capabilities);
            }
            else
            {
                throw new CommandException("Cannot call the ExecuteCommand on a context that is not a DataObject (i.e. the base Thread context).");
            }
        }
    }
}
