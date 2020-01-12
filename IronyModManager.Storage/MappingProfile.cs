// ***********************************************************************
// Assembly         : IronyModManager.Storage
// Author           : Mario
// Created          : 01-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-11-2020
// ***********************************************************************
// <copyright file="MappingProfile.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System;
using AutoMapper;

namespace IronyModManager.Storage
{
    /// <summary>
    /// Class MappingProfile.
    /// Implements the <see cref="AutoMapper.Profile" />
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class MappingProfile : Profile
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile" /> class.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<IDatabase, Database>().As<Database>();
            CreateMap<Database, IDatabase>().As<Database>();
        }

        #endregion Constructors
    }
}
