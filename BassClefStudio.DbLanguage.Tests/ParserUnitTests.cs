using BassClefStudio.DbLanguage.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Tests
{
    [TestClass]
    public static class ParserUnitTests
    {
        public static DbLanguageParser Parser;

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            Parser = new DbLanguageParser();
        }
    }
}
