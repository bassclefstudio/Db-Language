using BassClefStudio.DbLanguage.Compiler.Parse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.DbLanguage.Tests.Parse
{
    [TestClass]
    public class ParsePublishUnitTests
    {
        internal PidginTypeParseService Parser => ParserUnitTests.Parser;

        [TestMethod]
        public async Task ParsePublishSingleType()
        {
            string codeFile = "public type MyType { public int Test; bool RunMethod(MyType other) {} }";
            var type = Parser.ParseType(codeFile);
            TokenPackage package = new TokenPackage(null, new TokenType[] { type });
            using (var stream = new MemoryStream(2048))
            {
                await new JsonPublishPipeline().PublishPackageAsync(package, stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    Console.Write(streamReader.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public async Task ParsePublishSingleTypeWithScript()
        {
            string codeFile = "public type MyType { public int Test; bool RunMethod(MyType other) { this.Property.MyProperty(\"literal\"); this.Property = null; } }";
            var type = Parser.ParseType(codeFile);
            TokenPackage package = new TokenPackage(null, new TokenType[] { type });
            using (var stream = new MemoryStream(2048))
            {
                await new JsonPublishPipeline().PublishPackageAsync(package, stream);
                stream.Seek(0, SeekOrigin.Begin);
                using (var streamReader = new StreamReader(stream))
                {
                    Console.Write(streamReader.ReadToEnd());
                }
            }
        }
    }
}
