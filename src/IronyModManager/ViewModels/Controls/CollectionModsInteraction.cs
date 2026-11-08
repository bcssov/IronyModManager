// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System.Threading.Tasks;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns Collection Mods user interaction, including collection file selection.
    /// </summary>
    public class CollectionModsInteraction(IIDGenerator idGenerator, ILocalizationManager localizationManager,
        INotificationAction notificationAction, IAppAction appAction, IFileDialogAction fileDialogAction)
        : ModControlInteraction(idGenerator, localizationManager, notificationAction, appAction)
    {
        /// <summary>
        /// Selects an existing collection-related file.
        /// </summary>
        public virtual Task<string> SelectFileAsync(string title, string initialFileName, params string[] extensions) =>
            fileDialogAction.OpenDialogAsync(title, initialFileName, extensions);

        /// <summary>
        /// Selects a destination for a collection-related file.
        /// </summary>
        public virtual Task<string> SelectSavePathAsync(string title, string initialFileName,
            params string[] extensions) => fileDialogAction.SaveDialogAsync(title, initialFileName, extensions);
    }
}
