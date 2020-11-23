using BassClefStudio.DbLanguage.Compiler.Parse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace BassClefStudio.DbLanguage.Tests.Parse
{
    [TestClass]
    public class PropertyParseUnitTests
    {
        internal PidginTypeParseService Parser => ParserUnitTests.Parser;

        #region Syntax

        private TokenProperty CheckSingleProperty(string typeName, string propName, string visibility = "public")
        {
            string code = $"type Blah : Parent {{ {visibility} {typeName} {propName}; }}";
            var type = Parser.ParseType(code);
            var c = type.Children.FirstOrDefault();
            Assert.IsTrue(type.Children.Count() == 1, "Incorrect number of children.");
            Assert.IsTrue(c is TokenProperty, "Child is not a TokenProperty.");
            TokenProperty prop = c as TokenProperty;
            Assert.IsTrue(prop.Name == propName, "Incorrect name.");
            Assert.IsTrue(prop.ValueType == typeName, "Incorrect type.");
            return prop;
        }

        [TestMethod]
        public void AddProperty()
        {
            CheckSingleProperty("int", "Property");
        }

        [TestMethod]
        public void LongPropertyType()
        {
            CheckSingleProperty("a.long.type", "Property");
        }

        [TestMethod]
        public void InvalidName()
        {
            Assert.ThrowsException<ParseException>(() => CheckSingleProperty("int", "Prop.erty"));
        }

        [TestMethod]
        public void InvalidNameSpacing()
        {
            Assert.ThrowsException<ParseException>(() => Parser.ParseType("type Blah { publicint Property; }"));
        }

        [TestMethod]
        public void NoName()
        {
            Assert.ThrowsException<ParseException>(() => CheckSingleProperty(null, "Property", null));
        }

        [TestMethod]
        public void CheckVisibilityPublic()
        {
            var prop = CheckSingleProperty("int", "Property", "public");
            Assert.IsTrue(prop.IsPublic == true, "Visibility incorrect.");
        }

        [TestMethod]
        public void CheckVisibilityPrivate()
        {
            var prop = CheckSingleProperty("int", "Property", "private");
            Assert.IsTrue(prop.IsPublic == false, "Visibility incorrect.");
        }

        [TestMethod]
        public void InvalidVisibility()
        {
            Assert.ThrowsException<ParseException>(() => CheckSingleProperty("int", "Property", "blah"));
        }

        [TestMethod]
        public void CheckDefaultVisibility()
        {
            var prop = CheckSingleProperty("int", "Property", null);
            Assert.IsTrue(prop.IsPublic == false, "Default visibility not private.");
        }

        #endregion
    }
}
