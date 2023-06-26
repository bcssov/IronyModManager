
// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 06-25-2023
//
// Last Modified By : Mario
// Last Modified On : 06-25-2023
// ***********************************************************************
// <copyright file="DefinitionSearchBsonMapper.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LiteDB;

namespace IronyModManager.Parser.Definitions
{

    /// <summary>
    /// Class DefinitionSearchBsonMapper.
    /// Implements the <see cref="BsonMapper" />
    /// </summary>
    /// <seealso cref="BsonMapper" />
    internal class DefinitionSearchBsonMapper : BsonMapper
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionSearchBsonMapper"/> class.
        /// </summary>
        public DefinitionSearchBsonMapper() : base(typeNameBinder: new DefinitionSearchTypeBinder())
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the identifier member.
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
