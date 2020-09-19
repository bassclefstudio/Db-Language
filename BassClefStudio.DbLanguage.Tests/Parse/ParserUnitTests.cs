using BassClefStudio.DbLanguage.Compiler.Parse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Tests.Parse
{
    [TestClass]
    public static class ParserUnitTests
    {
        internal static DbParseService Parser;

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            Parser = new DbParseService();
        }
    }
}
