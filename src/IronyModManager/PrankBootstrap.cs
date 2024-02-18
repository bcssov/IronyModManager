// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-17-2024
//
// Last Modified By : Mario
// Last Modified On : 02-18-2024
// ***********************************************************************
// <copyright file="PrankBootstrap.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.DI;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization;
using IronyModManager.Services.Common;
using IronyModManager.Shared;

namespace IronyModManager
{
    /// <summary>
    /// Class PrankBootstrap.
    /// Implements the <see cref="PostStartup" />
    /// </summary>
    /// <seealso cref="PostStartup" />
    public class PrankBootstrap : PostStartup
    {
        #region Methods

        /// <summary>
        /// On post startup callback.
        /// </summary>
        public override void OnPostStartup()
        {
            Task.Run(InitPrankAsync);
        }

        /// <summary>
        /// Initialize the prank async.
        /// </summary>
        /// <returns>A Task.<see cref="Task" /></returns>
        private static async Task InitPrankAsync()
        {
            var prankDate = new DateTime(DateTime.Today.Year, 4, 1);
            var appState = DIResolver.Get<IAppStateService>();
            var state = appState.Get();
            if (DateTime.Today.IsDateSame(prankDate) && state.LastPrankCheck.GetValueOrDefault().Year != DateTime.Today.Year)
            {
                await Task.Delay(10000);
                var main = Helpers.GetMainWindow();
                while (main == null)
                {
                    main = Helpers.GetMainWindow();
                    await Task.Delay(250);
                }

                var notificationAction = DIResolver.Get<INotificationAction>();
                var locManager = DIResolver.Get<ILocalizationManager>();
                var title = locManager.GetResource(LocalizationResources.JokeError.Title);
                var message = locManager.GetResource(LocalizationResources.JokeError.Message);
                await Dispatcher.UIThread.SafeInvokeAsync(async () =>
                {
                    await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Error, PromptType.OK);
                });
                state.LastPrankCheck = DateTime.Now;
                appState.Save(state);
            }
        }

        #endregion Methods
    }
}
