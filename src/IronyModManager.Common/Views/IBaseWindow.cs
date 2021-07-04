// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 07-04-2021
//
// Last Modified By : Mario
// Last Modified On : 07-04-2021
// ***********************************************************************
// <copyright file="IBaseWindow.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Common.Views
{
    /// <summary>
    /// Interface IBaseWindow
    /// </summary>
    public interface IBaseWindow
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is activated.
        /// </summary>
        /// <value><c>true</c> if this instance is activated; otherwise, <c>false</c>.</value>
        bool IsActivated { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is center screen.
        /// </summary>
        /// <value><c>true</c> if this instance is center screen; otherwise, <c>false</c>.</value>
        bool IsCenterScreen { get; }

        #endregion Properties
    }
}
