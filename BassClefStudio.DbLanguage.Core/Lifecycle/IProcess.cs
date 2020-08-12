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
        ILibrary ExecutingLibrary { get; }
    }
}
