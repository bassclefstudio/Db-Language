using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Lifecycle
{
    /// <summary>
    /// Represents an application or running process that runs Db code.
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// The main <see cref="ILibrary"/> which the <see cref="IProcess"/> can use to start and end an <see cref="IProcess"/>.
        /// </summary>
        ILibrary ExecutingLibrary { get; }

        /// <summary>
        /// Gets the current <see cref="ProcessState"/> of this <see cref="IProcess"/>.
        /// </summary>
        ProcessState CurrentState { get; }
    }

    /// <summary>
    /// An enum representing the various stages in the lifecycle of an <see cref="IProcess"/>.
    /// </summary>
    public enum ProcessState
    {
        /// <summary>
        /// The <see cref="IProcess"/> is not running and has no active memory.
        /// </summary>
        Suspended = 0,

        /// <summary>
        /// The <see cref="IProcess"/> is currently being run and used.
        /// </summary>
        Running = 1,

        /// <summary>
        /// The <see cref="IProcess"/> is not running, but state data and memory is still saved.
        /// </summary>
        Paused = 2,

        /// <summary>
        /// The resources of the <see cref="IProcess"/> are being used or managed by another <see cref="IProcess"/>.
        /// </summary>
        Brokered = 3
    }
}