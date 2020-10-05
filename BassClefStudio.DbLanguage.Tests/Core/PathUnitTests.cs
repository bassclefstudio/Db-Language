using BassClefStudio.DbLanguage.Core.Documentation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BassClefStudio.DbLanguage.Tests.Core
{
    [TestClass]
    public class PathUnitTests
    {
        #region VersionPath

        [TestMethod]
        public void CompareVersions()
        {
            VersionPath v1 = new VersionPath(1, 0, 0);
            VersionPath v2 = new VersionPath(1, 0, 1);
            Assert.IsTrue(v2 > v1, "Version v1.0.1 was not greater than v1.0.0");
        }

        [TestMethod]
        public void CompareVersionsDiffLengths()
        {
            VersionPath v1 = new VersionPath(1, 0);
            VersionPath v2 = new VersionPath(1, 0, 0);
            Assert.IsTrue(v2 > v1, "Version v1.0.0 was not greater than v1.0.");
        }

        [TestMethod]
        public void CreateEmptyVersion()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new VersionPath());
        }

        [TestMethod]
        public void ParseVersion()
        {
            string v = "1.0.0.0";
            Assert.AreEqual($"{{v{v}}}", ((VersionPath)v).ToString(), $"Version {v} did not parse correctly.");
        }

        #endregion
        #region Namespace

        [TestMethod]
        public void CreateEmptyNamespace()
        {
            Assert.ThrowsException<ArgumentException>(
                () => new Namespace());
        }

        [TestMethod]
        public void ParseNamespace()
        {
            string name = "This.Is.A.Test";
            Assert.AreEqual($"{{{name}}}", ((Namespace)name).ToString(), $"Namespace {name} did not parse correctly.");
        }

        #endregion
    }
}
