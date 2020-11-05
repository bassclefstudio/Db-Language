using Pidgin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Parse
{
    internal class TokenPos
    {
        public int LineNumber { get; }
        public int ColumnNumber { get; }

        public TokenPos(SourcePos sourcePos)
        {
            LineNumber = sourcePos.Line;
            ColumnNumber = sourcePos.Col;
        }
    }

    internal class TokenChild
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public TokenPos SourcePosition { get; set; }
    }

    internal class TokenTypeHeader : TokenChild
    {
        public bool IsConcrete { get; set; }
        public IEnumerable<string> InheritsFrom { get; set; }
    }

    internal class TokenType
    {
        public TokenTypeHeader Header { get; set; }
        public IEnumerable<TokenChild> Children { get; set; }
    }

    internal class TokenProperty : TokenChild
    {
        public string ValueType { get; set; }
    }

    internal class TokenScript : TokenChild
    {
        public string ReturnType { get; set; }
    }
}
