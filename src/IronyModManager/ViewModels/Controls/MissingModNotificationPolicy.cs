// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Prevents duplicate missing-mod notifications across reactive collection reloads and imports.
    /// </summary>
    public class MissingModNotificationPolicy
    {
        private readonly Dictionary<string, string> shownWarnings = [];
        private string importWarningOwner;

        /// <summary>
        /// Gives the import-specific warning ownership of the next load for this collection.
        /// </summary>
        public virtual void BeginImport(string collectionName)
        {
            importWarningOwner = collectionName;
        }

        /// <summary>
        /// Returns whether the ordinary collection warning should be shown.
        /// </summary>
        public virtual bool ShouldShowOrdinaryWarning(string activeGameType, string collectionGameType,
            string collectionName, IEnumerable<string> missingModIdentities)
        {
            var key = $"{collectionGameType}:{collectionName}";
            var signature = string.Join("\n", (missingModIdentities ?? [])
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(p => p, StringComparer.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(signature))
            {
                shownWarnings.Remove(key);
                ConsumeImportOwnership(collectionName);
                return false;
            }

            if (ConsumeImportOwnership(collectionName))
            {
                shownWarnings[key] = signature;
                return false;
            }

            if (!string.Equals(activeGameType, collectionGameType, StringComparison.OrdinalIgnoreCase) ||
                shownWarnings.TryGetValue(key, out var previousSignature) && previousSignature == signature)
            {
                return false;
            }

            shownWarnings[key] = signature;
            return true;
        }

        private bool ConsumeImportOwnership(string collectionName)
        {
            if (!string.Equals(importWarningOwner, collectionName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            importWarningOwner = null;
            return true;
        }
    }
}
