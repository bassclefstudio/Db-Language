using BassClefStudio.DbLanguage.Core.Documentation;
using BassClefStudio.DbLanguage.Core.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Data
{
    /// <summary>
    /// Represents any construct in the Db language that can be cast to, and thus can be set as the value of <see cref="MemoryProperty.Type"/>.
    /// </summary>
    public interface IType
    {
        /// <summary>
        /// The full name of the <see cref="IType"/> object.
        /// </summary>
        Namespace TypeName { get; }

        /// <summary>
        /// Returns a <see cref="bool"/> value indicating whether an object of this <see cref="IType"/> may be cast as the given <see cref="IType"/> <paramref name="other"/>.
        /// </summary>
        /// <param name="other">The <see cref="IType"/> to check.</param>
        bool Is(IType other);
    }
}
