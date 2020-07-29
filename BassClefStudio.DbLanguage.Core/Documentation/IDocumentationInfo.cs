using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Core.Documentation
{
    /// <summary>
    /// Represents a group of human-readable information for developers or users regarding a specific component in a Db project.
    /// </summary>
    public interface IDocumentationInfo
    {
        /// <summary>
        /// The unique <see cref="Documentation.Namespace"/> of the object being documented, which can be used to find that object.
        /// </summary>
        Namespace Namespace { get; }

        /// <summary>
        /// Returns the human-readable documentation for this <see cref="IDocumentable"/> in Markdown format (see https://commonmark.org/help/ for more information).
        /// </summary>
        string GetMarkdownDocumentation();

        /// <summary>
        /// Returns the human-readable documentation for this <see cref="IDocumentable"/> but in plain text. For a formatted version of the documentation, see <seealso cref="GetMarkdownDocumentation"/>.
        /// </summary>
        string GetPlainTextDocumentation();
    }
}
