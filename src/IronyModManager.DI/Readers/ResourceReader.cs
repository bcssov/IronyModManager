// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 04-06-2020
// ***********************************************************************
// <copyright file="ResourceReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reflection;
using IronyModManager.DI.Assemblies;

namespace IronyModManager.DI.Readers
{
    /// <summary>
    /// Class ResourceReader.
    /// </summary>
    internal static class ResourceReader
    {
        #region Methods

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="ArgumentNullException">assembly</exception>
        public static byte[] GetPublicKey(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            var key = assembly.GetName().GetPublicKey();
            return key;
        }

        /// <summary>
        /// Gets the name of the strong.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns>StrongName.</returns>
        /// <exception cref="InvalidOperationException">publicKey</exception>
        /// <exception cref="ArgumentNullException">assembly</exception>
        /// <exception cref="ArgumentException">publicKey</exception>
        public static StrongName GetStrongName(byte[] publicKey, Assembly assembly)
        {
            if (publicKey == null || publicKey.Length == 0)
            {
                throw new InvalidOperationException("publicKey");
            }
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            var blob = new StrongNamePublicKeyBlob(publicKey);
            var assemblyName = assembly.GetName();
            var strongName = new StrongName(blob, assemblyName.Name, assemblyName.Version);
            return strongName;
        }

        #endregion Methods
    }
}
