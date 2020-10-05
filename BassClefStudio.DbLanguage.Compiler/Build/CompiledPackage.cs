using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Build
{
    /// <summary>
    /// A basic <see cref="IPackage"/> implementation for building compiled <see cref="IPackage"/>s.
    /// </summary>
    public class CompiledPackage : IPackage
    {
        /// <inheritdoc/>
        public IEnumerable<IType> DefinedTypes { get; set; }

        /// <inheritdoc/>
        public Namespace Name { get; }

        /// <summary>
        /// Creates a new <see cref="CompiledPackage"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="IPackage"/>.</param>
        public CompiledPackage(Namespace name)
        {
            Name = name;
        }
    }
}
