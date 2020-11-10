using BassClefStudio.DbLanguage.Core.Runtime.Commands;
using BassClefStudio.DbLanguage.Core.Runtime.Scripts;
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
    /// A parser based on the <see cref="Pidgin.Parser"/> framework that converts <see cref="string"/> code in the Db language to a tokenized output. Supports recursion and complex commands across the full Db language, along with exception support.
    /// </summary>
    internal class PidginTypeParseService : ITypeParseService
    {
        private Parser<char, TokenType> TypeParser { get; set; }

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
        private readonly Parser<char, string> This = String("this");
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

        private Parser<char, TokenAccessible> Script;
        private Parser<char, TokenScriptInput> ScriptInput;
        private Parser<char, TokenCommand> Command;

        private void InitScripts()
        {
            //// TODO: Add all commands to parser.
            Command = OneOf(
                CurrentPos.Before(This).Select<TokenCommand>(p => new ThisTokenCommand() { SourcePosition = new TokenPos(p) }),
                Map((p, n) => new PathTokenCommand() { Path = n, SourcePosition = new TokenPos(p) } as TokenCommand, CurrentPos, Dot.Optional().Then(Name)),
                Map((p, c) => new ExecuteTokenCommand() { SourcePosition = new TokenPos(p), Inputs = c } as TokenCommand, CurrentPos, Rec(() => Command).Separated(Comma).Between(OpenParenthesis, CloseParenthesis)),
                CurrentPos.Before(Equal).Select<TokenCommand>(p => new EqualTokenCommand() { SourcePosition = new TokenPos(p) }),
                CurrentPos.Before(SemiColon).Select<TokenCommand>(p => new EndLineTokenCommand() { SourcePosition = new TokenPos(p) }),
                Map((p, v) => new LiteralTokenCommand() { SourcePosition = new TokenPos(p), Value = v } as TokenCommand, CurrentPos, String));

            ScriptInput =
                from p in CurrentPos
                from t in Path.Before(Whitespace.AtLeastOnce())
                from n in Name
                select new TokenScriptInput() { SourcePosition = new TokenPos(p), Type = t, Name = n };

            Script =
                from p in CurrentPos
                from v in OneOf(Try(Public.ThenReturn(true)), Try(Private.ThenReturn(false))).Before(Whitespace.AtLeastOnce()).Optional()
                from t in Path.Before(Whitespace.AtLeastOnce())
                from n in Name
                from i in ScriptInput.Separated(Comma.Between(SkipWhitespaces)).Between(OpenParenthesis, CloseParenthesis)
                from c in Block(Command.SeparatedAndTerminated(SkipWhitespaces))
                select new TokenScript() { SourcePosition = new TokenPos(p), IsPublic = v.GetValueOrDefault(false), ReturnType = t, Name = n, Inputs = i, Commands = c } as TokenAccessible;
        }

        #endregion
        #region Properties

        private Parser<char, TokenAccessible> Property;

        private void InitProperties()
        {
            //// TODO: Initial value?
            Property =
                from p in CurrentPos
                from v in OneOf(Try(Public.ThenReturn(true)), Try(Private.ThenReturn(false))).Before(Whitespace.AtLeastOnce()).Optional()
                from t in Path.Before(Whitespace.AtLeastOnce())
                from n in Name.Before(SemiColon)
                select new TokenProperty() { SourcePosition = new TokenPos(p), IsPublic = v.GetValueOrDefault(false), ValueType = t, Name = n } as TokenAccessible;
        }

        #endregion
        #region Core

        private Parser<char, TokenTypeHeader> Header;
        private Parser<char, IEnumerable<TokenAccessible>> Body;

        private void InitLanguage()
        {
            Header =
                from p in CurrentPos
                from v in OneOf(Try(Public.ThenReturn(true)), Try(Private.ThenReturn(false))).Before(Whitespace.AtLeastOnce()).Optional()
                from t in OneOf(Try(Type.ThenReturn(true)), Contract.ThenReturn(false)).Before(Whitespace.AtLeastOnce())
                from n in Name
                from d in Try(Colon.Between(SkipWhitespaces).Then(Path.SeparatedAtLeastOnce(Comma.Then(SkipWhitespaces)))).Optional()
                select new TokenTypeHeader() { SourcePosition = new TokenPos(p), Name = n, IsPublic = v.GetValueOrDefault(false), IsConcrete = t, InheritsFrom = d.GetValueOrDefault(new string[0]) };

            Body = OneOf(Try(Property), Script).Before(SkipWhitespaces).Many();

            TypeParser =
                from h in Header
                from b in Block(Body)
                select new TokenType() { Header = h, Children = b };
        }

        #endregion
        #endregion

        /// <summary>
        /// Creates a new <see cref="PidginTypeParseService"/>, initializing the language parsing frameworks.
        /// </summary>
        public PidginTypeParseService()
        {
            InitStructures();
            InitScripts();
            InitProperties();
            InitLanguage();
        }

        /// <inheritdoc/>
        public TokenType ParseType(string type)
        {
            var result = TypeParser.Parse(type);
            if (result.Success)
            {
                return result.Value;
            }
            else
            {
                throw new ParseException(result.Error.ToString());
            }
        }

        /// <inheritdoc/>
        public TokenType ParseType(TextReader typeReader)
        {
            var result = TypeParser.Parse(typeReader);
            if (result.Success)
            {
                return result.Value;
            }
            else
            {
                throw new ParseException(result.Error.ToString());
            }
        }
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
