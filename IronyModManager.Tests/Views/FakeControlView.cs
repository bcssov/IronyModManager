// ***********************************************************************
// Assembly         :
// Author           : Mario
// Created          : 02-06-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2020
// ***********************************************************************
// <copyright file="FakeControlView.cs" company="Mario">
//     Copyright (c) Mario. All rights reserved.
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
    /// Class FakeView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.Tests.ViewModels.FakeControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.Tests.ViewModels.FakeControlViewModel}" />
    public class FakeControlView : BaseControl<FakeControlViewModel>
    {
    }
}
