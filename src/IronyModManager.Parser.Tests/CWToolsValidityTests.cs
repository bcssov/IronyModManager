using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AwesomeAssertions;
using CWTools.CSharp;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    public class CWToolsValidityTests
    {
        public CWToolsValidityTests()
        {
            DISetup.SetupContainer();
        }

        [Fact]
        public void PerformValidityCheck_should_accept_normal_assignment()
        {
            var parser = new CodeParser(new Logger());

            var result = parser.PerformValidityCheck(new[] { "root = { value = yes }" }, "assignment-test.txt");

            result.Should().BeNull();
        }

        [Theory]
        [InlineData("starbase?= { value = yes }")]
        [InlineData("starbase? = { value = yes }")]
        public void PerformValidityCheck_should_accept_Stellaris_question_mark_keys(string source)
        {
            var parser = new CodeParser(new Logger());

            var result = parser.PerformValidityCheck(new[] { source }, "question-key-test.txt");

            result.Should().BeNull();
        }

        [Fact]
        public void PerformValidityCheck_should_reject_malformed_source()
        {
            var parser = new CodeParser(new Logger());

            var result = parser.PerformValidityCheck(new[] { "root = { value = yes" }, "malformed-test.txt");

            result.Should().NotBeNull();
        }

        [Fact]
        public void Live_Stellaris_question_key_should_pass_the_CWTools_and_Irony_validity_paths()
        {
            var file = Path.Combine(AppContext.BaseDirectory, "testfiles", "cwtools", "stellaris-question-key.txt");
            var source = File.ReadAllText(file);

            var cwToolsResult = Parsers.ParseScriptFile(file, source);
            cwToolsResult.IsSuccess.Should().BeTrue();

            var ironyResult = new CodeParser(new Logger()).PerformValidityCheck(File.ReadAllLines(file), file);
            ironyResult.Should().BeNull();
        }

        [Fact]
        public void Malformed_live_Stellaris_question_key_should_fail_the_CWTools_and_Irony_validity_paths()
        {
            var file = Path.Combine(AppContext.BaseDirectory, "testfiles", "cwtools", "stellaris-question-key.txt");
            var source = File.ReadAllText(file);
            var malformed = source.Remove(source.LastIndexOf('}'));

            var cwToolsResult = Parsers.ParseScriptFile(file, malformed);
            var ironyResult = new CodeParser(new Logger()).PerformValidityCheck(malformed.Replace("\r\n", "\n").Split('\n'), file);

            cwToolsResult.IsSuccess.Should().BeFalse();
            ironyResult.Should().NotBeNull();
        }

        [Fact]
        public void Question_mark_key_should_use_CWTools_validity_check()
        {
            var codeParser = new Mock<ICodeParser>(MockBehavior.Strict);
            var lines = new[] { "starbase? = { value = yes }" };
            codeParser.Setup(p => p.VerifyAllowedLength(lines)).Returns((IScriptError)null);
            codeParser.Setup(p => p.PerformValidityCheck(lines, "question-key.txt", false)).Returns((IScriptError)null);
            var parser = new ValidateParser(codeParser.Object, new Logger());

            var result = parser.Validate(new ParserArgs
            {
                File = "question-key.txt",
                Lines = lines,
                ModName = "regression"
            });

            result.Should().BeNull();
            codeParser.VerifyAll();
        }

        [Fact]
        public void Validate_should_fall_back_to_basic_checking_when_CWTools_throws()
        {
            var codeParser = new Mock<ICodeParser>(MockBehavior.Strict);
            var lines = new[] { "root = { value = yes }" };
            codeParser.Setup(p => p.VerifyAllowedLength(lines)).Returns((IScriptError)null);
            codeParser.Setup(p => p.PerformValidityCheck(lines, "fallback-test.txt", false)).Throws<InvalidOperationException>();
            codeParser.Setup(p => p.PerformValidityCheck(lines, "fallback-test.txt", true)).Returns((IScriptError)null);
            var parser = new ValidateParser(codeParser.Object, new Logger());

            var result = parser.Validate(new ParserArgs
            {
                File = "fallback-test.txt",
                Lines = lines,
                ModName = "regression"
            });

            result.Should().BeNull();
            codeParser.VerifyAll();
        }

        [Fact]
        public void Direct_reference_assemblies_should_load_from_test_output()
        {
            foreach (var assemblyName in new[] { "CWTools", "CSharpHelpers", "Shared" })
            {
                var path = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.dll");
                File.Exists(path).Should().BeTrue($"{assemblyName}.dll should be copied from References/Direct");
                System.Reflection.Assembly.LoadFrom(path).GetName().Name.Should().Be(assemblyName);
            }
        }

        [Fact]
        public void PerformValidityCheck_should_support_representative_parallel_workload()
        {
            const string source = "starbase? = { value = yes nested = { count = 42 } }";

            var failures = Enumerable.Range(0, 250)
                .AsParallel()
                .WithDegreeOfParallelism(Math.Min(Environment.ProcessorCount, 8))
                .Select(iteration => new CodeParser(new Logger()).PerformValidityCheck(
                    new[] { source },
                    $"parallel-{iteration}.txt"))
                .Count(error => error != null);

            failures.Should().Be(0);
        }
    }
}
