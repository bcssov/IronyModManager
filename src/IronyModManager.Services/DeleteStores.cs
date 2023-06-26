
// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 06-26-2023
//
// Last Modified By : Mario
// Last Modified On : 06-26-2023
// ***********************************************************************
// <copyright file="DeleteStores.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.IO.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{

    /// <summary>
    /// Class DeleteStores.
    /// Implements the <see cref="PostStartup" />
    /// </summary>
    /// <seealso cref="PostStartup" />
    public class DeleteStores : PostStartup
    {
        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            CleanupStoreAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Cleanup store as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static Task CleanupStoreAsync()
        {
            try
            {
                var store = DIResolver.Get<IStorageProvider>();
                var path = Path.Combine(store.GetRootStoragePath(), Parser.Common.Constants.StoreCacheRootRolder);
                if (Directory.Exists(path))
                {
                    DiskOperations.DeleteDirectory(path, true);
                }
            }
            catch
            {
            }
            return Task.CompletedTask;
        }

        #endregion Methods
    }
}
