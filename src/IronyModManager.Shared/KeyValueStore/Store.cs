
// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-13-2023
//
// Last Modified By : Mario
// Last Modified On : 06-24-2023
// ***********************************************************************
// <copyright file="Store.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jering.KeyValueStore;

namespace IronyModManager.Shared.KeyValueStore
{

    /// <summary>
    /// Class Store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Store<T> : IDisposable where T : class
    {
        #region Fields

        /// <summary>
        /// The options
        /// </summary>
        private readonly MixedStorageKVStoreOptions options;

        /// <summary>
        /// The storage
        /// </summary>
        private readonly MixedStorageKVStore<string, T> storage;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Store{T}" /> class.
        /// </summary>
        /// <param name="cacheDirectory">The cache directory.</param>
        /// <param name="loadType">Type of the load.</param>
        public Store(string cacheDirectory, Func<string, Type> loadType = null)
        {
            options = new MixedStorageKVStoreOptions()
            {
                DeleteLogOnClose = true,
                LogDirectory = cacheDirectory,
                MessagePackSerializerOptions = new StoreOptions(loadType)
            };
            storage = new MixedStorageKVStore<string, T>(options);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Delete as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public async Task<bool> DeleteAsync(string key)
        {
            var result = await storage.DeleteAsync(key);
            return result == FASTER.core.Status.OK;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
                disposed = true;
            }
        }

        /// <summary>
        /// Insert as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public async Task<bool> InsertAsync(string key, T value)
        {
            await storage.UpsertAsync(key, value);
            return true;
        }

        /// <summary>
        /// Read as an asynchronous operation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A Task&lt;T&gt; representing the asynchronous operation.</returns>
        public async Task<T> ReadAsync(string key)
        {
            var result = await storage.ReadAsync(key);
            if (result.Item1 == FASTER.core.Status.OK)
            {
                return result.Item2;
            }
            return default;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                storage?.Dispose();
            }
        }

        #endregion Methods
    }
}
