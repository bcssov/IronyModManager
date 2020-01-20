// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 01-18-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="IViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Localization;

namespace IronyModManager.Common.ViewModels
{
    /// <summary>
    /// Interface IViewModel
    /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
    public interface IViewModel : ILocalizableModel
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is activated.
        /// </summary>
        /// <value><c>true</c> if this instance is activated; otherwise, <c>false</c>.</value>
        bool IsActivated { get; }

        #endregion Properties
    }
}
