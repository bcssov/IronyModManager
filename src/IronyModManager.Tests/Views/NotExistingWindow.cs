// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 02-06-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2020
// ***********************************************************************
// <copyright file="NotExistingWindow.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Common.Views;
using IronyModManager.Tests.ViewModels;

namespace IronyModManager.Tests.Views
{
    /// <summary>
    /// Class NotExistingWindow.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseWindow{IronyModManager.Tests.ViewModels.NonExistingViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseWindow{IronyModManager.Tests.ViewModels.NonExistingViewModel}" />
    public class NotExistingWindow : BaseWindow<NonExistingViewModel>
    {
    }
}
