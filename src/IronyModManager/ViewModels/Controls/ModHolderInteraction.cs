// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System;
using IronyModManager.Implementation.Actions;
using IronyModManager.Common;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;
using IronyModManager.Shared;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns Mod Holder user interaction and presentation of holder workflow failures.
    /// </summary>
    public class ModHolderInteraction(IIDGenerator idGenerator, ILocalizationManager localizationManager,
        INotificationAction notificationAction, IAppAction appAction, ILogger logger)
        : ModControlInteraction(idGenerator, localizationManager, notificationAction, appAction)
    {
        /// <summary>
        /// Reports and presents a collection-save failure without leaking logging policy into the ViewModel.
        /// </summary>
        public virtual void ReportSavingFailure(Exception exception)
        {
            logger.Error(exception);
            Notify(GetText(LocalizationResources.SavingError.Title),
                GetText(LocalizationResources.SavingError.Message), NotificationType.Error, 30);
        }
    }
}
