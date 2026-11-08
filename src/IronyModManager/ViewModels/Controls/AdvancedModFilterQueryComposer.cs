// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Values exposed by the bounded boolean controls in the advanced mod filter.
    /// </summary>
    public enum AdvancedFilterBooleanState
    {
        Any,
        Yes,
        No
    }

    /// <summary>
    /// Localized syntax used by the existing mod-filter parser.
    /// </summary>
    public class AdvancedModFilterQuerySyntax
    {
        public string Achievements { get; set; }
        public string Local { get; set; }
        public string Negate { get; set; }
        public string No { get; set; }
        public string OrSeparator { get; set; }
        public string Paradox { get; set; }
        public string Selected { get; set; }
        public string Source { get; set; }
        public string StatementSeparator { get; set; }
        public string Steam { get; set; }
        public string ValueSeparator { get; set; }
        public string Version { get; set; }
        public string Yes { get; set; }
    }

    /// <summary>
    /// Draft state represented by the bounded advanced mod-filter UI.
    /// </summary>
    public class AdvancedModFilterState
    {
        public AdvancedFilterBooleanState Achievements { get; set; }
        public bool Local { get; set; }
        public string PlainText { get; set; } = string.Empty;
        public bool Paradox { get; set; }
        public AdvancedFilterBooleanState Selected { get; set; }
        public bool Steam { get; set; }
        public string Version { get; set; } = string.Empty;
        public bool VersionExcluded { get; set; }
    }

    /// <summary>
    /// Composes and recognizes the lossless subset of existing query text exposed by the advanced UI.
    /// </summary>
    public class AdvancedModFilterQueryComposer
    {
        /// <summary>
        /// Composes ordinary localized mod-filter text.
        /// </summary>
        public virtual string Compose(AdvancedModFilterState state, AdvancedModFilterQuerySyntax syntax)
        {
            var statements = new List<string>();
            Add(statements, state.PlainText);

            var sources = new List<string>();
            if (state.Steam)
            {
                sources.Add(syntax.Steam);
            }
            if (state.Paradox)
            {
                sources.Add(syntax.Paradox);
            }
            if (state.Local)
            {
                sources.Add(syntax.Local);
            }
            if (sources.Count != 0)
            {
                statements.Add($"{syntax.Source}{syntax.ValueSeparator}{string.Join($" {syntax.OrSeparator} ", sources)}");
            }

            AddBoolean(statements, syntax.Selected, state.Selected, syntax);
            AddBoolean(statements, syntax.Achievements, state.Achievements, syntax);
            if (!string.IsNullOrWhiteSpace(state.Version))
            {
                var versions = state.Version.Split(new[] { syntax.OrSeparator }, StringSplitOptions.None)
                    .Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => state.VersionExcluded ? $"{syntax.Negate}{x}" : x).ToList();
                if (versions.Count != 0)
                {
                    statements.Add($"{syntax.Version}{syntax.ValueSeparator}{string.Join($" {syntax.OrSeparator} ", versions)}");
                }
            }

            return string.Join($" {syntax.StatementSeparator} ", statements);
        }

        /// <summary>
        /// Tries to initialize the bounded UI only when the current text can be preserved losslessly.
        /// </summary>
        public virtual bool TryParse(string query, AdvancedModFilterQuerySyntax syntax, out AdvancedModFilterState state)
        {
            state = new AdvancedModFilterState();
            if (string.IsNullOrWhiteSpace(query))
            {
                return true;
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var rawStatement in query.Split(new[] { syntax.StatementSeparator }, StringSplitOptions.None))
            {
                var statement = rawStatement.Trim();
                if (string.IsNullOrWhiteSpace(statement))
                {
                    return false;
                }

                var separatorIndex = statement.IndexOf(syntax.ValueSeparator, StringComparison.Ordinal);
                if (separatorIndex < 0)
                {
                    if (!string.IsNullOrEmpty(state.PlainText))
                    {
                        return false;
                    }
                    state.PlainText = statement;
                    continue;
                }

                var field = statement.Substring(0, separatorIndex).Trim();
                var value = statement.Substring(separatorIndex + syntax.ValueSeparator.Length).Trim();
                if (string.IsNullOrWhiteSpace(value) || !seen.Add(field))
                {
                    return false;
                }
                if (field.Equals(syntax.Source, StringComparison.OrdinalIgnoreCase))
                {
                    if (!TryParseSources(value, syntax, state))
                    {
                        return false;
                    }
                }
                else if (field.Equals(syntax.Selected, StringComparison.OrdinalIgnoreCase))
                {
                    if (!TryParseBoolean(value, syntax, out var selected))
                    {
                        return false;
                    }
                    state.Selected = selected;
                }
                else if (field.Equals(syntax.Achievements, StringComparison.OrdinalIgnoreCase))
                {
                    if (!TryParseBoolean(value, syntax, out var achievements))
                    {
                        return false;
                    }
                    state.Achievements = achievements;
                }
                else if (field.Equals(syntax.Version, StringComparison.OrdinalIgnoreCase))
                {
                    if (!TryParseVersions(value, syntax, out var version, out var excluded))
                    {
                        return false;
                    }
                    state.Version = version;
                    state.VersionExcluded = excluded;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static void Add(ICollection<string> statements, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                statements.Add(value.Trim());
            }
        }

        private static void AddBoolean(ICollection<string> statements, string field, AdvancedFilterBooleanState value,
            AdvancedModFilterQuerySyntax syntax)
        {
            if (value != AdvancedFilterBooleanState.Any)
            {
                statements.Add($"{field}{syntax.ValueSeparator}{(value == AdvancedFilterBooleanState.Yes ? syntax.Yes : syntax.No)}");
            }
        }

        private static bool TryParseBoolean(string value, AdvancedModFilterQuerySyntax syntax, out AdvancedFilterBooleanState result)
        {
            if (value.Equals(syntax.Yes, StringComparison.OrdinalIgnoreCase))
            {
                result = AdvancedFilterBooleanState.Yes;
                return true;
            }
            if (value.Equals(syntax.No, StringComparison.OrdinalIgnoreCase))
            {
                result = AdvancedFilterBooleanState.No;
                return true;
            }
            result = AdvancedFilterBooleanState.Any;
            return false;
        }

        private static bool TryParseSources(string value, AdvancedModFilterQuerySyntax syntax, AdvancedModFilterState state)
        {
            foreach (var rawSource in value.Split(new[] { syntax.OrSeparator }, StringSplitOptions.None))
            {
                var source = rawSource.Trim();
                if (source.Equals(syntax.Steam, StringComparison.OrdinalIgnoreCase) && !state.Steam)
                {
                    state.Steam = true;
                }
                else if (source.Equals(syntax.Paradox, StringComparison.OrdinalIgnoreCase) && !state.Paradox)
                {
                    state.Paradox = true;
                }
                else if (source.Equals(syntax.Local, StringComparison.OrdinalIgnoreCase) && !state.Local)
                {
                    state.Local = true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private static bool TryParseVersions(string value, AdvancedModFilterQuerySyntax syntax, out string version, out bool excluded)
        {
            version = string.Empty;
            excluded = false;
            var values = value.Split(new[] { syntax.OrSeparator }, StringSplitOptions.None).Select(x => x.Trim()).ToList();
            if (values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
            {
                return false;
            }

            var negated = values.Select(x => !string.IsNullOrEmpty(syntax.Negate) &&
                x.StartsWith(syntax.Negate, StringComparison.OrdinalIgnoreCase)).ToList();
            if (negated.Any(x => x) && !negated.All(x => x))
            {
                return false;
            }

            excluded = negated.All(x => x);
            if (excluded)
            {
                values = values.Select(x => x.Substring(syntax.Negate.Length).Trim()).ToList();
                if (values.Any(string.IsNullOrWhiteSpace))
                {
                    return false;
                }
            }

            version = string.Join($" {syntax.OrSeparator} ", values);
            return true;
        }
    }
}
