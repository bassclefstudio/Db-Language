﻿using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Commands;
using BassClefStudio.DbLanguage.Core.Runtime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Runtime.Threading
{
    /// <summary>
    /// Represents a thread in the Db runtime, which is a dynamic series of <see cref="ICommand"/> objects that are executed by the runtime.
    /// </summary>
    public class Thread
    {
        /// <summary>
        /// The friendly name of the thread.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A list of capabilities that this script has while running - only threads with required capabilities can run certain code.
        /// </summary>
        public CapabilitiesCollection Capabilities { get; }

        /// <summary>
        /// The <see cref="ThreadPointer"/> of a <see cref="Thread"/> is used to manage the <see cref="ICommand"/>s that should be run, returning, and managing exceptions.
        /// </summary>
        public ThreadPointer Pointer { get; private set; }

        /// <summary>
        /// Memory context, such as static type information, specific to the environment where the <see cref="Thread"/> is executing.
        /// </summary>
        public IMemoryGroup Context { get; }

        /// <summary>
        /// Creates a new <see cref="Thread"/>
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="capabilities">A collection of capabilities that the <see cref="Thread"/> has to run certain <see cref="ICommand"/> commands.</param>
        /// <param name="pointer">The <see cref="ThreadPointer"/> contains information about the <see cref="ICommand"/>s that will be run on the <see cref="Thread"/>.</param>
        /// <param name="context">Memory context, such as static type information, specific to the environment where the <see cref="Thread"/> is executing.</param>
        public Thread(string name, CapabilitiesCollection capabilities, ThreadPointer pointer, IMemoryGroup context)
        {
            Name = name;
            Capabilities = capabilities;
            Pointer = pointer;
            Context = context;
        }

        /// <summary>
        /// Creates a new thread as a child of another <see cref="Thread"/>. 
        /// </summary>
        /// <param name="name">The (optional) name of the thread. Defaults to the name of the parent "<see cref="Thread.Name"/>_{#}"</param>
        /// <param name="pointer">The <see cref="ThreadPointer"/> contains information about the <see cref="ICommand"/>s that will be run on the <see cref="Thread"/>.</param>
        /// <param name="parent">The parent <see cref="Thread"/> to create this <see cref="Thread"/> instance from. The <see cref="Thread.Context"/>, <see cref="Thread.Capabilities"/>, and other related data will be copied from the parent.</param>
        public Thread(Thread parent, ThreadPointer pointer, string name = null)
        {
            Name = name ?? $"{parent.Name}_{Guid.NewGuid()}";
            Capabilities = parent.Capabilities;
            Pointer = pointer;
            Context = parent.Context;
        }

        /// <summary>
        /// Starts the execution of a thread at a specific <see cref="ICommand"/> with a memory context in the form of an <see cref="IMemoryStack"/> and an input <see cref="IWritableMemoryGroup"/>.
        /// </summary>
        /// <param name="me">The owning <see cref="DataObject"/>, which executes the commands and provides bound .NET objects.</param>
        /// <param name="inputs">Memory context in the form of local variables such as inputs or scoped variables.</param>
        /// <returns>A <see cref="DataObject"/> representing the context at the point the <see cref="Thread"/> completed execution (this value may be null).</returns>
        public async Task<DataObject> RunThreadAsync(DataObject me, IMemoryGroup inputs)
        {
            ////Initializes memory.
            var defaultContext = new MemoryStack();
            defaultContext.Push(Context);
            defaultContext.Push(me.MemoryStack);
            defaultContext.Push(inputs);
            defaultContext.Push();

            IMemoryGroup commandContext = defaultContext;
            while (!Pointer.IsStopped)
            {
                IMemoryGroup newContext = await Pointer.CurrentCommand.ExecuteCommandAsync(this, me, commandContext);
                commandContext = newContext ?? defaultContext;
                Pointer.Next();
            }
            return commandContext as DataObject;
        }
    }
}
