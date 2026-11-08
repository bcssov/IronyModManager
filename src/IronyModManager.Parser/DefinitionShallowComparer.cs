// ***********************************************************************
// Assembly         : IronyModManager.Parser
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Implements collision-safe first-level definition comparison over parser output.
    /// </summary>
    public sealed class DefinitionShallowComparer(ICodeParser codeParser) : IDefinitionShallowComparer
    {
        /// <inheritdoc />
        public DefinitionShallowComparisonResult Compare(IReadOnlyCollection<IDefinition> definitions)
        {
            if (definitions == null || definitions.Count < 2)
            {
                return new DefinitionShallowComparisonResult(DefinitionShallowComparisonStatus.Unsupported, []);
            }

            var representations = new Dictionary<DefinitionCacheKey, ShallowRepresentation>();
            var equivalentGroups = new Dictionary<ShallowRepresentation, List<IDefinition>>();
            foreach (var definition in definitions)
            {
                var cacheKey = new DefinitionCacheKey(definition.DefinitionSHA, definition.Code);
                if (!representations.TryGetValue(cacheKey, out var representation))
                {
                    if (!TryBuild(definition, out representation))
                    {
                        return new DefinitionShallowComparisonResult(DefinitionShallowComparisonStatus.Unsupported, []);
                    }

                    representations.Add(cacheKey, representation);
                }

                if (!equivalentGroups.TryGetValue(representation, out var equivalentDefinitions))
                {
                    equivalentDefinitions = [];
                    equivalentGroups.Add(representation, equivalentDefinitions);
                }

                equivalentDefinitions.Add(definition);
            }

            var groups = equivalentGroups.Values.Select(group => (IReadOnlyList<IDefinition>)group).ToList();
            var status = groups.Count == 1 ? DefinitionShallowComparisonStatus.Equivalent : DefinitionShallowComparisonStatus.Different;
            return new DefinitionShallowComparisonResult(status, groups);
        }

        private bool TryBuild(IDefinition definition, out ShallowRepresentation representation)
        {
            representation = null;
            if (definition == null || string.IsNullOrWhiteSpace(definition.Code))
            {
                return false;
            }

            var response = codeParser.ParseScriptWithoutValidation(
                definition.Code.Split(["\r\n", "\r", "\n"], StringSplitOptions.None), definition.File ?? string.Empty);
            var roots = response?.Values?.ToList();
            if (response?.Error != null || roots == null || roots.Count != 1)
            {
                return false;
            }

            var root = roots[0];
            var immediateChildren = root.Values?.ToList();
            if (root.IsSimpleType || string.IsNullOrWhiteSpace(root.Key) || string.IsNullOrWhiteSpace(root.Operator) || immediateChildren == null ||
                immediateChildren.Any(child => string.IsNullOrWhiteSpace(child.Key) || string.IsNullOrWhiteSpace(child.Operator)))
            {
                return false;
            }

            var groupedValues = new Dictionary<string, Dictionary<OpaqueElement, int>>(ScriptTokenComparer.Instance);
            foreach (var child in immediateChildren)
            {
                if (!groupedValues.TryGetValue(child.Key, out var values))
                {
                    values = new Dictionary<OpaqueElement, int>();
                    groupedValues.Add(child.Key, values);
                }

                var opaqueValue = OpaqueElement.Create(child);
                values[opaqueValue] = values.GetValueOrDefault(opaqueValue) + 1;
            }

            representation = new ShallowRepresentation(root.Key, root.Operator, groupedValues);
            return true;
        }

        private readonly record struct DefinitionCacheKey(string DefinitionSha, string Code);

        private sealed class ShallowRepresentation : IEquatable<ShallowRepresentation>
        {
            private readonly Dictionary<string, Dictionary<OpaqueElement, int>> groups;
            private readonly int hashCode;
            private readonly string rootKey;
            private readonly string rootOperator;

            public ShallowRepresentation(string rootKey, string rootOperator, Dictionary<string, Dictionary<OpaqueElement, int>> groups)
            {
                this.rootKey = rootKey;
                this.rootOperator = rootOperator;
                this.groups = groups;

                var unorderedGroupsHash = 0;
                foreach (var group in groups)
                {
                    var unorderedValuesHash = 0;
                    foreach (var value in group.Value)
                    {
                        unorderedValuesHash += HashCode.Combine(value.Key.GetHashCode(), value.Value);
                    }

                    unorderedGroupsHash += HashCode.Combine(ScriptTokenComparer.Instance.GetHashCode(group.Key), unorderedValuesHash);
                }

                hashCode = HashCode.Combine(ScriptTokenComparer.Instance.GetHashCode(rootKey), StringComparer.Ordinal.GetHashCode(rootOperator), groups.Count, unorderedGroupsHash);
            }

            public bool Equals(ShallowRepresentation other)
            {
                if (other == null || hashCode != other.hashCode || groups.Count != other.groups.Count ||
                    !ScriptTokenComparer.Instance.Equals(rootKey, other.rootKey) || !string.Equals(rootOperator, other.rootOperator, StringComparison.Ordinal))
                {
                    return false;
                }

                foreach (var group in groups)
                {
                    if (!other.groups.TryGetValue(group.Key, out var otherValues) || group.Value.Count != otherValues.Count)
                    {
                        return false;
                    }

                    foreach (var value in group.Value)
                    {
                        if (!otherValues.TryGetValue(value.Key, out var otherCount) || value.Value != otherCount)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            public override bool Equals(object obj) => Equals(obj as ShallowRepresentation);

            public override int GetHashCode() => hashCode;
        }

        private sealed class OpaqueElement : IEquatable<OpaqueElement>
        {
            private readonly OpaqueElement[] children;
            private readonly int hashCode;
            private readonly bool isSimpleType;
            private readonly string key;
            private readonly string operatorValue;
            private readonly string value;

            private OpaqueElement(string key, string operatorValue, string value, bool isSimpleType, OpaqueElement[] children)
            {
                this.key = key ?? string.Empty;
                this.operatorValue = operatorValue ?? string.Empty;
                this.value = value ?? string.Empty;
                this.isSimpleType = isSimpleType;
                this.children = children;

                var hash = new HashCode();
                hash.Add(ScriptTokenComparer.Instance.GetHashCode(this.key));
                hash.Add(this.operatorValue, StringComparer.Ordinal);
                hash.Add(isSimpleType);
                if (isSimpleType)
                {
                    hash.Add(ScriptTokenComparer.Instance.GetHashCode(this.value));
                }
                else
                {
                    foreach (var child in children)
                    {
                        hash.Add(child.hashCode);
                    }
                }

                hashCode = hash.ToHashCode();
            }

            public static OpaqueElement Create(IScriptElement element)
            {
                var children = element.IsSimpleType ? [] : element.Values?.Select(Create).ToArray() ?? [];
                return new OpaqueElement(element.Key, element.Operator, element.Value, element.IsSimpleType, children);
            }

            public bool Equals(OpaqueElement other)
            {
                if (other == null || hashCode != other.hashCode || isSimpleType != other.isSimpleType ||
                    !ScriptTokenComparer.Instance.Equals(key, other.key) || !string.Equals(operatorValue, other.operatorValue, StringComparison.Ordinal))
                {
                    return false;
                }

                if (isSimpleType)
                {
                    return ScriptTokenComparer.Instance.Equals(value, other.value);
                }

                if (children.Length != other.children.Length)
                {
                    return false;
                }

                for (var i = 0; i < children.Length; i++)
                {
                    if (!children[i].Equals(other.children[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj) => Equals(obj as OpaqueElement);

            public override int GetHashCode() => hashCode;
        }

        private sealed class ScriptTokenComparer : IEqualityComparer<string>
        {
            public static ScriptTokenComparer Instance { get; } = new();

            public bool Equals(string left, string right)
            {
                left ??= string.Empty;
                right ??= string.Empty;
                var leftQuoted = IsQuoted(left);
                return leftQuoted == IsQuoted(right) && string.Equals(left, right,
                    leftQuoted ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string value)
            {
                value ??= string.Empty;
                return IsQuoted(value) ? StringComparer.Ordinal.GetHashCode(value) : StringComparer.OrdinalIgnoreCase.GetHashCode(value);
            }

            private static bool IsQuoted(string value) => value.Length >= 2 && value[0] == '"' && value[^1] == '"';
        }
    }
}
