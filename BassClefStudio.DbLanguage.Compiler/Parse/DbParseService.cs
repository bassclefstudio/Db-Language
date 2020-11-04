using Pidgin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace BassClefStudio.DbLanguage.Compiler.Parse
{
    /// <summary>
    /// A parser based on the <see cref="Pidgin.Parser"/> framework that converts <see cref="string"/> code in the Db language to a tokenized <see cref="StringPackage"/>. Supports recursion and complex commands across the full Db language, along with exception support.
    /// </summary>
    internal class DbParseService
    {
        //private Parser<char, StringPackage> LibraryParser { get; }

        #region Symbols

        private readonly Parser<char, char> OpenBrace = Char('{');
        private readonly Parser<char, char> CloseBrace = Char('}');
        private readonly Parser<char, char> OpenBracket = Char('[');
        private readonly Parser<char, char> CloseBracket = Char(']');
        private readonly Parser<char, char> OpenParenthesis = Char('(');
        private readonly Parser<char, char> CloseParenthesis = Char(')');
        private readonly Parser<char, char> Dot = Char('.');
        private readonly Parser<char, char> Comma = Char(',');
        private readonly Parser<char, char> Quote = Char('"');
        private readonly Parser<char, char> Colon = Char(':');
        private readonly Parser<char, char> SemiColon = Char(';');
        private readonly Parser<char, char> Equal = Char('=');

        #endregion
        #region Keywords

        private readonly Parser<char, string> Comment = String("//");

        #endregion
    }

    /// <summary>
    /// Represents an <see cref="Exception"/> thrown during <see cref="string"/>-to-token parsing.
    /// </summary>
    public class ParseException : Exception
    {
        /// <inheritdoc/>
        internal ParseException() { }
        /// <inheritdoc/>
        internal ParseException(string message) : base(message) { }
        /// <inheritdoc/>
        internal ParseException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Represents extension methods for the <see cref="Pidgin.Parser"/> and related types.
    /// </summary>
    internal static class ParseExtensions
    {
        /// <summary>
        /// Creates a parser that runs the current parser and returns <typeparamref name="T3"/> as implemented interface or type <typeparamref name="T2"/>.
        /// </summary>
        public static Parser<T1, T2> As<T1, T2, T3>(this Parser<T1, T3> parser) where T3 : T2
        {
            return parser.Select<T2>(p => p);
        }
    }
}
