// ***********************************************************************
// Assembly         : IronyModManager.Parser.Definitions
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 05-08-2020
// ***********************************************************************
// <copyright file="Definition.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// The external definition sha
        /// </summary>
        private string externalDefinitionSHA = string.Empty;

        /// <summary>
        /// The file
        /// </summary>
        private string file = string.Empty;

        /// <summary>
        /// The identifier
        /// </summary>
        private string id = string.Empty;

        /// <summary>
        /// The parent directory
        /// </summary>
        private string parentDirectory = string.Empty;

        /// <summary>
        /// The parent directory ci
        /// </summary>
        private string parentDirectoryCI = string.Empty;

        /// <summary>
        /// The single line code
        /// </summary>
        private string singleLineCode = string.Empty;

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
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var lines = value.ReplaceTabs().SplitOnNewLine();
                    var singleLine = new List<string>();
                    foreach (var line in lines)
                    {
                        singleLine.Add(line.Trim());
                    }
                    singleLineCode = string.Join(' ', singleLine.Where(s => !string.IsNullOrWhiteSpace(s)));
                    code = string.Join(Environment.NewLine, lines.Where(s => !string.IsNullOrWhiteSpace(s)));
                }
                else
                {
                    code = value;
                }
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
                if (!string.IsNullOrEmpty(externalDefinitionSHA))
                {
                    return externalDefinitionSHA;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(definitionSHA) && !string.IsNullOrWhiteSpace(singleLineCode))
                    {
                        definitionSHA = DIResolver.Get<ICodeParser>().CleanWhitespace(singleLineCode).CalculateSHA();
                    }
                    return definitionSHA;
                }
            }
            set
            {
                externalDefinitionSHA = value;
            }
        }

        /// <summary>
        /// Gets or sets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        public IEnumerable<string> Dependencies { get; set; }

        /// <summary>
        /// Gets or sets the error column.
        /// </summary>
        /// <value>The error column.</value>
        public long? ErrorColumn { get; set; }

        /// <summary>
        /// Gets or sets the error line.
        /// </summary>
        /// <value>The error line.</value>
        public long? ErrorLine { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public string File
        {
            get
            {
                return file;
            }
            set
            {
                var val = value ?? string.Empty;
                file = val;
                FileCI = val.ToLowerInvariant();
            }
        }

        /// <summary>
        /// Gets the file ci.
        /// </summary>
        /// <value>The file ci.</value>
        public string FileCI { get; private set; } = string.Empty;

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
        /// Gets or sets a value indicating whether this instance is first level.
        /// </summary>
        /// <value><c>true</c> if this instance is first level; otherwise, <c>false</c>.</value>
        public bool IsFirstLevel { get; set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        public string ModName { get; set; }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        /// <value>The parent directory.</value>
        public string ParentDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(parentDirectory))
                {
                    parentDirectory = Path.GetDirectoryName(File);
                }
                return parentDirectory;
            }
        }

        /// <summary>
        /// Gets the parent directory ci.
        /// </summary>
        /// <value>The parent directory ci.</value>
        public string ParentDirectoryCI
        {
            get
            {
                if (string.IsNullOrWhiteSpace(parentDirectoryCI))
                {
                    parentDirectoryCI = Path.GetDirectoryName(FileCI);
                }
                return parentDirectoryCI;
            }
        }

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
        /// Gets or sets the used parser.
        /// </summary>
        /// <value>The used parser.</value>
        public string UsedParser { get; set; }

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
                nameof(ModName) => ModName,
                nameof(ParentDirectory) => ParentDirectory,
                nameof(UsedParser) => UsedParser,
                nameof(ErrorColumn) => ErrorColumn,
                nameof(ErrorLine) => ErrorLine,
                nameof(ErrorMessage) => ErrorMessage,
                nameof(IsFirstLevel) => IsFirstLevel,
                nameof(FileCI) => FileCI,
                nameof(ParentDirectoryCI) => ParentDirectoryCI,
                _ => Id,
            };
        }

        #endregion Methods
    }
}
