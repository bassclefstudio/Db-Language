using Pidgin;
using System;
using System.Collections.Generic;
using System.IO;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace BassClefStudio.DbLanguage.Parser
{
    public class DbLanguageParser
    {
        public Parser<char, StringLibrary> LibraryParser { get; }

        #region Basic
        #region Symbols
        private readonly Parser<char, char> OpenBrace = Char('{');
        private readonly Parser<char, char> CloseBrace = Char('}');
        private readonly Parser<char, char> OpenBracket = Char('[');
        private readonly Parser<char, char> CloseBracket = Char(']');
        private readonly Parser<char, char> OpenParenthesis = Char('(');
        private readonly Parser<char, char> CloseParenthesis = Char(')');
        private readonly Parser<char, char> Dot = Char('.');
        private readonly Parser<char, char> Quote = Char('"');
        private readonly Parser<char, char> Colon = Char(':');
        #endregion
        #region Keywords
        private readonly Parser<char, string> Comment = String("//");
        private readonly Parser<char, string> Private = String("private");
        private readonly Parser<char, string> Public = String("public");
        private readonly Parser<char, string> Type = String("type");
        private readonly Parser<char, string> Contract = String("contract");
        #endregion
        #region Structures
        private Parser<char, string> String;
        private Parser<char, string> Path;
        private Parser<char, string> Name;
        private Parser<char, char> Block;

        private Parser<char, T> InBlock<T, U, V>(Parser<char, U> header, Parser<char, V> body, Func<U, V, T> func)
        {
            return Map((h, b) => func(h, b), header, Block.Then(body));
        }

        private void InitStructures()
        {
            Path = Map((a, b) => string.Concat(a, b), Letter, LetterOrDigit.Or(Dot).ManyString());
            Name = Map((a, b) => string.Concat(a, b), Letter, LetterOrDigit.ManyString());
            String = AnyCharExcept('"').ManyString().Between(Quote);
            Block = Any.Between(OpenBrace.Between(SkipWhitespaces), CloseBrace.Between(SkipWhitespaces));
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
        private Parser<char, StringProperty> Property;

        private void InitProperties()
        {
            // TODO: Initial value / script?
            Property =
                from v in OneOf(Try(Public.ThenReturn(true)), Try(Private.ThenReturn(false))).Optional().Between(SkipWhitespaces)
                from t in Path.Between(SkipWhitespaces)
                from n in Name
                select new StringProperty() { IsPublic = v.GetValueOrDefault(false), Type = t, Name = n };
        }

        #endregion
        #region Core

        private Parser<char, StringHeader> Header;
        private Parser<char, IEnumerable<StringProperty>> Body;
        private Parser<char, StringType> Class;

        private void InitLanguage()
        {
            Header =
                from t in OneOf(Try(Type.ThenReturn(true)), Contract.ThenReturn(false)).Between(SkipWhitespaces)
                from n in Name
                from d in Colon.Between(SkipWhitespaces).Then(Path.Many()).Optional()
                select new StringHeader() { Name = n, IsContract = t, Dependencies = d.GetValueOrDefault(new string[0]) };
            // TODO: Parse Body
            Body = null;
            Class = InBlock(Header, Body, (h, b) => new StringType() { Header = h, Properties = b });
        }

        #endregion
        #endregion

        public DbLanguageParser()
        {
            InitStructures();
            InitScripts();
            InitProperties();
            InitLanguage();
        }

        public StringLibrary CreateLibrary(TextReader textReader)
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
    }

    public class ParseException : Exception
    {
        public ParseException() { }
        public ParseException(string message) : base(message) { }
        public ParseException(string message, Exception inner) : base(message, inner) { }

    }
}
