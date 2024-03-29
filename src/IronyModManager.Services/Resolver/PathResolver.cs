﻿// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 09-22-2020
//
// Last Modified By : Mario
// Last Modified On : 09-12-2021
// ***********************************************************************
// <copyright file="PathResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.Shared;

namespace IronyModManager.Services.Resolver
{
    /// <summary>
    /// Class PathResolver.
    /// </summary>
    internal class PathResolver
    {
        #region Fields

        /// <summary>
        /// The map
        /// </summary>
        private static readonly Dictionary<string, Environment.SpecialFolder> map = new()
        {
            { "%USER_DOCUMENTS%", Environment.SpecialFolder.MyDocuments },
            { "$LINUX_DATA_HOME", Environment.SpecialFolder.LocalApplicationData }
        };

        #endregion Fields

        #region Methods

        /// <summary>
        /// Parses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        public string Parse(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }
            var segments = path.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            var newPath = new List<string>();
            foreach (var item in segments)
            {
                if (map.ContainsKey(item.ToUpperInvariant()))
                {
                    var resolvedPath = Environment.GetFolderPath(map[item.ToUpperInvariant()]);
                    newPath.Add(resolvedPath);
                }
                else
                {
                    newPath.Add(ResolveEnvironmentVariable(item));
                }
            }
            return Path.Combine(newPath.ToArray());
        }

        /// <summary>
        /// Resolves the environment variable.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns>System.String.</returns>
        private string ResolveEnvironmentVariable(string variable)
        {
            if (variable.Contains("$") || variable.Contains("%"))
            {
                var path = Environment.ExpandEnvironmentVariables(variable);
                return path;
            }
            else
            {
                return variable;
            }
        }

        #endregion Methods
    }
}
