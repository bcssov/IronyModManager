using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Parser.Models;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    public class InlineScriptFailureTests
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void Compact_script_is_rejected_when_preparation_or_expansion_exceeds_the_limit(bool nested, bool prepareFirst)
        {
            DISetup.SetupContainer();
            var codeParser = new CodeParser(new Logger());
            var compact = Enumerable.Range(0, 10001)
                .Select(i => $"button_{i}={{potential={{always=no}} allow={{always=no}} effect={{}}}}").ToList();
            var original = codeParser.ParseScriptWithoutValidation(compact, "compact.txt");
            Assert.Null(original.Error);
            var prepared = string.Join(Environment.NewLine, original.Values.Select(v => codeParser.FormatCode(v)));
            var input = prepareFirst ? prepared : string.Join(Environment.NewLine, compact);
            var parser = new ParametrizedParser(codeParser);
            var exception = Assert.Throws<ScriptParseException>(() =>
            {
                if (nested)
                    parser.ProcessFirstOptimized(input, "button={inline_script={script=compact}}", out _);
                else
                    parser.ProcessOptimized(input, "inline_script={script=compact}", out _);
            });
            Assert.Contains("100000", exception.Error.Message);
            Assert.Contains(prepareFirst || !nested ? "100010" : "100012", exception.Error.Message);
        }

        [Fact]
        public void Original_source_line_limit_is_preserved()
        {
            DISetup.SetupContainer();
            var codeParser = new CodeParser(new Logger());
            var lines = Enumerable.Repeat("value=yes", 100001).ToList();
            var result = codeParser.ParseScriptWithoutValidation(lines, "source.txt");
            Assert.Null(result.Values);
            Assert.Contains("100001", result.Error.Message);
            Assert.Contains("100000", result.Error.Message);
            var validated = codeParser.ParseScript(lines, "source.txt", true);
            Assert.Null(validated.Values);
            Assert.Equal(result.Error.Message, validated.Error.Message);
        }

        [Fact]
        public void Substitution_cannot_expand_past_the_existing_limit()
        {
            DISetup.SetupContainer();
            var codeParser = new CodeParser(new Logger());
            var content = string.Join(Environment.NewLine, Enumerable.Repeat("value=yes", 50001));
            var parameters = "inline_script={script=compact content=\"" + content + "\"}";
            Assert.Null(codeParser.VerifyAllowedLength(parameters.SplitOnNewLine()));
            var parser = new ParametrizedParser(codeParser);
            var exception = Assert.Throws<ScriptParseException>(() =>
                parser.ProcessOptimized("$content$" + Environment.NewLine + "$content$", parameters, out _));
            Assert.Contains("100002", exception.Error.Message);
        }

        [Fact]
        public void Small_valid_parameterized_script_keeps_its_output()
        {
            DISetup.SetupContainer();
            var codeParser = new CodeParser(new Logger());
            var result = new ParametrizedParser(codeParser).ProcessOptimized("button_$id$={allow={always=no}}",
                "inline_script={script=small id=7}", out var processed);
            Assert.True(processed);
            var parsed = codeParser.ParseScriptWithoutValidation(result.SplitOnNewLine(), string.Empty);
            Assert.Null(parsed.Error);
            var button = Assert.Single(parsed.Values);
            Assert.Equal("button_7", button.Key);
            Assert.Equal("no", Assert.Single(Assert.Single(button.Values).Values).Value);
        }

        [Fact]
        public void Syntax_rejection_retains_the_original_parser_error()
        {
            DISetup.SetupContainer();
            var codeParser = new CodeParser(new Logger());
            const string malformed = "root = { value";
            var error = codeParser.ParseScriptWithoutValidation([malformed], string.Empty).Error;
            Assert.NotNull(error);
            var exception = Assert.Throws<ScriptParseException>(() => new ParametrizedParser(codeParser)
                .ProcessOptimized(malformed, "inline_script={script=malformed}", out _));
            Assert.Equal(error.Message, exception.Error.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Missing_values_without_an_error_is_a_contract_violation(bool nullResponse)
        {
            var codeParser = new Mock<ICodeParser>();
            codeParser.Setup(p => p.ParseScriptWithoutValidation(It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
                .Returns(nullResponse ? null : new ParseResponse());
            Assert.Throws<InvalidOperationException>(() => new ParametrizedParser(codeParser.Object)
                .ProcessOptimized("value=yes", "inline_script={script=test}", out _));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Unexpected_failures_are_not_reported_as_script_rejections(bool outOfMemory)
        {
            DISetup.SetupContainer();
            var failure = outOfMemory ? (Exception)new OutOfMemoryException("test only") : new InvalidOperationException("test defect");
            var codeParser = new FailingCodeParser(failure);
            var observed = Record.Exception(() => codeParser.ParseScriptWithoutValidation(["value=yes"], string.Empty));
            Assert.Same(failure, observed);
        }

        private sealed class FailingCodeParser(Exception failure) : CodeParser(new Logger())
        {
            public override IEnumerable<string> CleanCode(string file, IEnumerable<string> lines)
            {
                throw failure;
            }
        }
    }
}
