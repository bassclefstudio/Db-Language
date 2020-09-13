using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Parse
{
    internal interface ICode
    { }
    internal interface ICodeStatement : ICode
    { }
    internal interface ICodeValue : ICode
    { }
    internal interface ICodeBoth : ICodeStatement, ICodeValue
    { }

    internal class CodeValueStack : ICodeValue
    {
        public IEnumerable<ICodeValue> Values { get; set; }
    }

    internal class CodeStack : ICodeBoth
    {
        public IEnumerable<ICodeValue> Values { get; set; }
        public ICodeBoth Statement => Values.LastOrDefault() as ICodeBoth;
    }

    internal class CodeVar : ICodeStatement
    {
        public string Name { get; set; }
    }

    internal class CodeAdd : CodeVar, ICodeStatement
    {
        public string Type { get; set; }
    }

    internal class CodeGet : ICodeValue
    {
        public string Name { get; set; }
    }

    internal class CodeSet : ICodeStatement
    {
        public string Path { get; set; }
        public ICodeValue Value { get; set; }
    }

    internal class CodeReturn : ICodeStatement
    {
        public ICodeValue Value { get; set; }
    }


    internal class CodeCall : ICodeBoth
    {
        public IEnumerable<ICodeValue> Inputs { get; set; }
    }
}

