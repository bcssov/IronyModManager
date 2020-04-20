// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 01-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="IWindowStateService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using IronyModManager.Models.Common;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Interface IWindowStateService
    /// Implements the <see cref="IronyModManager.Services.Common.IBaseService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.IBaseService" />
    public interface IWindowStateService : IBaseService
    {
        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IWindowState.</returns>
        IWindowState Get();

        /// <summary>
        /// Determines whether this instance is defined.
        /// </summary>
        /// <returns><c>true</c> if this instance is defined; otherwise, <c>false</c>.</returns>
        bool IsDefined();

        /// <summary>
        /// Determines whether this instance is maximized.
        /// </summary>
        /// <returns><c>true</c> if this instance is maximized; otherwise, <c>false</c>.</returns>
        bool IsMaximized();

        /// <summary>
        /// Saves the specified state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Save(IWindowState state);

        #endregion Methods
    }
}
