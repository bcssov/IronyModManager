// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Implementation.AppState;
using IronyModManager.Services.Common;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns the Installed Mods search and mutually exclusive sort presentation state.
    /// </summary>
    public class InstalledModsPresentationCoordinator(IAppStateService appStateService, SortOrderControlViewModel modSelectedSortOrder,
        SortOrderControlViewModel modNameSortOrder, SortOrderControlViewModel modVersionSortOrder, SearchModsControlViewModel filterMods)
    {
        /// <summary>
        /// The mod name sort key.
        /// </summary>
        public const string ModNameKey = "modName";

        /// <summary>
        /// The selected-state sort key.
        /// </summary>
        public const string ModSelectedKey = "modSelected";

        /// <summary>
        /// The version sort key.
        /// </summary>
        public const string ModVersionKey = "modVersion";

        private readonly Dictionary<string, SortOrderControlViewModel> sortOrders = [];

        /// <summary>
        /// Gets the search control.
        /// </summary>
        public virtual SearchModsControlViewModel FilterMods => filterMods;

        /// <summary>
        /// Gets the name sort control.
        /// </summary>
        public virtual SortOrderControlViewModel ModNameSortOrder => modNameSortOrder;

        /// <summary>
        /// Gets the selected-state sort control.
        /// </summary>
        public virtual SortOrderControlViewModel ModSelectedSortOrder => modSelectedSortOrder;

        /// <summary>
        /// Gets the version sort control.
        /// </summary>
        public virtual SortOrderControlViewModel ModVersionSortOrder => modVersionSortOrder;

        /// <summary>
        /// Initializes controls from persisted presentation state.
        /// </summary>
        public virtual void Initialize(string modName, string modVersion, string modSelected, string filterWatermark)
        {
            var appState = appStateService.Get();
            sortOrders.Clear();
            InitDefaultSortOrder(ModNameKey, ModNameSortOrder, Implementation.SortOrder.Asc, modName, appState);
            InitDefaultSortOrder(ModVersionKey, ModVersionSortOrder, Implementation.SortOrder.None, modVersion, appState);
            InitDefaultSortOrder(ModSelectedKey, ModSelectedSortOrder, Implementation.SortOrder.None, modSelected, appState);
            FilterMods.Text = appState?.InstalledModsSearchTerm;
            FilterMods.WatermarkText = filterWatermark;
        }

        /// <summary>
        /// Updates localized labels without changing state.
        /// </summary>
        public virtual void UpdateLocalization(string modName, string modVersion, string modSelected, string filterWatermark)
        {
            ModNameSortOrder.Text = modName;
            ModVersionSortOrder.Text = modVersion;
            ModSelectedSortOrder.Text = modSelected;
            FilterMods.WatermarkText = filterWatermark;
        }

        /// <summary>
        /// Gets the currently active sort key.
        /// </summary>
        public virtual string GetActiveSortKey()
        {
            return sortOrders.FirstOrDefault(p => p.Value.SortOrder != Implementation.SortOrder.None).Key;
        }

        /// <summary>
        /// Gets the sort control for a key.
        /// </summary>
        public virtual SortOrderControlViewModel GetSortOrder(string key)
        {
            return sortOrders.GetValueOrDefault(key);
        }

        /// <summary>
        /// Enforces mutually exclusive sort selection.
        /// </summary>
        public virtual void ResetOtherSortOrders(SortOrderControlViewModel activeSortOrder)
        {
            foreach (var sort in sortOrders.Values.Where(p => p != activeSortOrder))
            {
                sort.SetSortOrder(Implementation.SortOrder.None);
            }
        }

        /// <summary>
        /// Saves search and sort presentation state.
        /// </summary>
        public virtual void SaveState()
        {
            var state = appStateService.Get();
            state.InstalledModsSearchTerm = FilterMods.Text;
            var sortModel = sortOrders.FirstOrDefault(p => p.Value.SortOrder != Implementation.SortOrder.None);
            state.InstalledModsSortColumn = sortModel.Key;
            state.InstalledModsSortMode = (int)sortModel.Value.SortOrder;
            appStateService.Save(state);
        }

        private void InitDefaultSortOrder(string key, SortOrderControlViewModel viewModel, Implementation.SortOrder defaultOrder, string text, Models.Common.IAppState appState)
        {
            if (!string.IsNullOrWhiteSpace(appState.InstalledModsSortColumn) && Enum.IsDefined(typeof(Implementation.SortOrder), appState.InstalledModsSortMode))
            {
                viewModel.SortOrder = key.Equals(appState.InstalledModsSortColumn)
                    ? (Implementation.SortOrder)appState.InstalledModsSortMode
                    : Implementation.SortOrder.None;
            }
            else
            {
                viewModel.SortOrder = defaultOrder;
            }

            viewModel.Text = text;
            sortOrders[key] = viewModel;
        }
    }
}
