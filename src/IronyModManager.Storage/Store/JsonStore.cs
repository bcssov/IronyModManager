// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 04-28-2020
// ***********************************************************************
// <copyright file="JsonStore.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
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
            string filePath = GetfilePath(id);
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
            string filePath = GetfilePath(id);
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
        /// Getfiles the path.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        private string GetfilePath(string id)
        {
            return Path.Combine(RootPath, $"{id}{Shared.Constants.JsonExtension}");
        }

        #endregion Methods
    }
}
