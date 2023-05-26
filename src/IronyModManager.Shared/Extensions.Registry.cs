
// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 05-26-2023
//
// Last Modified By : Mario
// Last Modified On : 05-26-2023
// ***********************************************************************
// <copyright file="Extensions.Registry.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace IronyModManager.Shared
{

    /// <summary>
    /// Class Extensions.
    /// </summary>
    public partial class Extensions
    {
        #region Methods

        /// <summary>
        /// Gets the registry key.
        /// </summary>
        /// <param name="registryHive">The registry hive.</param>
        /// <param name="key">The key.</param>
        /// <returns>RegistryKey.</returns>
        [SupportedOSPlatform("windows")]
        public static RegistryKey GetRegistryKey(this RegistryHive registryHive, string key)
        {
            using var hive64 = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64);
            var result = GetSubKey(hive64, key);
            if (result != null)
            {
                return result;
            }
            using var hive32 = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry32);
            result = GetSubKey(hive32, key);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Gets the sub key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        /// <param name="key">The key.</param>
        /// <returns>RegistryKey.</returns>
        [SupportedOSPlatform("windows")]
        private static RegistryKey GetSubKey(RegistryKey registryKey, string key)
        {
            var regKey = registryKey.OpenSubKey(key);
            if (regKey != null)
            {
                return regKey;
            }
            regKey?.Dispose();
            return null;
        }

        #endregion Methods
    }
}
