using BassClefStudio.DbLanguage.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BassClefStudio.DbLanguage.Tests
{
    [TestClass]
    public class ParserUnitTests
    {
        private static string TestCode;
        private static StringReader TestCodeReader;
        private static DbLanguageParser Parser;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            TestCode =
"type Blah:BlahParent,Namespace.IBlahContract{public int Property;string ToString(int number, Task task){ number = blah; RunMyCode(number, task); }}";
            TestCodeReader = new StringReader(TestCode);
            Parser = new DbLanguageParser();
        }

        [TestMethod]
        public void TestCodeParse()
        {
            var c = Parser.CreateClass(TestCode);
            Console.WriteLine(c.ToString());
        }
    }
}
