// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 08-17-2020
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
using IronyModManager.Shared;
using IronyModManager.Storage.Store;
using Jot.Storage;
using Newtonsoft.Json;

namespace IronyModManager.Storage
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
        /// The root path
        /// </summary>
        private static string _rootPath = string.Empty;

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
        private static string RootPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_rootPath))
                {
                    _rootPath = InitPath();
                }
                return _rootPath;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IDictionary&lt;System.String, System.Object&gt;.</returns>
        public IDictionary<string, object> GetData(string id)
        {
            string filePath = GetFilePath(id, true);
            List<StoreItem> storeItems = null;
            if (File.Exists(filePath))
            {
                try
                {
                    var fileContents = File.ReadAllText(filePath);
                    storeItems = JsonConvert.DeserializeObject<List<StoreItem>>(fileContents, new StoreConverter());
                }
                catch { }
            }

            if (storeItems == null)
                storeItems = new List<StoreItem>();

            return storeItems.ToDictionary(item => item.Name, item => item.Value);
        }

        /// <summary>
        /// Sets the data.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="values">The values.</param>
        public void SetData(string id, IDictionary<string, object> values)
        {
            string filePath = GetFilePath(id);
            var list = values.Select(kvp => new StoreItem() { Name = kvp.Key, Value = kvp.Value, Type = FormatTypeName(kvp.Value) });
            string serialized = JsonConvert.SerializeObject(list, new JsonSerializerSettings() { Formatting = Formatting.None, TypeNameHandling = TypeNameHandling.None });

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, serialized);
        }

        /// <summary>
        /// Initializes the path.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string InitPath()
        {
            string companyPart = string.Empty;
            string appNamePart = string.Empty;

            var entryAssembly = Assembly.GetEntryAssembly();
            var companyAttribute = (AssemblyCompanyAttribute)Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyCompanyAttribute));
            if (!string.IsNullOrEmpty(companyAttribute.Company))
                companyPart = $"{companyAttribute.Company}\\";
            var titleAttribute = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(entryAssembly, typeof(AssemblyTitleAttribute));
            if (!string.IsNullOrEmpty(titleAttribute.Title))
                appNamePart = $"{titleAttribute.Title}\\";

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $@"{companyPart}{appNamePart}");
        }

        /// <summary>
        /// Formats the name of the type.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>System.String.</returns>
        private string FormatTypeName(object instance)
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
                var names = name.Split(Store.Constants.Dot, StringSplitOptions.RemoveEmptyEntries);
                return string.Join(Store.Constants.Dot, names);
            }
            else if (typeof(IEnumerable<IPropertyChangedModel>).IsAssignableFrom(type))
            {
                var name = type.GetGenericArguments().SingleOrDefault().FullName;
                var names = name.Split(Store.Constants.Dot, StringSplitOptions.RemoveEmptyEntries);
                return $"{nameof(IEnumerable)}{Store.Constants.EnumerableOpenTag}{string.Join(Store.Constants.Dot, names)}{Store.Constants.EnumerableCloseTag}";
            }
            return type.FullName;
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
            var path = Path.Combine(RootPath, $"{id}_{version.FileMajorPart}.{version.FileMinorPart}{Shared.Constants.JsonExtension}");
            if (File.Exists(path))
            {
                return path;
            }
            if (lookForOlderVersion)
            {
                if (storageItem == null && Directory.Exists(RootPath))
                {
                    var dbs = new List<StorageItem>();
                    foreach (var item in Directory.EnumerateFiles(RootPath, $"*{Shared.Constants.JsonExtension}"))
                    {
                        if (item.Contains("_", StringComparison.OrdinalIgnoreCase))
                        {
                            var versionData = item.Split("_", StringSplitOptions.RemoveEmptyEntries)[1].Replace(Shared.Constants.JsonExtension, string.Empty).Trim();
                            if (Version.TryParse(versionData, out var parsedVersion))
                            {
                                dbs.Add(new StorageItem()
                                {
                                    FileName = item,
                                    Version = parsedVersion
                                });
                            }
                            else
                            {
                                dbs.Add(new StorageItem()
                                {
                                    FileName = item,
                                    Version = new Version(0, 0, 0, 0)
                                });
                            }
                        }
                        else
                        {
                            dbs.Add(new StorageItem()
                            {
                                FileName = item,
                                Version = new Version(0, 0, 0, 0)
                            });
                        }
                    }
                    storageItem = dbs.OrderByDescending(p => p.Version).FirstOrDefault();
                }
                if (storageItem != null)
                {
                    return storageItem.FileName;
                }
            }
            return path;
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
            public string FileName { get; set; }

            /// <summary>
            /// Gets or sets the version.
            /// </summary>
            /// <value>The version.</value>
            public Version Version { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
