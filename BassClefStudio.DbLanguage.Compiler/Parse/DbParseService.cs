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
    internal class DbParseService : IParseService
    {
        private Parser<char, StringPackage> LibraryParser { get; }

        #region Basic
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
        private readonly Parser<char, string> Private = String("private");
        private readonly Parser<char, string> Public = String("public");
        private readonly Parser<char, string> Type = String("type");
        private readonly Parser<char, string> Contract = String("contract");
        private readonly Parser<char, string> Var = String("var");
        private readonly Parser<char, string> Return = String("return");
        #endregion
        #region Structures
        private Parser<char, string> String;
        private Parser<char, string> Path;
        private Parser<char, string> Name;
        
        private Parser<char, T> Block<T>(Parser<char, T> inner)
        {
            return inner.Between(OpenBrace.Between(SkipWhitespaces), CloseBrace.Between(SkipWhitespaces));
        }

        private void InitStructures()
        {
            Path =
                from a in Letter
                from b in LetterOrDigit.Or(Dot).ManyString()
                select string.Concat(a, b);
            Name =
                from a in Letter
                from b in LetterOrDigit.ManyString()
                select string.Concat(a, b);
            String = AnyCharExcept('"').ManyString().Between(Quote);
        }

        #endregion
        #endregion
        #region Language
        #region Scripts

        private void InitScripts()
        {

        }

        #endregion
        #region Properties
        private Parser<char, StringChild> Property;

        private void InitProperties()
        {
            // TODO: Initial value?
            Property =
                from v in OneOf(Try(Public.ThenReturn(true)), Try(Private.ThenReturn(false))).Before(Whitespace.AtLeastOnce()).Optional()
                from t in Path.Before(Whitespace.AtLeastOnce())
                from n in Name.Before(SemiColon)
                select new StringProperty() { IsPublic = v.GetValueOrDefault(false), Type = t, Name = n } as StringChild;
        }

        #endregion
        #region Core

        private Parser<char, StringTypeHeader> Header;
        private Parser<char, IEnumerable<StringChild>> Body;
        private Parser<char, StringType> Class;

        private void InitLanguage()
        {
            Header =
                from t in OneOf(Try(Type.ThenReturn(true)), Contract.ThenReturn(false)).Before(Whitespace.AtLeastOnce())
                from n in Name
                from d in Try(Colon.Between(SkipWhitespaces).Then(Path.SeparatedAtLeastOnce(Comma.Then(SkipWhitespaces)))).Optional()
                select new StringTypeHeader() { Name = n, IsConcrete = t, Dependencies = d.GetValueOrDefault(new string[0]) };
            
            //// Add OneOf(Try(Property), Script) when scripts are ready.
            Body = OneOf(Try(Property)).Before(SkipWhitespaces).Many();

            Class =
                from h in Header
                from b in Block(Body)
                select new StringType() { Header = h, Properties = b };
        }

        #endregion
        #endregion

        /// <summary>
        /// Creates a new <see cref="DbParseService"/>, initializing the language parsing frameworks.
        /// </summary>
        public DbParseService()
        {
            InitStructures();
            InitScripts();
            InitProperties();
            InitLanguage();
        }

        /// <inheritdoc/>
        public StringPackage ParseLibrary(string code) => ParseLibrary(new StringReader(code));
        /// <inheritdoc/>
        public StringPackage ParseLibrary(TextReader textReader)
        {
            var result = LibraryParser.Parse(textReader);
            if (result.Success)
            {
                return result.Value;
            }
            else
            {
                throw new ParseException(result.Error.RenderErrorMessage());
            }
        }

        /// <summary>
        /// Parses the code for a Db class (a type or contract) into a <see cref="StringType"/>.
        /// </summary>
        /// <param name="code">The <see cref="string"/> code.</param>
        public StringType ParseClass(string code) => ParseClass(new StringReader(code));
        /// <summary>
        /// Parses the code for a Db class (a type or contract) into a <see cref="StringType"/>.
        /// </summary>
        /// <param name="textReader">A <see cref="TextReader"/> that can read a block of code.</param>
        public StringType ParseClass(TextReader textReader)
        {
            var result = Class.Parse(textReader);
            if (result.Success)
            {
                return result.Value;
            }
            else
            {
                throw new ParseException(result.Error.RenderErrorMessage());
            }
        }
    }

    /// <summary>
    /// Represents an <see cref="Exception"/> thrown during <see cref="string"/>-to-token parsing.
    /// </summary>
    public class ParseException : Exception
    {
        internal ParseException() { }
        internal ParseException(string message) : base(message) { }
        internal ParseException(string message, Exception inner) : base(message, inner) { }

    }

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
