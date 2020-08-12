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
    }
}
