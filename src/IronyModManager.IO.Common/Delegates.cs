// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-19-2020
//
// Last Modified By : Mario
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="Delegates.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.IO.Common
{
    /// <summary>
    /// Class Delegates.
    /// </summary>
    public class Delegates
    {
        #region Delegates

        /// <summary>
        /// Delegate WriteOperationStateDelegate
        /// </summary>
        /// <param name="started">if set to <c>true</c> [started].</param>
        public delegate void WriteOperationStateDelegate(bool started);

        #endregion Delegates
    }
}
