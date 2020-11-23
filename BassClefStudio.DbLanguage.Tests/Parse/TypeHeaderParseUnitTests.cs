using BassClefStudio.DbLanguage.Compiler.Parse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Tests.Parse
{
    [TestClass]
    public class TypeHeaderParseUnitTests
    {
        internal PidginTypeParseService Parser => ParserUnitTests.Parser;

        #region Syntax

        [TestMethod]
        public void TypeWithDependenciesParse()
        {
            string code = "type Blah:BlahParent,Namespace.IBlahContract {}";
            var type = Parser.ParseType(code);
            Assert.IsTrue(type.Header.InheritsFrom.Count() == 2);
        }

        [TestMethod]
        public void WrongTypeName()
        {
            string code = "type Bl.ah {}";
            Assert.ThrowsException<ParseException>(() => Parser.ParseType(code));
        }

        [TestMethod]
        public void IncorrectTypeDependencyString()
        {
            string code = "type Blah : {}";
            Assert.ThrowsException<ParseException>(() => Parser.ParseType(code));
        }

        [TestMethod]
        public void CheckTypeConcreteParse()
        {
            string code = "type Blah {}";
            Assert.IsTrue(Parser.ParseType(code).Header.IsConcrete);
        }

        [TestMethod]
        public void CheckTypeContractParse()
        {
            string code = "contract Blah {}";
            Assert.IsFalse(Parser.ParseType(code).Header.IsConcrete);
        }

        [TestMethod]
        public void InvalidNameSpacing()
        {
            string code = "typeBlah {}";
            Assert.ThrowsException<ParseException>(() => Parser.ParseType(code));
        }

        [TestMethod]
        public void NoName()
        {
            string code = "{}";
            Assert.ThrowsException<ParseException>(() => Parser.ParseType(code));
        }

        #endregion
    }
}
