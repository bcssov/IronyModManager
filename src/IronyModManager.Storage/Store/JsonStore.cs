// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 12-06-2025
// ***********************************************************************
// <copyright file="JsonStore.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using IronyModManager.IO.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Store;
using Jot.Storage;
using Newtonsoft.Json;

#pragma warning disable IDE0130
namespace IronyModManager.Storage
#pragma warning restore IDE0130
{
    /// <summary>
    /// Class JsonStore.
    /// Implements the <see cref="Jot.Storage.IStore" />
    /// </summary>
    /// <seealso cref="Jot.Storage.IStore" />
    [ExcludeFromCoverage("Modified version of Jot item, no need for us to test it.")]
    internal class JsonStore : IStore
    {
        #region Fields

        /// <summary>
        /// The converters
        /// </summary>
        private static readonly JsonConverter[] converters = [new StoreConverter(), new DIConverter()];

        /// <summary>
        /// The root paths
        /// </summary>
        private static string rootPath;

        /// <summary>
        /// The storage item
        /// </summary>
        private static StorageItem storageItem;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the root path.
        /// </summary>
        /// <value>The root path.</value>
        protected internal static string RootPath
        {
            get
            {
                rootPath ??= InitPath();

                return rootPath;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Clears all.
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public void ClearAll()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Clears the data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        public void ClearData(string id)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IDictionary&lt;System.String, System.Object&gt;.</returns>
        public IDictionary<string, object> GetData(string id)
        {
            static List<StoreItem> readItems(string path)
            {
                List<StoreItem> items = null;
                if (File.Exists(path))
                {
                    try
                    {
                        var fileContents = File.ReadAllText(path);
                        items = JsonConvert.DeserializeObject<List<StoreItem>>(fileContents, converters);
                    }
                    catch { }
                }

                return items ??= [];
            }

            var filePath = GetFilePath(id, true);
            var tempFilePath = GetTempPath(filePath);
            var diskItems = readItems(filePath);
            var tempDiskItems = readItems(tempFilePath);

            var diskDateItem = diskItems.FirstOrDefault(p => p.Type == Store.Constants.StoreDateId);
            var tempDiskDateItem = tempDiskItems.FirstOrDefault(p => p.Type == Store.Constants.StoreDateId);
            var diskDate = diskDateItem != null ? (DateTime)diskDateItem.Value : DateTime.MinValue;
            var tempDiskDate = tempDiskDateItem != null ? (DateTime)tempDiskDateItem.Value : DateTime.MinValue;
            var storeItems = tempDiskDate > diskDate ? [.. tempDiskItems.Where(p => p != tempDiskDateItem)] : diskItems.Where(p => p != diskDateItem).ToList();

            return storeItems.ToDictionary(item => item.Name, item => item.Value);
        }

        /// <summary>
        /// Lists the ids.
        /// </summary>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public IEnumerable<string> ListIds()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="values">The values.</param>
        public void SetData(string id, IDictionary<string, object> values)
        {
            var filePath = GetFilePath(id);
            var tempFilePath = GetTempPath(filePath);
            var list = values.Select(kvp => new StoreItem { Name = kvp.Key, Value = kvp.Value, Type = FormatTypeName(kvp.Value) }).ToList();
            list.Add(new StoreItem { Name = Store.Constants.StoreDateId, Type = Store.Constants.StoreDateId, Value = DateTime.UtcNow });
            var serialized = JsonConvert.SerializeObject(list, new JsonSerializerSettings { Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.None });

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            File.WriteAllText(tempFilePath, serialized);
            File.Copy(tempFilePath, filePath, true);
            File.Delete(tempFilePath);
        }

        /// <summary>
        /// Formats the name of the type.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>System.String.</returns>
        private static string FormatTypeName(object instance)
        {
            Type type;
            if (instance is IProxyTargetAccessor proxy)
            {
                type = proxy.DynProxyGetTarget().GetType();
            }
            else
            {
                type = instance.GetType();
            }

            if (typeof(IPropertyChangedModel).IsAssignableFrom(type))
            {
                var name = type.FullName;
                var names = name!.Split(Store.Constants.Dot, StringSplitOptions.RemoveEmptyEntries);
                return string.Join(Store.Constants.Dot, names);
            }
            else if (typeof(IEnumerable<IPropertyChangedModel>).IsAssignableFrom(type))
            {
                // Sure why not make a breaking change
                var name = !type.IsArray ? type.GetGenericArguments().SingleOrDefault()!.FullName : type.GetElementType()!.FullName;

                var names = name!.Split(Store.Constants.Dot, StringSplitOptions.RemoveEmptyEntries);
                return $"{nameof(IEnumerable)}{Store.Constants.EnumerableOpenTag}{string.Join(Store.Constants.Dot, names)}{Store.Constants.EnumerableCloseTag}";
            }

            return type.FullName;
        }

        /// <summary>
        /// Gets the temporary path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        private static string GetTempPath(string filePath)
        {
            return $"{filePath}.tmp";
        }

        /// <summary>
        /// Initializes the path.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string InitPath()
        {
            var segment = string.Empty;

            var entryAssembly = Assembly.GetEntryAssembly();
            var titleAttribute = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(entryAssembly!, typeof(AssemblyTitleAttribute));
            if (!string.IsNullOrEmpty(titleAttribute!.Title))
            {
                segment = $"{titleAttribute.Title}\\";
            }

            var storagePath = DiskOperations.ResolveStoragePath();

            return Path.Combine(storagePath, segment).StandardizeDirectorySeparator();
        }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="lookForOlderVersion">if set to <c>true</c> [look for older version].</param>
        /// <returns>System.String.</returns>
        private string GetFilePath(string id, bool lookForOlderVersion = false)
        {
            var version = FileVersionInfo.GetVersionInfo(GetType().Assembly.Location);
            var mainPath = Path.Combine(RootPath, $"{id}_{version.FileMajorPart}.{version.FileMinorPart}{Shared.Constants.JsonExtension}");

            if (File.Exists(mainPath))
            {
                return mainPath;
            }

            if (lookForOlderVersion)
            {
                if (storageItem == null && Directory.Exists(mainPath))
                {
                    var dbs = new List<StorageItem>();
                    foreach (var item in Directory.EnumerateFiles(mainPath, $"*{Shared.Constants.JsonExtension}"))
                    {
                        if (item.Contains('_', StringComparison.OrdinalIgnoreCase))
                        {
                            var versionData = item.Split("_", StringSplitOptions.RemoveEmptyEntries)[1].Replace(Shared.Constants.JsonExtension, string.Empty).Trim();
                            if (System.Version.TryParse(versionData, out var parsedVersion))
                            {
                                dbs.Add(new StorageItem { FileName = item, Version = parsedVersion });
                            }
                            else
                            {
                                dbs.Add(new StorageItem { FileName = item, Version = new System.Version(0, 0, 0, 0) });
                            }
                        }
                        else
                        {
                            dbs.Add(new StorageItem { FileName = item, Version = new System.Version(0, 0, 0, 0) });
                        }
                    }

                    storageItem = dbs.OrderByDescending(p => p.Version).FirstOrDefault();
                }

                if (storageItem != null)
                {
                    return storageItem.FileName;
                }
            }

            return mainPath;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class StorageItem.
        /// </summary>
        private class StorageItem
        {
            #region Properties

            /// <summary>
            /// Gets or sets the name of the file.
            /// </summary>
            /// <value>The name of the file.</value>
            public string FileName { get; init; }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            /// <value>The version.</value>
            public System.Version Version { get; init; }

            #endregion Properties
        }

        #endregion Classes
    }
}
