// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 01-20-2020
//
// Last Modified By : Mario
// Last Modified On : 03-04-2020
// ***********************************************************************
// <copyright file="BaseModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Shared;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Class BaseModel.
    /// Implements the <see cref="IronyModManager.Shared.PropertyChangedModelBase" />
    /// Implements the <see cref="IronyModManager.Models.Common.IModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.PropertyChangedModelBase" />
    /// <seealso cref="IronyModManager.Models.Common.IModel" />
    public class BaseModel : PropertyChangedModelBase, IModel
    {
    }
}
