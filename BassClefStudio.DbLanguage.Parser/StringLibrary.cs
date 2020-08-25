using BassClefStudio.DbLanguage.Core.Data;
using System;
using System.Collections.Generic;
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
        public StringHeader Header { get; set; }
        public IEnumerable<StringProperty> Properties { get; set; }
    }

    public class StringHeader
    {
        public string Name { get; set; }
        public bool IsContract { get; set; }
        public IEnumerable<string> Dependencies { get; set; }
    }

    public class StringProperty
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsPublic { get; set; }
    }
}
