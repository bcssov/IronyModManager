// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-10-2020
// ***********************************************************************
// <copyright file="WritingStateOperationEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.IO.Common
{
    /// <summary>
    /// Class WritingStateOperationEvent.
    /// </summary>
    public class WritingStateOperationEvent
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is writting.
        /// </summary>
        /// <value><c>true</c> if this instance is writting; otherwise, <c>false</c>.</value>
        public bool IsWritting { get; set; }

        #endregion Properties
    }
}
