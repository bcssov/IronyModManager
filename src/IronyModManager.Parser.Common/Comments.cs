// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 10-31-2021
//
// Last Modified By : Mario
// Last Modified On : 10-31-2021
// ***********************************************************************
// <copyright file="Comments.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Parser.Common
{
    /// <summary>
    /// Class Comments.
    /// </summary>
    public static class Comments
    {
        #region Methods

        /// <summary>
        /// Gets the empty type of the comment.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>System.String.</returns>
        public static string GetEmptyCommentType(string filename)
        {
            if (filename.EndsWith(Constants.ShaderExtension, StringComparison.OrdinalIgnoreCase) || filename.EndsWith(Constants.FxhExtension, StringComparison.OrdinalIgnoreCase))
            {
                return Constants.EmptyShaderComment;
            }
            return Constants.EmptyOverwriteComment;
        }

        #endregion Methods
    }
}
