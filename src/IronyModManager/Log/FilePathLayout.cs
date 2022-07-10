// ***********************************************************************
// Assembly         : IronyModManager
// Author           : NLog
// Created          : 10-29-2021
//
// Last Modified By : Mario
// Last Modified On : 07-10-2022
// ***********************************************************************
// <copyright file="FilePathLayout.cs" company="NLog">
//     NLog
// </copyright>
// <summary>Copied only because it's internal in NLog and we need it to get the resolved directory where logs are dumped.</summary>
// ***********************************************************************

//
// Copyright (c) 2004-2021 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
//
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
// * Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of Jaroslaw Kowalski nor the names of its
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NLog.Layouts;
using NLog.Targets;

namespace IronyModManager.Log
{
    /// <summary>
    /// Class FilePathLayout.
    /// </summary>
    public class FilePathLayout
    {
        #region Fields

        /// <summary>
        /// Cached directory separator char array to avoid memory allocation on each method call.
        /// </summary>
        private static readonly char[] DirectorySeparatorChars = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        /// <summary>
        /// Cached invalid file names char array to avoid memory allocation every time Path.GetInvalidFileNameChars() is called.
        /// </summary>
        private static readonly HashSet<char> InvalidFileNameChars = new(Path.GetInvalidFileNameChars());

        /// <summary>
        /// not null when <see cref="filePathKind" /> == <c>false</c>
        /// </summary>
        private readonly string baseDir;

        /// <summary>
        /// non null is fixed,
        /// </summary>
        private readonly string cleanedFixedResult;

        /// <summary>
        /// The cleanup invalid chars
        /// </summary>
        private readonly bool cleanupInvalidChars;

        /// <summary>
        /// The file path kind
        /// </summary>
        private readonly FilePathKind filePathKind;

        /// <summary>
        /// The layout
        /// </summary>
        private readonly Layout layout;

        /// <summary>
        /// <see cref="cachedPrevCleanFileName" /> is the cache-value that is reused, when the newly rendered filename
        /// matches the cache-key <see cref="cachedPrevRawFileName" />
        /// </summary>
        private string cachedPrevCleanFileName;

        /// <summary>
        /// <see cref="cachedPrevRawFileName" /> is the cache-key, and when newly rendered filename matches the cache-key,
        /// then it reuses the cleaned cache-value <see cref="cachedPrevCleanFileName" />.
        /// </summary>
        private string cachedPrevRawFileName;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="cleanupInvalidChars">if set to <c>true</c> [cleanup invalid chars].</param>
        /// <param name="filePathKind">Kind of the file path.</param>
        public FilePathLayout(Layout layout, bool cleanupInvalidChars, FilePathKind filePathKind)
        {
            this.layout = layout;
            this.filePathKind = filePathKind;
            this.cleanupInvalidChars = cleanupInvalidChars;

            if (this.layout is null)
            {
                this.filePathKind = FilePathKind.Unknown;
                return;
            }

            //do we have to the layout?
            if (cleanupInvalidChars || this.filePathKind == FilePathKind.Unknown)
            {
                cleanedFixedResult = CreateCleanedFixedResult(cleanupInvalidChars, layout);
                this.filePathKind = DetectKind(layout, this.filePathKind);
            }

            if (this.filePathKind == FilePathKind.Relative)
            {
                baseDir = AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Renders the specified log event.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>System.String.</returns>
        public string Render(LogEventInfo logEvent)
        {
            var rawFileName = GetRenderedFileName(logEvent);
            if (string.IsNullOrEmpty(rawFileName))
            {
                return rawFileName;
            }

            if ((!cleanupInvalidChars || cleanedFixedResult != null) && filePathKind == FilePathKind.Absolute)
                return rawFileName; // Skip clean filename string-allocation

            if (string.Equals(cachedPrevRawFileName, rawFileName, StringComparison.Ordinal) && cachedPrevCleanFileName != null)
                return cachedPrevCleanFileName;    // Cache Hit, reuse clean filename string-allocation

            var cleanFileName = GetCleanFileName(rawFileName);
            cachedPrevCleanFileName = cleanFileName;
            cachedPrevRawFileName = rawFileName;
            return cleanFileName;
        }

        /// <summary>
        /// Cleanups the invalid file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.String.</returns>
        private static string CleanupInvalidFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return filePath;
            }

            var lastDirSeparator = filePath.LastIndexOfAny(DirectorySeparatorChars);

            char[] fileNameChars = null;

            for (int i = lastDirSeparator + 1; i < filePath.Length; i++)
            {
                if (InvalidFileNameChars.Contains(filePath[i]))
                {
                    //delay char[] creation until first invalid char
                    //is found to avoid memory allocation.
                    if (fileNameChars is null)
                    {
                        fileNameChars = filePath[(lastDirSeparator + 1)..].ToCharArray();
                    }
                    fileNameChars[i - (lastDirSeparator + 1)] = '_';
                }
            }

            //only if an invalid char was replaced do we create a new string.
            if (fileNameChars != null)
            {
                //keep the / in the dirname, because dirname could be c:/ and combine of c: and file name won't work well.
                var dirName = lastDirSeparator > 0 ? filePath.Substring(0, lastDirSeparator + 1) : string.Empty;
                string fileName = new string(fileNameChars);
                return Path.Combine(dirName, fileName);
            }

            return filePath;
        }

