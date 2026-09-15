// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************

using System;
using System.Reactive.Disposables;
using IronyModManager.Common;
using IronyModManager.Implementation.MessageBus;
using IronyModManager.Localization;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Owns collection import/export, patch-rename, and report progress presentation.
    /// </summary>
    public class CollectionOperationProgressCoordinator(ModExportProgressHandler modExportProgressHandler,
        PatchModRenameProgressHandler patchModRenameProgressHandler, ModReportExportHandler modReportExportHandler,
        ILocalizationManager localizationManager)
    {
        private IDisposable modExportProgress;

        /// <summary>
        /// Subscribes to collection import or export progress for the current operation.
        /// </summary>
        public virtual void SubscribeCollectionTransfer(long id, bool importing, CompositeDisposable disposables, Action<long, bool, string, string> showOverlay)
        {
            CompleteCollectionTransfer();
            modExportProgress = modExportProgressHandler.Subscribe(s =>
            {
                var messageResource = importing
                    ? LocalizationResources.Collection_Mods.Overlay_Importing_Message
                    : LocalizationResources.Collection_Mods.Overlay_Exporting_Message;
                var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Import_Export_Progress),
                    new { PercentDone = s.Progress.ToLocalizedPercentage() });
                showOverlay(id, true, localizationManager.GetResource(messageResource), overlayProgress);
            }).DisposeWith(disposables);
        }

        /// <summary>
        /// Ends the current collection transfer progress subscription.
        /// </summary>
        public virtual void CompleteCollectionTransfer()
        {
            modExportProgress?.Dispose();
            modExportProgress = null;
        }

        /// <summary>
        /// Subscribes to patch collection rename progress.
        /// </summary>
        public virtual IDisposable SubscribePatchRename(long id, Action<long, bool, string, string> showOverlay)
        {
            return patchModRenameProgressHandler.Subscribe(s =>
            {
                var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Rename_Message);
                var overlayProgress = IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.Overlay_Import_Export_Progress),
                    new { PercentDone = s.Percentage.ToLocalizedPercentage() });
                showOverlay(id, true, message, overlayProgress);
            });
        }

        /// <summary>
        /// Subscribes to hash-report import or export progress.
        /// </summary>
        public virtual IDisposable SubscribeReport(long id, bool importing, CompositeDisposable disposables, Action<long, bool, string, string> showOverlay)
        {
            return modReportExportHandler.Subscribe(s =>
            {
                if (importing)
                {
                    showOverlay(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ImportOverlay),
                        IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ProgressImport),
                            new { Progress = s.Percentage.ToLocalizedPercentage(), Count = s.Step, TotalCount = 2 }));
                }
                else
                {
                    showOverlay(id, true, localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ExportOverlay),
                        IronyFormatter.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.FileHash.ProgressExport),
                            new { Progress = s.Percentage.ToLocalizedPercentage() }));
                }
            }).DisposeWith(disposables);
        }
    }
}
