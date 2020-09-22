// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 09-22-2020
// ***********************************************************************
// <copyright file="IFileDialogAction.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace IronyModManager.Implementation.Actions
{
    /// <summary>
    /// Interface IFileDialogAction
    /// </summary>
    public interface IFileDialogAction
    {
        #region Methods

        /// <summary>
        /// Opens the dialog asynchronous.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="initialFileName">Initial name of the file.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> OpenDialogAsync(string title, string initialFileName = Shared.Constants.EmptyParam, params string[] extensions);

        /// <summary>
        /// Opens the folder dialog asynchronous.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> OpenFolderDialogAsync(string title);

        /// <summary>
        /// Saves the dialog asynchronous.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="initialFileName">Initial name of the file.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> SaveDialogAsync(string title, string initialFileName = Shared.Constants.EmptyParam, params string[] extensions);

        #endregion Methods
    }
}
