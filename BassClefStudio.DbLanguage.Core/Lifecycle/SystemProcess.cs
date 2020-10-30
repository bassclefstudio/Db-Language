using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Lifecycle
{
    /// <summary>
    /// A task being run by an <see cref="ISystem"/>.
    /// </summary>
    public class SystemProcess : Observable
    {
        /// <summary>
        /// A <see cref="PackageInfo"/> object providing information about the <see cref="IPackage"/> running this <see cref="SystemProcess"/>.
        /// </summary>
        public PackageInfo Package { get; }

        private ProcessStatus status;
        /// <summary>
        /// A <see cref="ProcessStatus"/> value indicating the running status of this <see cref="SystemProcess"/>.
        /// </summary>
        public ProcessStatus Status { get => status; set => Set(ref status, value); }
    }

    /// <summary>
    /// An enum representing the status of a <see cref="SystemProcess"/>.
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>
        /// The <see cref="SystemProcess"/> is normally running and code is being executed by the <see cref="IPackage"/>.
        /// </summary>
        Running = 0,

        /// <summary>
        /// The <see cref="SystemProcess"/> has been temporarily suspended to save resources. No code is runnig on the <see cref="IPackage"/>.
        /// </summary>
        Suspended = 1,

        /// <summary>
        /// The <see cref="SystemProcess"/> is running, but another <see cref="IPackage"/> is using the code/resources on the given <see cref="IPackage"/>.
        /// </summary>
        Brokered = 2
    }
}
