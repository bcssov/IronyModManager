// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 02-14-2021
// ***********************************************************************
// <copyright file="IDLC.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System;
using IronyModManager.Localization;
using IronyModManager.Shared.Models;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Interface IDLC
    /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
    /// Implements the <see cref="IronyModManager.Shared.Models.IDLCObject" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
    /// <seealso cref="IronyModManager.Shared.Models.IDLCObject" />
    public interface IDLC : ILocalizableModel, IDLCObject
    {
    }
}
