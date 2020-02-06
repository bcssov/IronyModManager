// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 02-06-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2020
// ***********************************************************************
// <copyright file="FakeWindow.cs" company="Mario">
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
    /// Class FakeWindowView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseWindow{IronyModManager.Tests.ViewModels.FakeWindowViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseWindow{IronyModManager.Tests.ViewModels.FakeWindowViewModel}" />
    public class FakeWindow : BaseWindow<FakeWindowViewModel>
    {
    }
}
