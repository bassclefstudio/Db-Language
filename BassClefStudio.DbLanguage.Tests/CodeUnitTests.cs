using BassClefStudio.DbLanguage.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BassClefStudio.DbLanguage.Tests
{
    [TestClass]
    public class CodeUnitTests
    {
        public DbLanguageParser Parser => ParserUnitTests.Parser;

        #region Syntax

        private StringScript CreateScript(string code)
        {
            string fullCode = $"type Blah {{ {code} }}";
            var type = Parser.ParseClass(fullCode);
            var c = type.Properties.FirstOrDefault();
            Assert.IsTrue(type.Properties.Count() == 1, "Incorrect number of children.");
            Assert.IsTrue(c is StringScript, "Child is not a StringScript.");
            return c as StringScript;
        }

        [TestMethod]
        public void CreateMethod()
        {
            string methodName = "Test";
            string returnType = "void";
            var script = CreateScript($"{returnType} {methodName}(){{}}");
            Assert.IsTrue(script.Name == methodName, "Incorrect name.");
            Assert.IsTrue(script.ReturnType == returnType, "Incorrect return type.");
            Assert.IsTrue(script.Inputs.Count() == 0, "Incorrect input count (expected 0).");
        }

        [TestMethod]
        public void CreateMethodWithInput()
        {
            string methodName = "Test";
            string returnType = "void";
            var script = CreateScript($"{returnType} {methodName}(int myInput){{}}");
            Assert.IsTrue(script.Name == methodName, "Incorrect name.");
            Assert.IsTrue(script.ReturnType == returnType, "Incorrect return type.");
            Assert.IsTrue(script.Inputs.Count() == 1, "Incorrect input count (exprected 1).");
        }

        [TestMethod]
        public void IncorrectNameSpacing()
        {
            string methodName = "Test";
            string returnType = "void";
            Assert.ThrowsException<ParseException>(() => CreateScript($"{returnType}{methodName}(){{}}"));
        }

        [TestMethod]
        public void NoName()
        {
            string returnType = "void";
            Assert.ThrowsException<ParseException>(() => CreateScript($"{returnType}(){{}}"));
        }

        [TestMethod]
        public void CheckVisibilityPublic()
        {
            string methodName = "Test";
            string returnType = "void";
            var script = CreateScript($"public {returnType} {methodName}(){{}}");
            Assert.IsTrue(script.IsPublic, "Visibility is not public.");
        }

        [TestMethod]
        public void CheckVisibilityPrivate()
        {
            string methodName = "Test";
            string returnType = "void";
            var script = CreateScript($"private {returnType} {methodName}(){{}}");
            Assert.IsTrue(!script.IsPublic, "Visibility is not private.");
        }

        [TestMethod]
        public void InvalidVisibility()
        {
            string methodName = "Test";
            string returnType = "void";
            Assert.ThrowsException<ParseException>(() => CreateScript($"blah {returnType} {methodName}(){{}}"));
        }

        [TestMethod]
        public void CheckVisibilityDefault()
        {
            string methodName = "Test";
            string returnType = "void";
            var script = CreateScript($"{returnType} {methodName}(){{}}");
            Assert.IsTrue(!script.IsPublic, "Visibility is not private.");
        }

        [TestMethod]
        public void CheckInputs()
        {
            string methodName = "Test";
            string returnType = "void";
            var script = CreateScript($"{returnType} {methodName}(int i1, string i2){{}}");
            Assert.IsTrue(script.Inputs.Count() == 2, "Incorrect number of inputs.");
            var i1 = script.Inputs.ElementAt(0);
            var i2 = script.Inputs.ElementAt(1);
            Assert.IsTrue(i1.Name == "i1", "Incorrect input[0] name.");
            Assert.IsTrue(i2.Name == "i2", "Incorrect input[1] name.");
            Assert.IsTrue(i1.Type == "int", "Incorrect input[0] type.");
            Assert.IsTrue(i2.Type == "string", "Incorrect input[1] type.");
        }

        [TestMethod]
        public void InvalidInput()
        {
            string methodName = "Test";
            string returnType = "void";
            Assert.ThrowsException<ParseException>(() => CreateScript($"blah {returnType} {methodName}(inti1){{}}"));
        }

        #endregion
        #region CodingPatterns
        #region InContext

        private ICodeStatement ParseStatementInContext(string statement)
        {
            var type = Parser.ParseClass($"type Blah {{ void Test() {{ {statement} }} }}");
            var script = type.Properties.FirstOrDefault() as StringScript;
            Assert.IsNotNull(script, "Script property does not exist.");
            Assert.IsTrue(script.Commands.Count() == 1, "Onlly one statement was expected.");
            return script.Commands.First();
        }

        [TestMethod]
        public void RecognizeAddStatementInContext()
        {
            var statement = ParseStatementInContext("int Variable;");
            Assert.IsInstanceOfType(statement, typeof(CodeAdd));
        }

        [TestMethod]
        public void RecognizeVarStatementInContext()
        {
            var statement = ParseStatementInContext("var Variable;");
            Assert.IsInstanceOfType(statement, typeof(CodeVar));
        }

        [TestMethod]
        public void RecognizeSetStatementInContext()
        {
            var statement = ParseStatementInContext("Variable = blah;");
            Assert.IsInstanceOfType(statement, typeof(CodeSet));
        }

        [TestMethod]
        public void RecognizeCallStatementInContext()
        {
            var statement = ParseStatementInContext("Method();");
            Assert.IsInstanceOfType(statement, typeof(CodeStack));
            Assert.IsInstanceOfType((statement as CodeStack).Statement, typeof(CodeCall));
        }

        [TestMethod]
        public void ValueAsStatementFailsInContext()
        {
            Assert.ThrowsException<ParseException>(() => ParseStatementInContext("Get.Value.As.Path;"));
        }

        [TestMethod]
        public void TrickyValueAsStatementFailsInContext()
        {
            Assert.ThrowsException<ParseException>(() => ParseStatementInContext("var.Value.As.Path;"));
        }

        #endregion
        #region StatementParser

        private ICodeStatement GetFirstStatement(string code)
        {
            var statements = Parser.ParseCode(code);
            Assert.IsTrue(statements.Count() == 1, "Expected only one statement.");
            return statements.First();
        }

        [TestMethod]
        public void ValueAsStatementFails()
        {
            Assert.ThrowsException<ParseException>(() => Parser.ParseCode("Get.Value.As.Path;"));
        }

        [TestMethod]
        public void TrickyValueAsStatementFails()
        {
            Assert.ThrowsException<ParseException>(() => Parser.ParseCode("var.Value.As.Path;"));
        }

        [TestMethod]
        public void StackGet()
        {
            var statement = GetFirstStatement("test = This.Is.A.Very.Long.Path;");
            Assert.IsInstanceOfType(statement, typeof(CodeSet), "Expected statement of type CodeSet.");
            var value = (statement as CodeSet).Value;
            Assert.IsInstanceOfType(value, typeof(CodeValueStack), "Expected set parameter of type CodeValueStack");
            var valueStack = (value as CodeValueStack).Values;
            Assert.IsTrue(valueStack.All(v => v is CodeGet), "All values in CodeValueStack should be of type CodeGet");
        }

        [TestMethod]
        public void StackGetWithMethod()
        {
            var statement = GetFirstStatement("test = This.Method().Path;");
            Assert.IsInstanceOfType(statement, typeof(CodeSet), "Expected statement of type CodeSet.");
            var value = (statement as CodeSet).Value;
            Assert.IsInstanceOfType(value, typeof(CodeValueStack), "Expected set parameter of type CodeValueStack");
            var valueStack = (value as CodeValueStack).Values.ToArray();
            Assert.IsInstanceOfType(valueStack[0], typeof(CodeGet), "Stack[0] should be of type CodeGet");
            Assert.IsInstanceOfType(valueStack[1], typeof(CodeGet), "Stack[1] should be of type CodeGet");
            Assert.IsInstanceOfType(valueStack[2], typeof(CodeCall), "Stack[2] should be of type CodeCall");
            Assert.IsInstanceOfType(valueStack[3], typeof(CodeGet), "Stack[3] should be of type CodeGet");
        }

        #endregion
        #endregion
    }
}
