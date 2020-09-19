using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Lifecycle;
using BassClefStudio.DbLanguage.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Build
{
    /// <summary>
    /// Represents an <see cref="ILibrary"/> that can be built from a stream by the parser/compiler services.
    /// </summary>
    public class CompilerLibrary : ILibrary
    {
        /// <inheritdoc/>
        public Namespace Name { get; set; }

        /// <inheritdoc/>
        public IEnumerable<ILibrary> DependentLibraries { get; set; }

        /// <inheritdoc/>
        public IEnumerable<IType> Definitions { get; set; }

        /// <inheritdoc/>
        public IMemoryGroup ManagedContext { get; private set; }

        /// <summary>
        /// Builds the <see cref="ManagedContext"/> object once all other properties have been resolved by the compiler.
        /// </summary>
        public void BuildContext()
        {
            throw new NotImplementedException();
        }
    }
}
