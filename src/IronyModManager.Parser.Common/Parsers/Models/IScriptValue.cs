// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-24-2020
// ***********************************************************************
// <copyright file="IScriptValue.cs" company="Mario">
//     Mario
// </copyright>
// <summary>Based on CWTools samples.</summary>
// ***********************************************************************
namespace IronyModManager.Parser.Common.Parsers.Models
{
    /// <summary>
    /// Interface IScriptValue
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.Models.IScriptBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.Models.IScriptBase" />
    public interface IScriptValue : IScriptBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        string Value { get; set; }

        #endregion Properties
    }
}
