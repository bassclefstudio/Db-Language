using BassClefStudio.DbLanguage.Compiler.Parse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Tests.Parse
{
    [TestClass]
    public class CodeParseUnitTests
    {
        internal PidginTypeParseService Parser => ParserUnitTests.Parser;
    }
}
