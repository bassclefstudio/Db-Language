using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace BassClefStudio.DbLanguage.Parser
{
    public interface ICode
    { }
    public interface ICodeStatement : ICode
    { }
    public interface ICodeValue : ICode
    { }
    public interface ICodeBoth : ICodeStatement, ICodeValue
    { }

    public class CodeValueStack : ICodeValue
    {
        public IEnumerable<ICodeValue> Values { get; set; }
    }

    public class CodeStack : ICodeBoth
    {
        public IEnumerable<ICodeValue> Values { get; set; }
        public ICodeBoth Statement => Values.LastOrDefault() as ICodeBoth;
    }

    public class CodeVar : ICodeStatement
    {
        public string Name { get; set; }
    }

    public class CodeAdd : CodeVar, ICodeStatement
    {
        public string Type { get; set; }
    }

    public class CodeGet : ICodeValue
    {
        public string Name { get; set; }
    }

    public class CodeSet : ICodeStatement
    {
        public string Path { get; set; }
        public ICodeValue Value { get; set; }
    }

    public class CodeReturn : ICodeStatement
    {
        public ICodeValue Value { get; set; }
    }


    public class CodeCall : ICodeBoth
    {
        public IEnumerable<ICodeValue> Inputs { get; set; }
    }
}

