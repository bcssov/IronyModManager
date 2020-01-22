// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="LocalizationAttributeBase.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Localization.Attributes
{
    /// <summary>
    /// Class LocalizationAttributeBase.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class LocalizationAttributeBase : Attribute
    {
    }
}
