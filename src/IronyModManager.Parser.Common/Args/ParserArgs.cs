// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Args
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="ParserArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Parser.Common.Args
{
    /// <summary>
    /// Class ParserArgs.
    /// </summary>
    public class ParserArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserArgs"/> class.
        /// </summary>
        public ParserArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserArgs"/> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ParserArgs(ParserArgs args)
        {
            ContentSHA = args.ContentSHA;
            File = args.File;
            Lines = args.Lines;
            ModDependencies = args.ModDependencies;
            ModName = args.ModName;
            ValidationType = args.ValidationType;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the content sha.
        /// </summary>
        /// <value>The content sha.</value>
        public string ContentSHA { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the lines.
        /// </summary>
        /// <value>The lines.</value>
        public IEnumerable<string> Lines { get; set; }

        /// <summary>
        /// Gets or sets the mod dependencies.
        /// </summary>
        /// <value>The mod dependencies.</value>
        public IEnumerable<string> ModDependencies { get; set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        public string ModName { get; set; }

        /// <summary>
        /// Gets or sets the type of the validation.
        /// </summary>
        /// <value>The type of the validation.</value>
        public ValidationType ValidationType { get; set; }

        #endregion Properties
    }
}
