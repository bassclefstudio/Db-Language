using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Scripts.Commands;
using BassClefStudio.DbLanguage.Core.Scripts.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Scripts.Threading
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
        /// A list of capabilities that this script has while running - only threads with required capabilities can run certain <see cref="ICommand"/> commands.
        /// </summary>
        public CapabilitiesCollection Capabilities { get; }

        /// <summary>
        /// The <see cref="CommandPointer"/> of a <see cref="Thread"/> is used to determine the next <see cref="ICommand"/> that should be run.
        /// </summary>
        public CommandPointer Pointer { get; private set; }

        /// <summary>
        /// The memory stack is used to store the context of the <see cref="DataObject"/> where the thread is running as well as thread memory and scopes.
        /// </summary>
        public IWritableMemoryStack MemoryStack { get; private set; }

        /// <summary>
        /// Creates a new thread with a name and <see cref="Scripts.capabilityCollection"/>.
        /// </summary>
        /// <param name="name">The name of the thread.</param>
        /// <param name="capabilities">A collection of capabilities that the <see cref="Thread"/> has to run certain <see cref="ICommand"/> commands.</param>
        public Thread(string name, CapabilitiesCollection capabilities, CommandPointer pointer)
        {
            Name = name;
            Capabilities = capabilities;
            Pointer = pointer;
        }

        /// <summary>
        /// Starts the execution of a thread at a specific <see cref="ICommand"/> with a memory context in the form of a <see cref="DataObject"/>.
        /// </summary>
        /// <param name="me">The owning <see cref="DataObject"/>, which executes the commands and provides bound .NET objects and <see cref="DataStructure"/> info. This <see cref="DataObject"/> also provides the memory/type context for this <see cref="Thread"/>.</param>
        public async Task RunThreadAsync(DataObject me)
            => await RunThreadAsync(me, me.MemoryStack, new MemoryGroup());

        /// <summary>
        /// Starts the execution of a thread at a specific <see cref="ICommand"/> with a memory context in the form of an <see cref="IMemoryStack"/>.
        /// </summary>
        /// <param name="me">The owning <see cref="DataObject"/>, which executes the commands and provides bound .NET objects and <see cref="DataStructure"/> info.</param>
        /// <param name="context">Memory context, such as types and public/private properties. This is often the same context as that found in the <see cref="DataObject.MemoryStack"/> of the owning <see cref="DataObject"/> <paramref name="me"/>.</param>
        public async Task RunThreadAsync(DataObject me, IMemoryStack context)
            => await RunThreadAsync(me, context, new MemoryGroup());

        /// <summary>
        /// Starts the execution of a thread at a specific <see cref="ICommand"/> with a memory context in the form of a <see cref="DataObject"/> and an input <see cref="IWritableMemoryGroup"/>.
        /// </summary>
        /// <param name="me">The owning <see cref="DataObject"/>, which executes the commands and provides bound .NET objects and <see cref="DataStructure"/> info. This <see cref="DataObject"/> also provides the memory/type context for this <see cref="Thread"/>.</param>
        /// <param name="inputs">Memory context in the form of local variables such as inputs, usually provided by the calling <see cref="ICommand"/> or its <see cref="DataObject"/>.</param>
        public async Task RunThreadAsync(DataObject me, IWritableMemoryGroup inputs)
            => await RunThreadAsync(me, me.MemoryStack, inputs);

        /// <summary>
        /// Starts the execution of a thread at a specific <see cref="ICommand"/> with a memory context in the form of an <see cref="IMemoryStack"/> and an input <see cref="IWritableMemoryGroup"/>.
        /// </summary>
        /// <param name="me">The owning <see cref="DataObject"/>, which executes the commands and provides bound .NET objects and <see cref="DataStructure"/> info.</param>
        /// <param name="context">Memory context, such as types and public/private properties. This is often the same context as that found in the <see cref="DataObject.MemoryStack"/> of the owning <see cref="DataObject"/> <paramref name="me"/>.</param>
        /// <param name="inputs">Memory context in the form of local variables such as inputs, usually provided by the calling <see cref="ICommand"/> or its <see cref="DataObject"/>.</param>
        public async Task RunThreadAsync(DataObject me, IMemoryStack context, IWritableMemoryGroup inputs)
        {
            ////Initializes memory.
            MemoryStack = new MemoryStack();
            MemoryStack.Push(context);
            MemoryStack.Push(inputs);

            ////Now, for example, I could add a new item to the memory knowing the top layer is writable:
            ////
            ////    Memory.Add(new MemoryItem("Test", [My_DataType]));
            ////
            ////I can also, of course, push and pull layers for different scopes (note: a script that pushes a scope should run on the same thred to preserve thread memory in this way).

            while (!Pointer.IsStopped)
            {
                if (Capabilities.CanAccess(Pointer.CurrentCommand.Requiredcapabilities))
                {
                    await Pointer.CurrentCommand.ExecuteCommandAsync(me, MemoryStack, Capabilities);
                    Pointer.Next();
                }
                else
                {
                    throw new CapabilityException($"Thread does not have the required capabilities to execute command. capabilities required:\r\n{string.Join(", ", Pointer.CurrentCommand.Requiredcapabilities.RequiredCapabilities.Select(p => p.ToString()))}");
                }
            }
        }
    }


    public class ThreadException : Exception
    {
        public ThreadException() { }
        public ThreadException(string message) : base(message) { }
        public ThreadException(string message, Exception inner) : base(message, inner) { }
    }
}
