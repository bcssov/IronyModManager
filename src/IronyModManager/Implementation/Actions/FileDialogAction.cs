// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 09-30-2020
// ***********************************************************************
// <copyright file="FileDialogAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using IronyModManager.Shared;

namespace IronyModManager.Implementation.Actions
{
    /// <summary>
    /// Class FileDialogAction.
    /// Implements the <see cref="IronyModManager.Implementation.Actions.IFileDialogAction" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.Actions.IFileDialogAction" />
    [ExcludeFromCoverage("UI Actions are tested via functional testing.")]
    public class FileDialogAction : IFileDialogAction
    {
        #region Fields

        /// <summary>
        /// The extension separator
        /// </summary>
        private const string ExtensionSeparator = ",";

        #endregion Fields

        #region Methods

        /// <summary>
        /// open dialog as an asynchronous operation.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="initialFileName">Initial name of the file.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public async Task<string> OpenDialogAsync(string title, string initialFileName = Shared.Constants.EmptyParam, params string[] extensions)
        {
            var dialog = new OpenFileDialog
            {
                Title = title,
                Filters = GetFilters(extensions),
                Directory = GetInitialDirectory(),
                AllowMultiple = false
            };
            if (!string.IsNullOrWhiteSpace(initialFileName))
            {
                dialog.InitialFileName = initialFileName;
            }
            var result = await dialog.ShowAsync(Helpers.GetMainWindow());
            var file = result?.FirstOrDefault();
            if (File.Exists(file))
            {
                return file;
            }
            return file;
        }

        /// <summary>
        /// open folder dialog as an asynchronous operation.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public async Task<string> OpenFolderDialogAsync(string title)
        {
            var dialog = new OpenFolderDialog()
            {
                Title = title,
                Directory = GetInitialDirectory()
            };
            var result = await dialog.ShowAsync(Helpers.GetMainWindow());
            if (Directory.Exists(result))
            {
                return result;
            }
            return string.Empty;
        }

        /// <summary>
        /// save dialog as an asynchronous operation.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="initialFileName">Initial name of the file.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public async Task<string> SaveDialogAsync(string title, string initialFileName = Shared.Constants.EmptyParam, params string[] extensions)
        {
            var dialog = new SaveFileDialog
            {
                Title = title,
                Filters = GetFilters(extensions),
                Directory = GetInitialDirectory()
            };
            if (!string.IsNullOrWhiteSpace(initialFileName))
            {
                dialog.InitialFileName = initialFileName.GenerateValidFileName();
            }
            var result = await dialog.ShowAsync(Helpers.GetMainWindow());
            return result;
        }

        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <param name="extensions">The extensions.</param>
        /// <returns>List&lt;FileDialogFilter&gt;.</returns>
        private List<FileDialogFilter> GetFilters(params string[] extensions)
        {
            var filter = new List<FileDialogFilter>();
            foreach (var item in extensions)
            {
                filter.Add(new FileDialogFilter()
                {
                    Name = item,
                    Extensions = item.Split(ExtensionSeparator, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList() // magic string
                });
            }
            return filter;
        }

        /// <summary>
        /// Gets the initial directory.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetInitialDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        #endregion Methods
    }
}
