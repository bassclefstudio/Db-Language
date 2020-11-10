using BassClefStudio.DbLanguage.Core.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Compiler.Parse.Projects
{
    /// <summary>
    /// An <see cref="IParsePipeline{T}"/> that parses a package from an <see cref="IProjectReference"/> containing references to files for types and metadata in a Db project.
    /// </summary>
    public class ProjectParsePipeline : IParsePipeline<IProjectReference>
    {
        /// <summary>
        /// Represents the parser loaded for this <see cref="ProjectParsePipeline"/> to parse code files.
        /// </summary>
        public ITypeParseService TypeService { get; }

        /// <summary>
        /// Represents the parser loaded for this <see cref="ProjectParsePipeline"/> to parse project metadata files.
        /// </summary>
        public IMetadataParseService MetadataService { get; }

        /// <summary>
        /// Creates a new <see cref="ProjectParsePipeline"/>.
        /// </summary>
        /// <param name="typeService">Represents the parser loaded for this <see cref="ProjectParsePipeline"/> to parse code files.</param>
        /// <param name="metadataService">Represents the parser loaded for this <see cref="ProjectParsePipeline"/> to parse project metadata files.</param>
        public ProjectParsePipeline(ITypeParseService typeService, IMetadataParseService metadataService)
        {
            TypeService = typeService;
            MetadataService = metadataService;
        }

        /// <inheritdoc/>
        public async Task<TokenPackage> ParsePackageAsync(IProjectReference packageInput)
        {
            List<TokenType> types = new List<TokenType>();
            foreach (var code in await packageInput.GetCodeFilesAsync())
            {
                types.Add(TypeService.ParseType(code));
            }
            PackageInfo info = MetadataService.ParseProject(await packageInput.GetProjectFileAsync());
            return new TokenPackage(info, types);
        }
    }
}
