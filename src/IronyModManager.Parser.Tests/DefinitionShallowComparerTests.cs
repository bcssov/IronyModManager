using System.Linq;
using AwesomeAssertions;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Definitions;
using IronyModManager.Shared.Models;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    public class DefinitionShallowComparerTests
    {
        [Fact]
        public void Should_resolve_shallow_comparer_from_parser_composition()
        {
            DISetup.SetupContainer();

            var comparer = DISetup.Container.GetInstance<IDefinitionShallowComparer>();

            comparer.Should().BeOfType<DefinitionShallowComparer>();
        }

        [Fact]
        public void Should_ignore_only_immediate_child_order()
        {
            var comparer = GetComparer();
            var left = Definition("test = { first = 1 second = { a = 1 b = 2 } }");
            var right = Definition("test = { second = { a = 1 b = 2 } first = 1 }");

            var result = comparer.Compare([left, right]);

            result.Status.Should().Be(DefinitionShallowComparisonStatus.Equivalent);
            result.EquivalentGroups.Should().ContainSingle();
        }

        [Fact]
        public void Should_preserve_nested_order()
        {
            var comparer = GetComparer();
            var left = Definition("test = { value = { a = 1 b = 2 } }");
            var right = Definition("test = { value = { b = 2 a = 1 } }");

            var result = comparer.Compare([left, right]);

            result.Status.Should().Be(DefinitionShallowComparisonStatus.Different);
            result.EquivalentGroups.Should().HaveCount(2);
        }

        [Fact]
        public void Should_group_duplicate_immediate_keys_as_a_multiset()
        {
            var comparer = GetComparer();
            var left = Definition("test = { modifier = { value = A } modifier = { value = B } }");
            var right = Definition("test = { modifier = { value = B } modifier = { value = A } }");

            var result = comparer.Compare([left, right]);

            result.Status.Should().Be(DefinitionShallowComparisonStatus.Equivalent);
        }

        [Fact]
        public void Should_compare_unquoted_tokens_case_insensitively_and_quoted_content_exactly()
        {
            var comparer = GetComparer();
            var unquoted = comparer.Compare([
                Definition("test = { trigger = { FROM = { ROOT = VALUE } } }"),
                Definition("test = { trigger = { from = { root = value } } }")]);
            var quoted = comparer.Compare([
                Definition("test = { name = \"VALUE\" }"),
                Definition("test = { name = \"value\" }")]);

            unquoted.Status.Should().Be(DefinitionShallowComparisonStatus.Equivalent);
            quoted.Status.Should().Be(DefinitionShallowComparisonStatus.Different);
        }

        [Fact]
        public void Should_return_unsupported_for_malformed_code()
        {
            var comparer = GetComparer();

            var result = comparer.Compare([Definition("test = { value = 1"), Definition("test = { value = 1 }")]);

            result.Status.Should().Be(DefinitionShallowComparisonStatus.Unsupported);
            result.EquivalentGroups.Should().BeEmpty();
        }

        [Fact]
        public void Should_keep_partial_equivalence_classes_for_service_orchestration()
        {
            var comparer = GetComparer();
            var first = Definition("test = { first = 1 second = 2 }");
            var reordered = Definition("test = { second = 2 first = 1 }");
            var different = Definition("test = { first = 1 second = 3 }");

            var result = comparer.Compare([first, reordered, different]);

            result.Status.Should().Be(DefinitionShallowComparisonStatus.Different);
            result.EquivalentGroups.Should().HaveCount(2);
            result.EquivalentGroups.Should().Contain(group => group.Count == 2 && group.Contains(first) && group.Contains(reordered));
        }

        private static DefinitionShallowComparer GetComparer()
        {
            DISetup.SetupContainer();
            return new DefinitionShallowComparer(new CodeParser(new Logger()));
        }

        private static IDefinition Definition(string code)
        {
            return new Definition
            {
                Code = code,
                DefinitionSHA = code,
                File = "common\\test\\test.txt",
                Id = "test",
                Type = "common-test",
                ValueType = ValueType.Object
            };
        }
    }
}
