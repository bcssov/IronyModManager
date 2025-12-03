// ***********************************************************************
// Assembly         : IronyModManager.Models.Common
// Author           : Mario
// Created          : 03-01-2020
//
// Last Modified By : Mario
// Last Modified On : 12-03-2025
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Models.Common
{
    /// <summary>
    /// Enum AchievementStatus
    /// </summary>
    public enum AchievementStatus
    {
        /// <summary>
        /// The not evaluated
        /// </summary>
        NotEvaluated,

        /// <summary>
        /// The compatible
        /// </summary>
        Compatible,

        /// <summary>
        /// The not compatible
        /// </summary>
        NotCompatible,

        /// <summary>
        /// The attempted evaluation
        /// </summary>
        AttemptedEvaluation
    }

    /// <summary>
    /// Enum DefinitionPriorityType
    /// </summary>
    public enum DefinitionPriorityType
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The mod order
        /// </summary>
        ModOrder,

        /// <summary>
        /// The fios
        /// </summary>
        FIOS,

        /// <summary>
        /// The lios
        /// </summary>
        LIOS,

        /// <summary>
        /// The mod override
        /// </summary>
        ModOverride,

        /// <summary>
        /// The no provider
        /// </summary>
        NoProvider
    }

    /// <summary>
    /// Enum GameAdvancedFeatures
    /// </summary>
    public enum GameAdvancedFeatures
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The read only
        /// </summary>
        ReadOnly,

        /// <summary>
        /// The full
        /// </summary>
        Full
    }

    /// <summary>
    /// Enum HashReportType
    /// </summary>
    public enum HashReportType
    {
        /// <summary>
        /// The collection
        /// </summary>
        Collection,

        /// <summary>
        /// The game
        /// </summary>
        Game
    }

    /// <summary>
    /// Enum ModDescriptorType
    /// </summary>
    public enum ModDescriptorType
    {
        /// <summary>
        /// The descriptor mod
        /// </summary>
        DescriptorMod,

        /// <summary>
        /// The json metadata
        /// </summary>
        JsonMetadata,

        /// <summary>
        /// The json metadata v2
        /// </summary>
        JsonMetadataV2
    }

    /// <summary>
    /// Enum ModSource
    /// </summary>
    public enum ModSource
    {
        /// <summary>
        /// The local
        /// </summary>
        Local,

        /// <summary>
        /// The steam
        /// </summary>
        Steam,

        /// <summary>
        /// The paradox
        /// </summary>
        Paradox
    }

    /// <summary>
    /// Enum NotificationPosition
    /// </summary>
    public enum NotificationPosition
    {
        /// <summary>
        /// The bottom right
        /// </summary>
        BottomRight,

        /// <summary>
        /// The bottom left
        /// </summary>
        BottomLeft,

        /// <summary>
        /// The top right
        /// </summary>
        TopRight,

        /// <summary>
        /// The top left
        /// </summary>
        TopLeft
    }

    /// <summary>
    /// Enum PatchStateMode
    /// </summary>
    public enum PatchStateMode
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The default
        /// </summary>
        Default,

        /// <summary>
        /// The advanced
        /// </summary>
        Advanced,

        /// <summary>
        /// The read only
        /// </summary>
        ReadOnly,

        /// <summary>
        /// The default without localization
        /// </summary>
        DefaultWithoutLocalization,

        /// <summary>
        /// The advanced without localization
        /// </summary>
        AdvancedWithoutLocalization,

        /// <summary>
        /// The read only without localization
        /// </summary>
        ReadOnlyWithoutLocalization
    }

    /// <summary>
    /// Enum SupportedMergeTypes
    /// </summary>
    [Flags]
    public enum SupportedMergeTypes
    {
        /// <summary>
        /// The basic
        /// </summary>
        Basic = 1,

        /// <summary>
        /// The zip
        /// </summary>
        Zip = 2
    }

    /// <summary>
    /// Enum SupportedOperatingSystems
    /// </summary>
    [Flags]
    public enum SupportedOperatingSystems
    {
        /// <summary>
        /// The windows
        /// </summary>
        Windows,

        /// <summary>
        /// The osx
        /// </summary>
        OSX,

        /// <summary>
        /// The linux
        /// </summary>
        Linux
    }
}
