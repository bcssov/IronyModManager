// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using IronyModManager.Implementation.AppState;
using IronyModManager.Services.Common;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns Collection Mods search, sort, selection, and scroll presentation state.
    /// </summary>
    public class CollectionModsPresentationCoordinator(IAppStateService appStateService, IScrollState scrollState,
        SearchModsControlViewModel searchMods, SortOrderControlViewModel modNameSortOrder)
    {
        /// <summary>
        /// The persisted name-sort key.
        /// </summary>
        public const string ModNameKey = "modName";

        /// <summary>
        /// Gets the search control.
        /// </summary>
        public virtual SearchModsControlViewModel SearchMods => searchMods;

        /// <summary>
        /// Gets the name sort control.
        /// </summary>
        public virtual SortOrderControlViewModel ModNameSortOrder => modNameSortOrder;

        /// <summary>
        /// Restores persisted presentation state and configures the controls for the current locale.
        /// </summary>
        public virtual (bool JumpOnPositionChange, string SelectedDescriptor) Restore(string searchWatermark, string modName)
        {
            var state = appStateService.Get();
            SearchMods.WatermarkText = searchWatermark;
            SearchMods.Text = state.CollectionModsSearchTerm;
            ModNameSortOrder.Text = modName;
            return (state.CollectionJumpOnPositionChange, state.CollectionModsSelectedMod);
        }

        /// <summary>
        /// Updates locale-dependent presentation labels without disturbing user state.
        /// </summary>
        public virtual void UpdateLocalization(string searchWatermark, string modName)
        {
            SearchMods.WatermarkText = searchWatermark;
            ModNameSortOrder.Text = modName;
        }

        /// <summary>
        /// Persists collection-list presentation state.
        /// </summary>
        public virtual void Save(string selectedDescriptor, bool jumpOnPositionChange, bool preserveSelection)
        {
            var state = appStateService.Get();
            if (!preserveSelection)
            {
                state.CollectionModsSelectedMod = selectedDescriptor;
                state.CollectionModsSearchTerm = SearchMods.Text;
            }

            state.CollectionModsSortColumn = ModNameKey;
            state.CollectionJumpOnPositionChange = jumpOnPositionChange;
            appStateService.Save(state);
        }

        /// <summary>
        /// Temporarily disables or restores list scrolling while queued reordering is applied.
        /// </summary>
        public virtual void SetScrollState(bool enabled)
        {
            scrollState.SetState(enabled);
        }
    }
}
