// ***********************************************************************
// Assembly         : IronyModManager.Storage.Common
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 02-12-2020
// ***********************************************************************
// <copyright file="IGameType.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Storage.Common
{
    /// <summary>
    /// Class IGameType.
    /// </summary>
    public interface IGameType
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion Properties
    }
}
