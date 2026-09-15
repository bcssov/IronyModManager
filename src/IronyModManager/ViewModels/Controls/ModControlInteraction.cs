// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System;
using System.Threading.Tasks;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;
using IronyModManager.Shared;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns the common user-interaction boundary used by the mod controls.
    /// </summary>
    public class ModControlInteraction(IIDGenerator idGenerator, ILocalizationManager localizationManager,
        INotificationAction notificationAction, IAppAction appAction)
    {
        /// <summary>
        /// Allocates an identifier for an interactive operation.
        /// </summary>
        public virtual long BeginOperation() => idGenerator.GetNextId();

        /// <summary>
        /// Resolves localized presentation text.
        /// </summary>
        public virtual string GetText(string key) => localizationManager.GetResource(key);

        /// <summary>
        /// Shows a user notification.
        /// </summary>
        public virtual void Notify(string title, string message, NotificationType type, int timeout = 5,
            Action onClick = null) => notificationAction.ShowNotification(title, message, type, timeout, onClick);

        /// <summary>
        /// Shows an interactive prompt.
        /// </summary>
        public virtual Task<bool> PromptAsync(string title, string header, string message, NotificationType type,
            PromptType promptType = PromptType.YesNo) =>
            notificationAction.ShowPromptAsync(title, header, message, type, promptType);

        /// <summary>
        /// Opens a command or path through the platform interaction boundary.
        /// </summary>
        public virtual Task<bool> OpenAsync(string command) => appAction.OpenAsync(command);

        /// <summary>
        /// Opens an application through Flatpak.
        /// </summary>
        public virtual Task<bool> OpenFlatpakAsync(params string[] commands) => appAction.OpenFlatpakAsync(commands);

        /// <summary>
        /// Copies text to the clipboard.
        /// </summary>
        public virtual Task<bool> CopyAsync(string text) => appAction.CopyAsync(text);

        /// <summary>
        /// Reads text from the clipboard.
        /// </summary>
        public virtual Task<string> GetClipboardTextAsync() => appAction.GetAsync();

        /// <summary>
        /// Launches a game through the platform interaction boundary.
        /// </summary>
        public virtual Task<bool> RunGameAsync(bool createSteamFile, string path, string steamRoot,
            string steamProtonVersion, int appId, string args = IronyModManager.Shared.Constants.EmptyParam) =>
            appAction.RunGameAsync(createSteamFile, path, steamRoot, steamProtonVersion, appId, args);

        /// <summary>
        /// Exits the application after a successful launch hand-off.
        /// </summary>
        public virtual Task ExitApplicationAsync() => appAction.ExitAppAsync();
    }
}
