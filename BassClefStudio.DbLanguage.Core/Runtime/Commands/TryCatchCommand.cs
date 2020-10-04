using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Runtime.Info;
using BassClefStudio.DbLanguage.Core.Runtime.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Core.Runtime.Commands
{
    /// <summary>
    /// Represents a TRY...CATCH command that can run an <see cref="ICommand"/> should a given <see cref="ICommand"/> throw an exception.
    /// </summary>
    public class TryCatchCommand : ICommand
    {
        /// <inheritdoc/>
        public CapabilitiesCollection RequiredCapabilities { get; }

        /// <summary>
        /// The <see cref="ICommand"/> that is initially run.
        /// </summary>
        public ICommand TryCommand { get; }

        /// <summary>
        /// An <see cref="ICommand"/> that is run if <see cref="TryCommand"/> throws an exception.
        /// </summary>
        public ICommand CatchCommand { get; }

        /// <summary>
        /// Creates a new TRY command with a try and catch <see cref="ICommand"/>.
        /// </summary>
        /// <param name="tryCommand">The <see cref="ICommand"/> that is initially run.</param>
        /// <param name="catchCommand">An <see cref="ICommand"/> that is run if <see cref="TryCommand"/> throws an exception.</param>
        public TryCatchCommand(ICommand tryCommand, ICommand catchCommand)
        {
            TryCommand = tryCommand;
            CatchCommand = catchCommand;
        }

        /// <inheritdoc/>
        public async Task<DataObject> ExecuteCommandAsync(DataObject me, Thread thread)
        {
            try
            {
                await TryCommand.ExecuteCommandAsync(me, thread);
                return null;
            }
            catch (CommandException ex)
            {
                Debug.WriteLine($"Exeption caught: {ex}");
                await CatchCommand.ExecuteCommandAsync(me, thread);
                return null;
            }
        }
    }
}
