// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-29-2021
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="MappingProfile.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.IO.Common.Models;
using IronyModManager.IO.Models;
using IronyModManager.Models.Common;
using IronyModManager.Shared;

namespace IronyModManager.IO
{
    /// <summary>
    /// Class MappingProfile.
    /// Implements the <see cref="IronyModManager.Shared.BaseMappingProfile" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.BaseMappingProfile" />
    [ExcludeFromCoverage("Mapping profile shouldn't be tested.")]
    public class MappingProfile : BaseMappingProfile
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile" /> class.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<ICollectionImportResult, CollectionImportResult>().ReverseMap();
            CreateMap<IModCollection, ICollectionImportResult>()
                .ForMember(m => m.Game, o => o.MapFrom(s => s.Game))
                .ForMember(m => m.IsSelected, o => o.MapFrom(s => s.IsSelected))
                .ForMember(m => m.MergedFolderName, o => o.MapFrom(s => s.MergedFolderName))
                .ForMember(m => m.Name, o => o.MapFrom(s => s.Name))
                .ForMember(m => m.PatchModEnabled, o => o.MapFrom(s => s.PatchModEnabled))
                .ForMember(m => m.PatchModEnabled, o => o.MapFrom(s => s.PatchModEnabled))
                .ReverseMap()
                .IgnoreAllUnmappedMembers();
        }

        #endregion Constructors
    }
}
