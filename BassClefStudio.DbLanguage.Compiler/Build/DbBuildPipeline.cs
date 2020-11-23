using BassClefStudio.DbLanguage.Compiler.Parse;
using BassClefStudio.DbLanguage.Core.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Compiler.Build
{
    /// <summary>
    /// Represents the default <see cref="IBuildPipeline"/> for the Db language.
    /// </summary>
    public class DbBuildPipeline : IBuildPipeline
    {
        /// <inheritdoc/>
        public Task<IPackage> BuildPackageAsync(TokenPackage packageInput)
        {
            throw new NotImplementedException();
        }
    }
}
