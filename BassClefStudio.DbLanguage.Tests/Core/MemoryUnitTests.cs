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
        public void CreateObject()
        {
            DataObject o = new DataObject(TestType);
            Assert.IsTrue(o.DataType == TestType, "DataType type not set.");
        }

        [TestMethod]
        public void TrySetTypeBinding()
        {
            DataObject o = new DataObject(TestType);
            o.TrySetObject<string>("This is a test");
            Assert.AreEqual("This is a test", o.GetObject<string>(), "Could not store and retreive bound object.");
        }

        [TestMethod]
        public void InvalidTypeBinding()
        {
            DataObject o = new DataObject(TestType);
            Assert.ThrowsException<TypeBindingException>(() => o.TrySetObject<int>(13));
        }

        [TestMethod]
        public void SetMemoryItem()
        {
            MemoryItem testItem = new MemoryItem(new MemoryProperty("Prop", TestType));
            var o = new DataObject(TestType);
            Assert.IsTrue(testItem.Set(o), "Set operation failed.");
            Assert.AreEqual(o, testItem.Value, "Value failed to store in memory.");
        }

        [TestMethod]
        public void SetMemoryItemInvalidType()
        {
            MemoryItem testItem = new MemoryItem(new MemoryProperty("Prop", TestType));
            var o = new DataObject(AnotherTestType);
            Assert.IsFalse(testItem.Set(o), "Set operation should not have succeeded with incorrect type.");
            Assert.AreNotEqual(o, testItem.Value, "Value was incorrectly stored in memory.");
        }

        #endregion
        #region Groups



        #endregion
        #region Stacks



        #endregion
    }
}
