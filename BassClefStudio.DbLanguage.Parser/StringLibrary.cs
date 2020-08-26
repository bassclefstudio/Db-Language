using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Scripts.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BassClefStudio.DbLanguage.Parser
{
    public class StringLibrary
    {
        public string Name { get; set; }
        public IEnumerable<string> Dependencies { get; set; }
        public IEnumerable<StringType> DefinedTypes { get; set; }
    }

    public class StringType
    {
        public StringTypeHeader Header { get; set; }
        public IEnumerable<StringChild> Properties { get; set; }
    }

    public class StringTypeHeader
    {
        public string Name { get; set; }
        public bool IsContract { get; set; }
        public IEnumerable<string> Dependencies { get; set; }
    }

    public class StringChild
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
    }

    public class StringProperty : StringChild
    {
        public string Type { get; set; }
    }

    public class StringScript : StringChild
    {
        public string ReturnType { get; set; }
        public IEnumerable<StringInput> Inputs { get; set; }
        public IEnumerable<ICommand> Commands { get; set; }
    }

    public class StringInput
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
