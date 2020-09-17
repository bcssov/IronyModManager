// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-16-2020
//
// Last Modified By : Mario
// Last Modified On : 09-17-2020
// ***********************************************************************
// <copyright file="UpdaterCleanup.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.Implementation.Updater;
using IronyModManager.Shared;
using Newtonsoft.Json;

namespace IronyModManager
{
    /// <summary>
    /// Class UpdateCleanup.
    /// Implements the <see cref="IronyModManager.Shared.PostStartup" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.PostStartup" />
    public class UpdaterCleanup : PostStartup
    {
        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            CleanupUpdaterAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// cleanup updater as an asynchronous operation.
        /// </summary>
        private async Task CleanupUpdaterAsync()
        {
            var path = StaticResources.GetUpdaterPath();
            if (Directory.Exists(path))
            {
                bool cleanup = true;
                var settingsFileName = Path.Combine(path, Constants.UpdateSettings);
                if (File.Exists(settingsFileName))
                {
                    var text = await File.ReadAllTextAsync(settingsFileName);
                    var settings = JsonConvert.DeserializeObject<UpdateSettings>(text);
                    cleanup = settings.Updated || settings.IsInstaller;
                }
                if (cleanup)
                {
                    await Task.Delay(5000);
                    try
                    {
                        Directory.Delete(path, true);
                    }
                    catch (Exception ex)
                    {
                        var logger = DIResolver.Get<ILogger>();
                        logger.Error(ex);
                    }
                }
            }
        }

        #endregion Methods
    }
}
