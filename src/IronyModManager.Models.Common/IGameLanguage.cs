// ***********************************************************************
// Assembly         :
// Author           : Mario
// Created          : 02-25-2024
//
// Last Modified By : Mario
// Last Modified On : 02-25-2024
// ***********************************************************************
// <copyright file="IGameLanguage.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// An i game language interface.
    /// </summary>
    public interface IGameLanguage : IModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the is selected.
        /// </summary>
        /// <value><c>true</c> if is selected; otherwise, <c>false</c>.</value>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets a value representing the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        #endregion Properties
    }
}
