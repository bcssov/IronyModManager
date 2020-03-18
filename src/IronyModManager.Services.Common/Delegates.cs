// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 03-17-2020
//
// Last Modified By : Mario
// Last Modified On : 03-18-2020
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
    /// Delegate ModDefinitionAnalyzeDelegate
    /// </summary>
    /// <param name="percent">The percent.</param>
    public delegate void ModDefinitionAnalyzeDelegate(int percent);

    /// <summary>
    /// Delegate ModAnalyzeDelegate
    /// </summary>
    /// <param name="percent">The percent.</param>
    public delegate void ModDefinitionLoadDelegate(int percent);
}
