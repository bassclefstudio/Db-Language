using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Compiler.Parse.Projects
{
    /// <summary>
    /// Represents a reference to the relevant files in a Db project containing code and metadata to be parsed.
    /// </summary>
    public interface IProjectReference
    {
        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of <see cref="TextReader"/> objects representing the relevant code files to parse.
        /// </summary>
        /// <returns>A collection of <see cref="TextReader"/>s for each code file.</returns>
        Task<IEnumerable<TextReader>> GetCodeFilesAsync();

        /// <summary>
        /// Returns a <see cref="TextReader"/> representing the project metadata file to parse for the project.
        /// </summary>
        /// <returns>A <see cref="TextReader"/> for the project file.</returns>
        Task<TextReader> GetProjectFileAsync();
    }
}
