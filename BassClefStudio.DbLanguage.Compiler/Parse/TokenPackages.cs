using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Parse
{
    /// <summary>
    /// Represents the location in a document that a particular token was retrieved from.
    /// </summary>
    public class TokenPos
    {
        /// <summary>
        /// The number of the line in the document.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// The column, or character in the line, of the location in the document.
        /// </summary>
        public int ColumnNumber { get; }

        /// <summary>
        /// Creates a new <see cref="TokenPos"/>.
        /// </summary>
        /// <param name="lineNumber">The number of the line in the document.</param>
        /// <param name="columnNumber">The column, or character in the line, of the location in the document.</param>
        public TokenPos(int lineNumber, int columnNumber)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }

        /// <summary>
        /// Creates a new <see cref="TokenPos"/>.
        /// </summary>
        /// <param name="sourcePos">A <see cref="Pidgin.SourcePos"/> collected from a <see cref="Pidgin.Parser"/> that represents the position in the document.</param>
        public TokenPos(Pidgin.SourcePos sourcePos)
        {
            LineNumber = sourcePos.Line;
            ColumnNumber = sourcePos.Col;
        }
    }

    /// <summary>
    /// Represents any named item in the Db code model.
    /// </summary>
    public abstract class TokenChild
    {
        /// <summary>
        /// The <see cref="string"/> name of the <see cref="TokenChild"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="TokenChild"/> should be publicly accessible.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// An optional <see cref="TokenPos"/> representing the location in the source code where the text representing this <see cref="TokenChild"/> occurs.
        /// </summary>
        public TokenPos SourcePosition { get; set; }
    }

    /// <summary>
    /// Represents a <see cref="TokenChild"/> header for a type.
    /// </summary>
    public class TokenTypeHeader : TokenChild
    {
        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="TokenType"/> this header is attached to should be treated as a type (class) or contract (interface).
        /// </summary>
        public bool IsConcrete { get; set; }

        /// <summary>
        /// A list of <see cref="string"/> names of <see cref="TokenType"/>s this type inherits from.
        /// </summary>
        public IEnumerable<string> InheritsFrom { get; set; }
    }

    /// <summary>
    /// Represents a tokenized type or contract.
    /// </summary>
    public class TokenType
    {
        /// <summary>
        /// The <see cref="TokenTypeHeader"/> containing metadata about the type.
        /// </summary>
        public TokenTypeHeader Header { get; set; }

        /// <summary>
        /// The body of the <see cref="TokenType"/>, containing a number of <see cref="TokenChild"/>s (traditionally properties or methods).
        /// </summary>
        public IEnumerable<TokenChild> Children { get; set; }
    }

    /// <summary>
    /// Represents a tokenized property definition for a <see cref="TokenType"/>.
    /// </summary>
    public class TokenProperty : TokenChild
    {
        /// <summary>
        /// The <see cref="string"/> name of the type of the value stored in this property.
        /// </summary>
        public string ValueType { get; set; }
    }

    /// <summary>
    /// Represents a tokenized method definition for a <see cref="TokenType"/>.
    /// </summary>
    public class TokenScript : TokenChild
    {
        /// <summary>
        /// The <see cref="string"/> name of the type this <see cref="TokenScript"/> returns.
        /// </summary>
        public string ReturnType { get; set; }
    }
}
