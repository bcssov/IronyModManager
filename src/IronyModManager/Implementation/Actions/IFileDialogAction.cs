// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 03-09-2020
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
        /// <param name="extensions">The extensions.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> OpenDialogAsync(string title, params string[] extensions);

        /// <summary>
        /// Saves the dialog asynchronous.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="extensions">The extensions.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        Task<string> SaveDialogAsync(string title, params string[] extensions);

        #endregion Methods
    }
}
