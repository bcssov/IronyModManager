// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System.Threading.Tasks;
using IronyModManager.Common;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Presents one user-action-level outcome for collection application.
    /// </summary>
    public class ModApplyResultPresenter(ILocalizationManager localizationManager, INotificationAction notificationAction)
    {
        /// <summary>
        /// Shows the apply result and a single warning when virtual collection members were skipped.
        /// </summary>
        public virtual async Task ShowAsync(IModCollection collection, ModApplyResult result, bool launchingGame = false)
        {
            var notificationType = result?.Succeeded == true ? NotificationType.Success : NotificationType.Error;
            var titleResource = result?.Succeeded == true
                ? LocalizationResources.Notifications.CollectionApplied.Title
                : LocalizationResources.Notifications.CollectionNotApplied.Title;
            var messageResource = result?.Succeeded == true
                ? LocalizationResources.Notifications.CollectionApplied.Message
                : LocalizationResources.Notifications.CollectionNotApplied.Message;
            var title = localizationManager.GetResource(titleResource);
            var message = IronyFormatter.Format(localizationManager.GetResource(messageResource),
                new { CollectionName = collection?.Name ?? string.Empty });
            notificationAction.ShowNotification(title, message, notificationType, 5);

            if (result?.Succeeded == true && result.SkippedVirtualMods > 0)
            {
                var warningTitle = localizationManager.GetResource(LocalizationResources.Notifications.CollectionAppliedMissingMods.Title);
                var warningMessage = localizationManager.GetResource(LocalizationResources.Notifications.CollectionAppliedMissingMods.Message);
                if (launchingGame)
                {
                    await notificationAction.ShowPromptAsync(warningTitle, warningTitle, warningMessage,
                        NotificationType.Warning, PromptType.OK);
                }
                else
                {
                    notificationAction.ShowNotification(warningTitle, warningMessage, NotificationType.Warning, 10);
                }
            }
        }
    }
}
