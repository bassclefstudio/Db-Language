using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Parse
{
    /// <summary>
    /// Represents a service that can take a stream of <see cref="char"/>s (or a <see cref="string"/>) of code and output <see cref="StringLibrary"/> objects representing the tokenized code.
    /// </summary>
    internal interface IParseService
    {
        /// <summary>
        /// Parses the code for a library (collection of types) into a <see cref="StringLibrary"/>.
        /// </summary>
        /// <param name="code">The <see cref="string"/> code.</param>
        StringLibrary ParseLibrary(string code);

        /// <summary>
        /// Parses the code for a library (collection of types) into a <see cref="StringLibrary"/>.
        /// </summary>
        /// <param name="textReader">A <see cref="TextReader"/> that can read a block of code.</param>
        StringLibrary ParseLibrary(TextReader textReader);
    }
}
