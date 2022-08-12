// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-12-2022
//
// Last Modified By : Mario
// Last Modified On : 08-12-2022
// ***********************************************************************
// <copyright file="DefinitionBsonMapper.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LiteDB;

namespace IronyModManager.IO.Game
{
    /// <summary>
    /// Class DefinitionBsonMapper.
    /// Implements the <see cref="BsonMapper" />
    /// </summary>
    /// <seealso cref="BsonMapper" />
    internal class DefinitionBsonMapper : BsonMapper
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionBsonMapper" /> class.
        /// </summary>
        public DefinitionBsonMapper() : base(typeNameBinder: new DefinitionTypeBinder())
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets MemberInfo that refers to Id from a document object.
        /// </summary>
        /// <param name="members">The members.</param>
        /// <returns>MemberInfo.</returns>
        protected override MemberInfo GetIdMember(IEnumerable<MemberInfo> members)
        {
            // Let litedb generate auto id itself don't use our field by name and we don't want to use attributes
            return null;
        }

        #endregion Methods
    }
}
