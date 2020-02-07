// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 01-19-2020
//
// Last Modified By : Mario
// Last Modified On : 01-19-2020
// ***********************************************************************
// <copyright file="DoNotNotifyAttribute.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared
{
    /// <summary>
    /// Class DoNotNotifyAttribute.
    /// Implements the <see cref="System.Attribute" />
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DoNotNotifyAttribute : Attribute
    {
    }
}
