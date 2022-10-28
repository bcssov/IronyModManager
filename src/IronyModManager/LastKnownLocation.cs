// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-28-2022
//
// Last Modified By : Mario
// Last Modified On : 10-28-2022
// ***********************************************************************
// <copyright file="LastKnownLocation.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.Shared;
using Newtonsoft.Json;

namespace IronyModManager
{
    /// <summary>
    /// Class LastKnownLocation.
    /// Implements the <see cref="PostStartup" />
    /// </summary>
    /// <seealso cref="PostStartup" />
    public class LastKnownLocation : PostStartup
    {
        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            StoreInfoAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Store information as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static Task StoreInfoAsync()
        {
            try
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(typeof(LastKnownLocation).Assembly.Location);
                var info = new Info()
                {
                    ProductVersion = versionInfo.ProductVersion,
                    FileVersion = versionInfo.FileVersion,
                    Path = AppDomain.CurrentDomain.BaseDirectory
                };
                var data = JsonConvert.SerializeObject(info, Formatting.Indented);
                if (!Directory.Exists(StaticResources.GetLastKnownLocationInfo()))
                {
                    Directory.CreateDirectory(StaticResources.GetLastKnownLocationInfo());
                }
                return File.WriteAllTextAsync(Path.Combine(StaticResources.GetLastKnownLocationInfo(), Constants.LastRunLocation), data);
            }
            catch
            {
                // Not too important to log
            }
            return Task.CompletedTask;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class Info.
        /// </summary>
        private class Info
        {
            #region Properties

            /// <summary>
            /// Gets or sets the file version.
            /// </summary>
            /// <value>The file version.</value>
            public string FileVersion { get; set; }

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            public string Path { get; set; }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            /// <value>The version.</value>
            public string ProductVersion { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
