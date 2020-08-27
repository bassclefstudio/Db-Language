using BassClefStudio.DbLanguage.Core.Data;
using BassClefStudio.DbLanguage.Core.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.DbLanguage.Tests.Core
{
    [TestClass]
    public class TypeUnitTests
    {
        [TestMethod]
        public void IsType()
        {
            DataType t = new DataType("TestType", typeof(string), new MemoryProperty[0], new MemoryProperty[0]);
            Assert.IsTrue(t.Is(t), "Type IS not itself.");
            DataType dT = new DataType("DerivedType", typeof(string), new MemoryProperty[0], new MemoryProperty[0], null, t);
            Assert.IsTrue(dT.Is(t), "Derived type IS not parent.");
            Assert.IsFalse(t.Is(dT), "Parent type IS derived type.");
        }
    }
}
