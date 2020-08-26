using BassClefStudio.DbLanguage.Core.Scripts.Commands;
using Pidgin;
using System;
using System.Collections.Generic;
using System.IO;
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
                from cs in Block(SkipWhitespaces.Then(Statements))
                select new StringScript() { IsPublic = v.GetValueOrDefault(false), ReturnType = t, Name = n, Inputs = i, Commands = cs } as StringChild;
        }

        #region Code

        private Parser<char, IEnumerable<ICommand>> MethodInputs;
        private Parser<char, ICommand> Method;
        private Parser<char, ICommand> GetVariable;
        private Parser<char, ICommand> SetVariable;
        private Parser<char, ICommand> AddVariable;
        private Parser<char, ICommand> Statement;
        private Parser<char, ICommand> Value;
        private Parser<char, IEnumerable<ICommand>> Statements;

        private void InitCode()
        {
            MethodInputs = Rec(() => Value).Separated(Comma.Then(SkipWhitespaces)).Between(OpenParenthesis, CloseParenthesis);
            GetVariable = Path.Select<ICommand>(p => new GetCommand(p));
            Method =
                from v in GetVariable
                from i in MethodInputs
                select new ScriptCommand(v, i) as ICommand;
            
            //AddVariable = 
            //    from t in Path
            //    from n in Name
            //    select t == null ?  : new AddCommand(n, t)

            SetVariable =
                from p in Path
                from eq in Equal.Between(SkipWhitespaces)
                from v in Rec(() => Value)
                select new SetCommand(p, v) as ICommand;
            Value = Try(Method).Or(GetVariable);
            Statement = Try(Method).Or(SetVariable);
            Statements = Statement.SeparatedAndTerminated(SemiColon.Then(SkipWhitespaces));
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
                from d in Colon.Between(SkipWhitespaces).Then(Path.Separated(Comma.Then(SkipWhitespaces))).Optional()
                select new StringTypeHeader() { Name = n, IsConcrete = t, Dependencies = d.GetValueOrDefault(new string[0]) };
            
            Body = OneOf(Try(Property), Script).Separated(SkipWhitespaces);

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

        public StringType CreateClass(string code)
        {
            var result = Class.Parse(code);
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
