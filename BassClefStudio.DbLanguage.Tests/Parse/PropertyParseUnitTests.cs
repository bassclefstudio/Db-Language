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
        public DbLanguageParser Parser => ParserUnitTests.Parser;

        #region Syntax

        private StringProperty CheckSingleProperty(string typeName, string propName, string visibility = "public")
        {
            string code = $"type Blah : Parent {{ {visibility} {typeName} {propName}; }}";
            var type = Parser.ParseClass(code);
            var c = type.Properties.FirstOrDefault();
            Assert.IsTrue(type.Properties.Count() == 1, "Incorrect number of children.");
            Assert.IsTrue(c is StringProperty, "Child is not a StringProperty.");
            StringProperty prop = c as StringProperty;
            Assert.IsTrue(prop.Name == propName, "Incorrect name.");
            Assert.IsTrue(prop.Type == typeName, "Incorrect type.");
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
            Assert.ThrowsException<ParseException>(() => Parser.ParseClass("type Blah { publicint Property; }"));
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
