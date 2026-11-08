// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// ***********************************************************************

using System.Collections.Generic;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// Compares definitions using the bounded first-level equivalence contract.
    /// </summary>
    public interface IDefinitionShallowComparer
    {
        /// <summary>
        /// Compares and groups definitions without exposing parser implementation details.
        /// </summary>
        /// <param name="definitions">The definitions to compare.</param>
        /// <returns>The comparison result.</returns>
        DefinitionShallowComparisonResult Compare(IReadOnlyCollection<IDefinition> definitions);
    }

    /// <summary>
    /// Describes the outcome of a shallow definition comparison.
    /// </summary>
    public enum DefinitionShallowComparisonStatus
    {
        /// <summary>
        /// At least one definition could not be represented safely.
        /// </summary>
        Unsupported,

        /// <summary>
        /// All supplied definitions are shallow-equivalent.
        /// </summary>
        Equivalent,

        /// <summary>
        /// More than one shallow-equivalence class remains.
        /// </summary>
        Different
    }

    /// <summary>
    /// Contains only the domain-level result of a shallow comparison.
    /// </summary>
    public sealed class DefinitionShallowComparisonResult(
        DefinitionShallowComparisonStatus status,
        IReadOnlyList<IReadOnlyList<IDefinition>> equivalentGroups)
    {
        /// <summary>
        /// Gets the comparison status.
        /// </summary>
        public DefinitionShallowComparisonStatus Status { get; } = status;

        /// <summary>
        /// Gets the collision-verified equivalence classes when comparison is supported.
        /// </summary>
        public IReadOnlyList<IReadOnlyList<IDefinition>> EquivalentGroups { get; } = equivalentGroups;
    }
}