        /// <summary>
        /// Creates the cleaned fixed result.
        /// </summary>
        /// <param name="cleanupInvalidChars">if set to <c>true</c> [cleanup invalid chars].</param>
        /// <param name="layout">The layout.</param>
        /// <returns>System.String.</returns>
        private static string CreateCleanedFixedResult(bool cleanupInvalidChars, Layout layout)
        {
            if (layout is SimpleLayout simpleLayout)
            {
                var isFixedText = simpleLayout.IsFixedText;
                if (isFixedText)
                {
                    var cleanedFixedResult = simpleLayout.FixedText;
                    if (cleanupInvalidChars)
                    {
                        //clean first
                        cleanedFixedResult = CleanupInvalidFilePath(cleanedFixedResult);
                    }

                    return cleanedFixedResult;
                }
            }

            return null;
        }

        /// <summary>
        /// Detects the kind of the file path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="isFixedText">if set to <c>true</c> [is fixed text].</param>
        /// <returns>FilePathKind.</returns>
        private static FilePathKind DetectFilePathKind(string path, bool isFixedText = true)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = path.TrimStart();

                int length = path.Length;
                if (length >= 1)
                {
                    var firstChar = path[0];
                    if (IsAbsoluteStartChar(firstChar))
                        return FilePathKind.Absolute;

                    if (firstChar == '.') //. and ..
                    {
                        return FilePathKind.Relative;
                    }

                    if (length >= 2)
                    {
                        var secondChar = path[1];
                        //on unix VolumeSeparatorChar == DirectorySeparatorChar
                        if (Path.VolumeSeparatorChar != Path.DirectorySeparatorChar && secondChar == Path.VolumeSeparatorChar)
                            return FilePathKind.Absolute;
                    }

                    if (IsLayoutRenderer(path, isFixedText))
                    {
                        //if first part is a layout, then unknown
                        return FilePathKind.Unknown;
                    }

                    //not a layout renderer, but text
                    return FilePathKind.Relative;
                }
            }
            return FilePathKind.Unknown;
        }

        /// <summary>
        /// Is this (templated/invalid) path an absolute, relative or unknown?
        /// </summary>
        /// <param name="pathLayout">The path layout.</param>
        /// <returns>FilePathKind.</returns>
        private static FilePathKind DetectFilePathKind(SimpleLayout pathLayout)
        {
            var isFixedText = pathLayout.IsFixedText;

            //nb: ${basedir} has already been rewritten in the SimpleLayout.compile
            var path = isFixedText ? pathLayout.FixedText : pathLayout.Text;
            return DetectFilePathKind(path, isFixedText);
        }

        /// <summary>
        /// Detects the kind.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <param name="currentFilePathKind">Kind of the current file path.</param>
        /// <returns>FilePathKind.</returns>
        private static FilePathKind DetectKind(Layout layout, FilePathKind currentFilePathKind)
        {
            if (layout is SimpleLayout simpleLayout)
            {
                //detect absolute
                if (currentFilePathKind == FilePathKind.Unknown)
                {
                    return DetectFilePathKind(simpleLayout);
                }
            }
            else
            {
                return FilePathKind.Unknown;
            }

            return currentFilePathKind;
        }

        /// <summary>
        /// Determines whether [is absolute start character] [the specified first character].
        /// </summary>
        /// <param name="firstChar">The first character.</param>
        /// <returns><c>true</c> if [is absolute start character] [the specified first character]; otherwise, <c>false</c>.</returns>
        private static bool IsAbsoluteStartChar(char firstChar)
        {
            return firstChar == Path.DirectorySeparatorChar || firstChar == Path.AltDirectorySeparatorChar;
        }

        /// <summary>
        /// Determines whether [is layout renderer] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="isFixedText">if set to <c>true</c> [is fixed text].</param>
        /// <returns><c>true</c> if [is layout renderer] [the specified path]; otherwise, <c>false</c>.</returns>
        private static bool IsLayoutRenderer(string path, bool isFixedText)
        {
            return !isFixedText && path.StartsWith("${", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Convert the raw filename to a correct filename
        /// </summary>
        /// <param name="rawFileName">The filename generated by Layout.</param>
        /// <returns>String representation of a correct filename.</returns>
        private string GetCleanFileName(string rawFileName)
        {
            var cleanFileName = rawFileName;
            if (cleanupInvalidChars && cleanedFixedResult is null)
            {
                cleanFileName = CleanupInvalidFilePath(rawFileName);
            }

            if (filePathKind == FilePathKind.Absolute)
            {
                return cleanFileName;
            }

            if (filePathKind == FilePathKind.Relative && baseDir != null)
            {
                //use basedir, faster than Path.GetFullPath
                cleanFileName = Path.Combine(baseDir, cleanFileName);
                return cleanFileName;
            }
            //unknown, use slow method
            cleanFileName = Path.GetFullPath(cleanFileName);
            return cleanFileName;
        }

        /// <summary>
        /// Render the raw filename from Layout
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <returns>String representation of a layout.</returns>
        private string GetRenderedFileName(LogEventInfo logEvent)
        {
            if (cleanedFixedResult != null)
            {
                return cleanedFixedResult;
            }

            if (layout == null)
            {
                return null;
            }

            return layout.Render(logEvent);
        }

        #endregion Methods
    }
}
