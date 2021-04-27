// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-27-2021
//
// Last Modified By : Mario
// Last Modified On : 04-27-2021
// ***********************************************************************
// <copyright file="ConfigurationLoader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace IronyModManager.Implementation.Config
{
    /// <summary>
    /// Class ConfigurationLoader.
    /// </summary>
    public class ConfigurationLoader
    {
        #region Fields

        /// <summary>
        /// The application settings
        /// </summary>
        private readonly JObject appSettings;

        /// <summary>
        /// The application settings override
        /// </summary>
        private readonly JObject appSettingsOverride;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationLoader" /> class.
        /// </summary>
        /// <param name="appSettingsPath">The application settings path.</param>
        /// <param name="appSettingsOverridePath">The application settings override path.</param>
        public ConfigurationLoader(string appSettingsPath, string appSettingsOverridePath)
        {
            appSettings = ReadJson(appSettingsPath);
            appSettingsOverride = ReadJson(appSettingsOverridePath);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns>Stream.</returns>
        public Stream GetStream()
        {
            appSettings.Merge(appSettingsOverride, new JsonMergeSettings()
            {
                MergeArrayHandling = MergeArrayHandling.Replace,
                MergeNullValueHandling = MergeNullValueHandling.Merge,
                PropertyNameComparison = StringComparison.OrdinalIgnoreCase
            });
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(appSettings.ToString(Newtonsoft.Json.Formatting.Indented)));
            return ms;
        }

        /// <summary>
        /// Reads the json.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>JObject.</returns>
        private JObject ReadJson(string path)
        {
            if (File.Exists(path))
            {
                var content = File.ReadAllText(path);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    return JObject.Parse(content);
                }
            }
            return new JObject();
        }

        #endregion Methods
    }
}
