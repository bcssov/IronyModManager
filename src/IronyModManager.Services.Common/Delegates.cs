// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 03-17-2020
//
// Last Modified By : Mario
// Last Modified On : 03-17-2020
// ***********************************************************************
// <copyright file="Delegates.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Delegate ModAnalyzeDelegate
    /// </summary>
    /// <param name="percent">The percent.</param>
    /// <param name="inProgress">if set to <c>true</c> [in progress].</param>
    public delegate void ModAnalyzeDelegate(int percent, bool inProgress);
}
