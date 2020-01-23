// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="AutoRefreshLocalizationAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Localization.Attributes
{
    /// <summary>
    /// Class ForceLocalizeAttribute.
    /// Implements the <see cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AutoRefreshLocalizationAttribute : LocalizationAttributeBase
    {
    }
}
