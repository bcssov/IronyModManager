// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 12-07-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="IExternalEditorService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IExternalEditorService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IExternalEditorService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IExternalEditor.</returns>
        IExternalEditor Get();

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <returns>IExternalEditorFiles.</returns>
        IExternalEditorFiles GetFiles();

        /// <summary>
        /// Gets the launch arguments.
        /// </summary>
        /// <param name="leftLocation">The left location.</param>
        /// <param name="rightLocation">The right location.</param>
        /// <returns>System.String.</returns>
        string GetLaunchArguments(string leftLocation, string rightLocation);

        /// <summary>
        /// Saves the specified external editor.
        /// </summary>
        /// <param name="externalEditor">The external editor.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Save(IExternalEditor externalEditor);

        #endregion Methods
    }
}
