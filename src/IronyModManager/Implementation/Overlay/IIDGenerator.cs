// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-19-2020
//
// Last Modified By : Mario
// Last Modified On : 11-19-2020
// ***********************************************************************
// <copyright file="IIDGenerator.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace IronyModManager.Implementation.Overlay
{
    /// <summary>
    /// Interface IIDGenerator
    /// </summary>
    public interface IIDGenerator
    {
        #region Methods

        /// <summary>
        /// Gets the next identifier.
        /// </summary>
        /// <returns>System.Int64.</returns>
        long GetNextId();

        #endregion Methods
    }
}
