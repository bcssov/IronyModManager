// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 01-20-2020
// ***********************************************************************
// <copyright file="JsonStore.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
                catch
                {
                }
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
            var list = values.Select(kvp => new StoreItem() { Name = kvp.Key, Value = kvp.Value, Type = FormatTypeName(kvp.Value.GetType()) });
            string serialized = JsonConvert.SerializeObject(list, new JsonSerializerSettings() { Formatting = Formatting.Indented, TypeNameHandling = TypeNameHandling.None });

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

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
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        private string FormatTypeName(Type type)
        {
            var name = type.Name.Replace("Proxy", string.Empty);
            if (!name.StartsWith("I"))
            {
                return $"I{name}";
            }
            return name;
        }

        /// <summary>
        /// Getfiles the path.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
        private string GetfilePath(string id)
        {
            return Path.Combine(RootPath, $"{id}{Constants.JsonExtension}");
        }

        #endregion Methods
    }
}
