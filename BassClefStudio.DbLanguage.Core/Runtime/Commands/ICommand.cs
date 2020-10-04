using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Core;
using BassClefStudio.DbLanguage.Core.Runtime.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Runtime.Commands
{
    /// <summary>
    /// Represents a piece of code that has hard-coded native inputs when constructed. The most basic operation the Db runtime can evaluate. <see cref="ICommand"/>s are run directly on a <see cref="Thread"/>.
    /// </summary>
    public interface ICommand : ICapable
    {
        /// <summary>
        /// Executes the command asynchronously.
        /// </summary>
        /// <param name="me">The current <see cref="DataObject"/> context at the point this <see cref="ICommand"/> is called.</param>
        /// <param name="thread">The owning <see cref="Thread"/> object, which manages the memory and <see cref="CapabilitiesCollection"/> for the <see cref="ICommand"/>.</param>
        Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread);
    }
}
