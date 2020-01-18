// ***********************************************************************
// Assembly         : IronyModManager.Models
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-15-2020
// ***********************************************************************
// <copyright file="Theme.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.ComponentModel;
using IronyModManager.Models.Common;

namespace IronyModManager.Models
{
    /// <summary>
    /// Class Theme.
    /// Implements the <see cref="IronyModManager.Models.Common.ITheme" />
    /// </summary>
    /// <seealso cref="IronyModManager.Models.Common.ITheme" />
    public class Theme : ITheme
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
#pragma warning disable 67 // False detection

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

#pragma warning restore 67

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public virtual Common.Enums.Theme Type { get; set; }

        #endregion Properties
    }
}
