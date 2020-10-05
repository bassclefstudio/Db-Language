using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Memory;
using BassClefStudio.DbLanguage.Core.Runtime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Lifecycle
{
    /// <summary>
    /// Represents a container of <see cref="IType"/>s and the information describing how these types connect to other <see cref="IPackage"/>s and the system.
    /// </summary>
    public interface IPackage
    {
        /// <summary>
        /// A collection of <see cref="IType"/> objects described in this <see cref="IPackage"/>.
        /// </summary>
        IEnumerable<IType> DefinedTypes { get; }

        /// <summary>
        /// The name path for the given <see cref="IPackage"/>.
        /// </summary>
        Namespace Name { get; }
    }

    /// <summary>
    /// An extension class for the <see cref="IPackage"/> interface.
    /// </summary>
    public static class PackageExtensions
    {
    }
}
