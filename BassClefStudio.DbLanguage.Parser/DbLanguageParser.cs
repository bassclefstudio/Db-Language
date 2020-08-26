using BassClefStudio.DbLanguage.Core.Scripts.Commands;
using Pidgin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Schema;
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

        private Parser<char, StringChild> Script;
        private Parser<char, StringInput> Input;

        private void InitScripts()
        {
            Input =
                from t in Path.Before(Whitespace.AtLeastOnce())
                from n in Name
                select new StringInput() { Type = t, Name = n };

            Script =
                from v in OneOf(Try(Public.ThenReturn(true)), Try(Private.ThenReturn(false))).Before(Whitespace.AtLeastOnce()).Optional()
                from t in Path.Before(Whitespace.AtLeastOnce())
                from n in Name.Before(SkipWhitespaces)
                from i in Input.Separated(Comma.Between(SkipWhitespaces)).Between(OpenParenthesis, CloseParenthesis)
                from cs in Block(SkipWhitespaces.Then(Lines))
                select new StringScript() { IsPublic = v.GetValueOrDefault(false), ReturnType = t, Name = n, Inputs = i, Commands = cs } as StringChild;
        }

        #region Code

        private Parser<char, ICodeValue> GetValue;
        private Parser<char, ICodeBoth> Method;
        private Parser<char, ICodeStatement> VarStatement;
        private Parser<char, ICodeStatement> AddStatement;
        private Parser<char, ICodeStatement> SetStatement;
        private Parser<char, ICodeStatement> ReturnStatement;

        private Parser<char, ICodeValue> ValueStack;
        private Parser<char, IEnumerable<ICodeValue>> AnyValue;
        private Parser<char, ICodeBoth> Stack;
        private Parser<char, ICodeStatement> AnyStatement;
        private Parser<char, IEnumerable<ICodeStatement>> Lines;

        private void InitCode()
        {
            GetValue = Name.Select<ICodeValue>(n => new CodeGet() { Name = n });
            Method = Rec(() => ValueStack).Separated(Comma.Then(SkipWhitespaces)).Between(OpenParenthesis, CloseParenthesis).Select<ICodeBoth>(i => new CodeCall() { Inputs = i });
            VarStatement =
                from v in Var.Before(Whitespace.AtLeastOnce())
                from n in Name
                select new CodeVar() { Name = n } as ICodeStatement;
            AddStatement =
                from t in Path.Before(Whitespace.AtLeastOnce())
                from n in Name
                select new CodeAdd() { Type = t, Name = n } as ICodeStatement;
            SetStatement =
                from p in Path
                from eq in Equal.Between(SkipWhitespaces)
                from v in Rec(() => ValueStack)
                select new CodeSet() { Path = p, Value = v } as ICodeStatement;
            ReturnStatement =
                from r in Return.Before(Whitespace.AtLeastOnce())
                from v in Rec(() => ValueStack)
                select new CodeReturn() { Value = v } as ICodeStatement;
            AnyValue = Try(Map((a, b) => new ICodeValue[] { a, b } as IEnumerable<ICodeValue>, GetValue, Method)).Or(GetValue.Select<IEnumerable<ICodeValue>>(g => new ICodeValue[] { g }));
            ValueStack = AnyValue.Separated(Dot).Select<ICodeValue>(v => new CodeValueStack() { Values = v.SelectMany(vs => vs) });
            Stack = ValueStack.Bind(v =>
            {
                var stack = v as CodeValueStack;
                return stack.Values.LastOrDefault() is ICodeBoth ?
                    Return<ICodeBoth>(new CodeStack() { Values = stack.Values }) :
                    Fail<ICodeBoth>("Stack ended in value, not method call - a line cannot end with a GET request, only a method call.");
            });
            AnyStatement = OneOf(Try(VarStatement), Try(AddStatement), Try(SetStatement), Try(ReturnStatement), Stack.As<char, ICodeStatement, ICodeBoth>());
            Lines = AnyStatement.SeparatedAndTerminated(SemiColon.Then(SkipWhitespaces));
        }

        #endregion
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
            
            Body = OneOf(Try(Property), Script).Before(SkipWhitespaces).Many();

            Class =
                from h in Header
                from b in Block(Body)
                select new StringType() { Header = h, Properties = b };
        }

        #endregion
        #endregion

        public DbLanguageParser()
        {
            InitStructures();
            InitCode();
            InitScripts();
            InitProperties();
            InitLanguage();
        }

        public StringLibrary ParseLibrary(string code) => ParseLibrary(new StringReader(code));
        public StringLibrary ParseLibrary(TextReader textReader)
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

        public StringType ParseClass(string code) => ParseClass(new StringReader(code));
        public StringType ParseClass(TextReader reader)
        {
            var result = Class.Parse(reader);
            if (result.Success)
            {
                return result.Value;
            }
            else
            {
                throw new ParseException(result.Error.RenderErrorMessage());
            }
        }

        public IEnumerable<ICodeStatement> ParseCode(string code) => ParseCode(new StringReader(code));
        public IEnumerable<ICodeStatement> ParseCode(TextReader reader)
        {
            var result = Lines.Parse(reader);
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

    public static class ParseExtensions
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
