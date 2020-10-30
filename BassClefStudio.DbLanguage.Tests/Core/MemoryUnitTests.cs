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
        public static DataType TestType { get; set; }
        public static DataType AnotherTestType { get; set; }
        public static DataType DerivedTestType { get; set; }

        [ClassInitialize]
        public static void InitializeTestType(TestContext context)
        {
            // Creates valid types for setting memory.
            TestType = new DataType("TestType", typeof(string), new MemoryProperty[0], new MemoryProperty[0]);
            AnotherTestType = new DataType("OtherType", typeof(string), new MemoryProperty[0], new MemoryProperty[0]);
            DerivedTestType = new DataType("DerivedType", typeof(string), new MemoryProperty[0], new MemoryProperty[0], null, TestType);
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

        [TestMethod]
        public void SetMemoryItem()
        {
            MemoryItem testItem = new MemoryItem(new MemoryProperty("Prop", TestType));
            var o = new DataObject(TestType);
            testItem.Set(o);
            Assert.AreEqual(o, testItem.Value, "Value failed to store in memory.");
        }

        [TestMethod]
        public void SetMemoryItemInvalidType()
        {
            MemoryItem testItem = new MemoryItem(new MemoryProperty("Prop", TestType));
            var o = new DataObject(AnotherTestType);
            Assert.ThrowsException<MemoryException>(() => testItem.Set(o), "Set operation should not have succeeded with incorrect type.");
            Assert.AreNotEqual(o, testItem.Value, "Value was incorrectly stored in memory.");
        }

        #endregion
        #region Groups

        [TestMethod]
        public void DuplicateMemoryInGroup()
        {
            IWritableMemoryGroup testGroup = new MemoryGroup();
            MemoryProperty prop = new MemoryProperty("Property", new DataType("TestType", new MemoryProperty[0], new MemoryProperty[0]));
            MemoryItem i = new MemoryItem(prop);
            Assert.IsTrue(testGroup.Add(i), "Failed to add item.");
            Assert.IsFalse(testGroup.Add(i), "Duplicate was allowed to be added.");
        }

        [TestMethod]
        public void DuplicatePropertyInGroup()
        {
            IWritableMemoryGroup testGroup = new MemoryGroup();
            MemoryProperty prop = new MemoryProperty("Property", new DataType("TestType", new MemoryProperty[0], new MemoryProperty[0]));
            Assert.IsTrue(testGroup.Add(new MemoryItem(prop)), "Failed to add item.");
            Assert.IsFalse(testGroup.Add(new MemoryItem(prop)), "Duplicate property was allowed to be added.");
        }

        [TestMethod]
        public void DuplicateNameInGroup()
        {
            IWritableMemoryGroup testGroup = new MemoryGroup();
            MemoryProperty prop = new MemoryProperty("Property", new DataType("TestType", new MemoryProperty[0], new MemoryProperty[0]));
            MemoryProperty prop2 = new MemoryProperty("Property", new DataType("TestType", new MemoryProperty[0], new MemoryProperty[0]));
            Assert.IsTrue(testGroup.Add(new MemoryItem(prop)), "Failed to add item.");
            Assert.IsFalse(testGroup.Add(new MemoryItem(prop2)), "Duplicate name was allowed to be added.");
        }

        #endregion
        #region Stacks



        #endregion
    }
}
