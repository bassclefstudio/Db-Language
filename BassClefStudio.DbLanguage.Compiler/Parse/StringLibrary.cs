using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BassClefStudio.DbLanguage.Compiler.Parse
{
    internal class StringLibrary
    {
        public string Name { get; set; }
        public IEnumerable<string> Dependencies { get; set; }
        public IEnumerable<StringType> DefinedTypes { get; set; }
    }

    internal class StringType
    {
        public StringTypeHeader Header { get; set; }
        public IEnumerable<StringChild> Properties { get; set; }

        public override string ToString()
        {
            return $"{Header}\r\n - {string.Join("\r\n - ", Properties)}";
        }
    }

    internal class StringTypeHeader
    {
        public string Name { get; set; }
        public bool IsConcrete { get; set; }
        public IEnumerable<string> Dependencies { get; set; }

        public override string ToString()
        {
            return $"{Name} [{(IsConcrete ? "type" : "contract")}] : {string.Join(", ", Dependencies)}";
        }
    }

    internal class StringChild
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
    }

    internal class StringProperty : StringChild
    {
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{Type} {Name} [{(IsPublic ? "public" : "private")}]";
        }
    }

    internal class StringScript : StringChild
    {
        public string ReturnType { get; set; }
        public IEnumerable<StringInput> Inputs { get; set; }
        public IEnumerable<ICodeStatement> Commands { get; set; }

        public override string ToString()
        {
            return $"{ReturnType} {Name} [{(IsPublic ? "public" : "private")}] {{{string.Join(", ", Inputs)}}}";
        }
    }

    internal class StringInput
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{Type} [{Name}]";
        }
    }
}
