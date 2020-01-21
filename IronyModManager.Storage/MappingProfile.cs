// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-21-2020
// ***********************************************************************
// <copyright file="MappingProfile.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class MappingProfile.
    /// Implements the <see cref="IronyModManager.Shared.BaseMappingProfile" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.BaseMappingProfile" />
    public class MappingProfile : BaseMappingProfile
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile" /> class.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<IDatabase, Database>().ReverseMap();
            CreateMap<IPreferences, IPreferences>().ReverseMap();
        }

        #endregion Constructors
    }
}
