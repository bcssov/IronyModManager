// ***********************************************************************
// Assembly         : IronyModManager.Parser.Definitions
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 01-31-2022
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
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using Newtonsoft.Json;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Definitions
{
    /// <summary>
    /// Class Definition.
    /// Implements the <see cref="IronyModManager.Shared.Models.IDefinition" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Models.IDefinition" />
    public class Definition : IDefinition
    {
        #region Fields

        /// <summary>
        /// The additional files lock
        /// </summary>
        private readonly object additionalFilesLock = new();

        /// <summary>
        /// The generated files lock
        /// </summary>
        private readonly object generatedFilesLock = new();

        /// <summary>
        /// The overwritten files lock
        /// </summary>
        private readonly object overwrittenFilesLock = new();

        /// <summary>
        /// The additional file names
        /// </summary>
        private IList<string> additionalFileNames = new List<string>();

        /// <summary>
        /// The code
        /// </summary>
        private string code = string.Empty;

        /// <summary>
        /// The code separator
        /// </summary>
        private string codeSeparator = string.Empty;

        /// <summary>
        /// The code tag
        /// </summary>
        private string codeTag = string.Empty;

        /// <summary>
        /// The definition sha
        /// </summary>
        private string definitionSHA = string.Empty;

        /// <summary>
        /// The disk file
        /// </summary>
        private string diskFile = string.Empty;

        /// <summary>
        /// The external definition sha
        /// </summary>
        private string externalDefinitionSHA = string.Empty;

        /// <summary>
        /// The file
        /// </summary>
        private string file = string.Empty;

        /// <summary>
        /// The generated file names
        /// </summary>
        private IList<string> generatedFileNames = new List<string>();

        /// <summary>
        /// The identifier
        /// </summary>
        private string id = string.Empty;

        /// <summary>
        /// The original code
        /// </summary>
        private string originalCode = string.Empty;

        /// <summary>
        /// The original file name
        /// </summary>
        private string originalFileName = string.Empty;

        /// <summary>
        /// The overwritten file names
        /// </summary>
        private IList<string> overwrittenFileNames = new List<string>();

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

        /// <summary>
        /// The virtual path
        /// </summary>
        private string virtualPath = string.Empty;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the additional file names.
        /// </summary>
        /// <value>The additional file names.</value>
        public IList<string> AdditionalFileNames
        {
            get
            {
                if (!additionalFileNames.Contains(File))
                {
                    lock (additionalFilesLock)
                    {
                        additionalFileNames.Add(File);
                    }
                }
                return additionalFileNames.Distinct().ToList();
            }
            set
            {
                IList<string> val;
                if (value == null)
                {
                    val = new List<string>();
                }
                else
                {
                    val = value;
                }
                lock (additionalFilesLock)
                {
                    additionalFileNames = val;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow duplicate].
        /// </summary>
        /// <value><c>true</c> if [allow duplicate]; otherwise, <c>false</c>.</value>
        public bool AllowDuplicate { get; set; }

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
        /// Gets the code separator.
        /// </summary>
        /// <value>The code separator.</value>
        [JsonIgnore]
        public string CodeSeparator
        {
            get
            {
                return codeSeparator ?? string.Empty;
            }
            set
            {
                codeSeparator = value;
            }
        }

        /// <summary>
        /// Gets the code tag.
        /// </summary>
        /// <value>The code tag.</value>
        [JsonIgnore]
        public string CodeTag
        {
            get
            {
                return codeTag ?? string.Empty;
            }
            set
            {
                codeTag = value;
            }
        }

        /// <summary>
        /// Gets or sets the content sha.
        /// </summary>
        /// <value>The content sha.</value>
        public string ContentSHA { get; set; }

        /// <summary>
        /// Gets or sets the custom priority order.
        /// </summary>
        /// <value>The custom priority order.</value>
        public int CustomPriorityOrder { get; set; }

        /// <summary>
        /// Gets the definition sha.
        /// </summary>
        /// <value>The definition sha.</value>
        public string DefinitionSHA
        {
            get
            {
                if (ValueType == ValueType.Binary)
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
        /// Gets or sets the disk file.
        /// </summary>
        /// <value>The disk file.</value>
        public string DiskFile
        {
            get
            {
                return diskFile;
            }
            set
            {
                var val = value ?? string.Empty;
                diskFile = val;
                DiskFileCI = val.ToLowerInvariant();
            }
        }

        /// <summary>
        /// Gets the disk file ci.
        /// </summary>
        /// <value>The disk file ci.</value>
        public string DiskFileCI { get; private set; } = string.Empty;

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
        /// Gets or sets a value indicating whether [exists in last file].
        /// </summary>
        /// <value><c>true</c> if [exists in last file]; otherwise, <c>false</c>.</value>
        public bool ExistsInLastFile { get; set; } = true;

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
                var old = file;
                var val = value ?? string.Empty;
                file = val;
                FileCI = val.ToLowerInvariant();
                parentDirectory = string.Empty;
                parentDirectoryCI = string.Empty;
                if (generatedFileNames.Contains(old))
                {
                    lock (generatedFilesLock)
                    {
                        generatedFileNames.Remove(old);
                    }
                }
                if (overwrittenFileNames.Contains(old))
                {
                    lock (overwrittenFilesLock)
                    {
                        overwrittenFileNames.Remove(old);
                    }
                }
                if (additionalFileNames.Contains(old))
                {
                    lock (additionalFilesLock)
                    {
                        additionalFileNames.Remove(old);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the file ci.
        /// </summary>
        /// <value>The file ci.</value>
        public string FileCI { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the generated file names.
        /// </summary>
        /// <value>The generated file names.</value>
        [JsonIgnore]
        public IList<string> GeneratedFileNames
        {
            get
            {
                if (!generatedFileNames.Contains(File))
                {
                    lock (generatedFilesLock)
                    {
                        generatedFileNames.Add(File);
                    }
                }
                return generatedFileNames.Distinct().ToList();
            }
            set
            {
                IList<string> val;
                if (value == null)
                {
                    val = new List<string>();
                }
                else
                {
                    val = value;
                }
                lock (generatedFilesLock)
                {
                    generatedFileNames = val;
                }
            }
        }

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
        /// Gets or sets a value indicating whether this instance is custom patch.
        /// </summary>
        /// <value><c>true</c> if this instance is custom patch; otherwise, <c>false</c>.</value>
        public bool IsCustomPatch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is from game.
        /// </summary>
        /// <value><c>true</c> if this instance is from game; otherwise, <c>false</c>.</value>
        public bool IsFromGame { get; set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        public string ModName { get; set; }

        /// <summary>
        /// Gets or sets the mod path.
        /// </summary>
        /// <value>The mod path.</value>
        [JsonIgnore]
        public string ModPath { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the original code.
        /// </summary>
        /// <value>The original code.</value>
        [JsonIgnore]
        public string OriginalCode
        {
            get
            {
                return originalCode ?? string.Empty;
            }
            set
            {
                originalCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the original file.
        /// </summary>
        /// <value>The name of the original file.</value>
        public string OriginalFileName
        {
            get
            {
                return originalFileName;
            }
            set
            {
                originalFileName = value ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the name of the original mod.
        /// </summary>
        /// <value>The name of the original mod.</value>
        public string OriginalModName { get; set; }

        /// <summary>
        /// Gets or sets the overwritten file names.
        /// </summary>
        /// <value>The overwritten file names.</value>
        public IList<string> OverwrittenFileNames
        {
            get
            {
                if (!overwrittenFileNames.Contains(File))
                {
                    lock (overwrittenFilesLock)
                    {
                        overwrittenFileNames.Add(File);
                    }
                }
                return overwrittenFileNames.Distinct().ToList();
            }
            set
            {
                IList<string> val;
                if (value == null)
                {
                    val = new List<string>();
                }
                else
                {
                    val = value;
                }
                lock (overwrittenFilesLock)
                {
                    overwrittenFileNames = val;
                }
            }
        }

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
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        [JsonIgnore]
        public IList<string> Tags { get; set; } = new List<string>();

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
        public ValueType ValueType { get; set; }

        /// <summary>
        /// Gets or sets the variables.
        /// </summary>
        /// <value>The variables.</value>
        [JsonIgnore]
        public IEnumerable<IDefinition> Variables { get; set; }

        /// <summary>
        /// Gets the virtual localization path ci.
        /// </summary>
        /// <value>The virtual localization path ci.</value>
        [JsonIgnore]
        public string VirtualParentDirectory { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the virtual parent directory ci.
        /// </summary>
        /// <value>The virtual parent directory ci.</value>
        [JsonIgnore]
        public string VirtualParentDirectoryCI { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the virtual localization path.
        /// </summary>
        /// <value>The virtual localization path.</value>
        public string VirtualPath
        {
            get
            {
                return virtualPath;
            }
            set
            {
                virtualPath = value ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(virtualPath))
                {
                    VirtualParentDirectory = Path.GetDirectoryName(virtualPath);
                    VirtualParentDirectoryCI = VirtualParentDirectory.ToLowerInvariant();
                }
                else
                {
                    VirtualParentDirectory = VirtualParentDirectoryCI = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [will be reset].
        /// </summary>
        /// <value><c>true</c> if [will be reset]; otherwise, <c>false</c>.</value>
        [JsonIgnore]
        public bool WillBeReset { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <param name="unwrap">if set to <c>true</c> [unwrap].</param>
        /// <returns>System.Object.</returns>
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
                nameof(FileCI) => FileCI,
                nameof(ParentDirectoryCI) => ParentDirectoryCI,
                nameof(GeneratedFileNames) => GeneratedFileNames,
                nameof(AdditionalFileNames) => AdditionalFileNames,
                nameof(OverwrittenFileNames) => OverwrittenFileNames,
                nameof(ModPath) => ModPath,
                nameof(CodeSeparator) => CodeSeparator,
                nameof(CodeTag) => CodeTag,
                nameof(OriginalCode) => originalCode,
                nameof(Tags) => Tags,
                nameof(Order) => Order,
                nameof(DiskFile) => DiskFile,
                nameof(OriginalModName) => OriginalModName,
                nameof(Variables) => Variables,
                nameof(DiskFileCI) => DiskFileCI,
                nameof(OriginalFileName) => OriginalFileName,
                nameof(VirtualPath) => VirtualPath,
                nameof(VirtualParentDirectory) => VirtualParentDirectory,
                nameof(VirtualParentDirectoryCI) => VirtualParentDirectoryCI,
                nameof(CustomPriorityOrder) => CustomPriorityOrder,
                nameof(IsCustomPatch) => IsCustomPatch,
                nameof(IsFromGame) => IsFromGame,
                nameof(AllowDuplicate) => AllowDuplicate,
                nameof(WillBeReset) => WillBeReset,
                _ => Id
            };
        }

        /// <summary>
        /// Determines whether the specified term is match.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns><c>true</c> if the specified term is match; otherwise, <c>false</c>.</returns>
        public bool IsMatch(string term)
        {
            string searchTerm = string.Empty;
            if (!string.IsNullOrWhiteSpace(ModName) && !string.IsNullOrWhiteSpace(Id))
            {
                searchTerm = $"{ModName} - {Id}";
            }
            else if (!string.IsNullOrWhiteSpace(ModName))
            {
                searchTerm = ModName;
            }
            else if (!string.IsNullOrWhiteSpace(Id))
            {
                searchTerm = Id;
            }
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return false;
            }
            term ??= string.Empty;
            return searchTerm.StartsWith(term, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Methods
    }
}
