using BassClefStudio.DbLanguage.Compiler.Parse;
using BassClefStudio.DbLanguage.Core.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Build
{
    /// <summary>
    /// Represents a service that builds an <see cref="ILibrary"/> from the given <see cref="StringLibrary"/> tokenized form.
    /// </summary>
    internal interface IBuildService
    {
        /// <summary>
        /// Builds the tokenized library, applying type-checking, assigning values, and dynamically building memory and scripts.
        /// </summary>
        /// <param name="stringLib">A <see cref="StringLibrary"/> containing the tokenized form of the code.</param>
        ILibrary Build(StringLibrary stringLib);
    }
}
