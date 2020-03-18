// ***********************************************************************
// Assembly         : IronyModManager.Parser.Definitions
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 03-18-2020
// ***********************************************************************
// <copyright file="Definition.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Definitions
{
    /// <summary>
    /// Class Definition.
    /// Implements the <see cref="IronyModManager.Parser.Common.Definitions.IDefinition" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Definitions.IDefinition" />
    public class Definition : IDefinition
    {
        #region Fields

        /// <summary>
        /// The code
        /// </summary>
        private string code = string.Empty;

        /// <summary>
        /// The definition sha
        /// </summary>
        private string definitionSHA = string.Empty;

        /// <summary>
        /// The identifier
        /// </summary>
        private string id = string.Empty;

        /// <summary>
        /// The type
        /// </summary>
        private string type = string.Empty;

        /// <summary>
        /// The type and identifier
        /// </summary>
        private string typeAndId = string.Empty;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code
        {
            get
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return string.Empty;
                }
                return code;
            }
            set
            {
                definitionSHA = string.Empty;
                code = value;
            }
        }

        /// <summary>
        /// Gets or sets the content sha.
        /// </summary>
        /// <value>The content sha.</value>
        public string ContentSHA { get; set; }

        /// <summary>
        /// Gets the definition sha.
        /// </summary>
        /// <value>The definition sha.</value>
        public string DefinitionSHA
        {
            get
            {
                if (ValueType == Common.ValueType.Binary)
                {
                    return ContentSHA;
                }
                if (string.IsNullOrWhiteSpace(definitionSHA))
                {
                    definitionSHA = DIResolver.Get<ITextParser>().CleanWhitespace(Code).CalculateSHA();
                }
                return definitionSHA;
            }
        }

        /// <summary>
        /// Gets or sets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        public IEnumerable<string> Dependencies { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                typeAndId = string.Empty;
                id = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        public string ModName { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                typeAndId = string.Empty;
                type = value;
            }
        }

        /// <summary>
        /// Gets the type and identifier.
        /// </summary>
        /// <value>The type and identifier.</value>
        public string TypeAndId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(typeAndId))
                {
                    typeAndId = $"{Type}-{Id}";
                }
                return typeAndId;
            }
        }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        public Common.ValueType ValueType { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <param name="unwrap">if set to <c>true</c> [unwrap].</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object GetValue(string propName, bool unwrap)
        {
            return propName switch
            {
                nameof(Code) => Code,
                nameof(ContentSHA) => ContentSHA,
                nameof(Dependencies) => Dependencies,
                nameof(File) => File,
                nameof(Type) => Type,
                nameof(TypeAndId) => TypeAndId,
                nameof(ValueType) => ValueType,
                nameof(DefinitionSHA) => DefinitionSHA,
                _ => Id,
            };
        }

        #endregion Methods
    }
}
