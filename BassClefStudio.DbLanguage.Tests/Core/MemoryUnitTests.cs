using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Tests.Core
{
    [TestClass]
    public class MemoryUnitTests
    {
        public static IType TestType { get; set; }

        [ClassInitialize]
        public static void InitializeTestType(TestContext context)
        {
            // Creates a valid type for setting memory.
            TestType = new DataType("TestType", typeof(string), new MemoryProperty[0], new MemoryProperty[0]);
        }

        #region Items

        [TestMethod]
        public void CreateProperty()
        {
            MemoryProperty property = new MemoryProperty("Prop", TestType);
            Assert.IsTrue(property.Key == "Prop", "Property has incorrect name");
            Assert.IsTrue(property.Type == TestType, "Property has incorrect datatype");
        }

        [TestMethod]
        public void CreatePropertyNoType()
        {
            Assert.ThrowsException<ArgumentException>(() => new MemoryProperty("Prop", null));
        }

        [TestMethod]
        public void CreateMemoryItem()
        {
            MemoryItem testItem = new MemoryItem(new MemoryProperty("Prop", TestType));
            Assert.IsTrue(testItem.Value == null, "Value was not initialized to null.");
        }

        #endregion
        #region Groups



        #endregion
        #region Stacks



        #endregion
    }
}
