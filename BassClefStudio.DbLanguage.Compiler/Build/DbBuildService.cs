using BassClefStudio.DbLanguage.Compiler.Parse;
using BassClefStudio.DbLanguage.Core.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Build
{
    /// <summary>
    /// An <see cref="IBuildService"/> for building, type-checking, compiling, and creating <see cref="ILibrary"/> instances from <see cref="StringLibrary"/> objects created by a relevant <see cref="IParseService"/>.
    /// </summary>
    internal class DbBuildService : IBuildService
    {
        /// <inheritdoc/>
        public ILibrary Build(StringLibrary stringLib)
        {
            throw new NotImplementedException();
        }
    }
}
