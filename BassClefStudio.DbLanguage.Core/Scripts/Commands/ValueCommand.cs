using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Scripts.Info;
using BassClefStudio.DbLanguage.Core.Scripts.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Scripts.Commands
{
    /// <summary>
    /// Represents an <see cref="ICommand"/> which returns a given value when provided with the owning <see cref="DataObject"/>.
    /// </summary>
    public class ValueCommand : ICommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection RequiredCapabilities { get; }

        /// <summary>
        /// A <see cref="Func{T, TResult}"/> that takes the calling <see cref="DataObject"/> and returns a <see cref="DataObject"/> value.
        /// </summary>
        public Func<DataObject, DataObject> ValueFunc { get; }

        /// <summary>
        /// Creates a new <see cref="ValueCommand"/>.
        /// </summary>
        /// <param name="valueFunc">A <see cref="Func{T, TResult}"/> that takes the calling <see cref="DataObject"/> and returns a <see cref="DataObject"/> value.</param>
        public ValueCommand(Func<DataObject, DataObject> valueFunc)
        {
            ValueFunc = valueFunc;
            RequiredCapabilities = new CapabilitiesCollection();
        }

        /// <inheritdoc/>
        public async Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread)
        {
            return ValueFunc(me);
        }
    }
}
