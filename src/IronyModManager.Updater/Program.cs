// ***********************************************************************
// Assembly         : IronyModManager.Updater
// Author           : Mario
// Created          : 09-16-2020
//
// Last Modified By : Mario
// Last Modified On : 04-30-2021
// ***********************************************************************
// <copyright file="Program.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IronyModManager.Updater
{
    /// <summary>
    /// Class Program.
    /// </summary>
    internal class Program
    {
        #region Fields

        /// <summary>
        /// The settings path
        /// </summary>
        private static readonly string SettingsPath = ".." + Path.DirectorySeparatorChar + "update-settings.json";

        /// <summary>
        /// The update folders
        /// </summary>
        private static readonly string[] UpdateFolders = new string[] { "linux-x64", "osx-x64", "win-x64", "win-x64-setup" };

        #endregion Fields

        #region Methods

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        public static void Main()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            MainAsync().Wait();
        }

        /// <summary>
        /// Cleanups the failed updates.
        /// </summary>
        private static void CleanupFailedUpdates()
        {
            foreach (var folder in UpdateFolders)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
                if (Directory.Exists(path))
                {
                    DeleteDirectory(path, true);
                }
            }
        }

        /// <summary>
        /// Copies the update asynchronous.
        /// </summary>
        /// <param name="oldPath">The old path.</param>
        /// <param name="newPath">The new path.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        private static Task<bool> CopyUpdateAsync(string oldPath, string newPath)
        {
            if (Directory.Exists(oldPath))
            {
                var files = Directory.EnumerateFiles(oldPath, "*", SearchOption.AllDirectories);
                foreach (var item in files)
                {
                    var info = new FileInfo(item);
                    var destinationPath = Path.Combine(newPath, info.FullName.Replace(oldPath, string.Empty, StringComparison.OrdinalIgnoreCase).TrimStart(Path.DirectorySeparatorChar));
                    if (!Directory.Exists(Path.GetDirectoryName(destinationPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    }
                    info.CopyTo(destinationPath, true);
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        private static void DeleteDirectory(string directory, bool recursive)
        {
            var dirInfo = new DirectoryInfo(directory) { Attributes = FileAttributes.Normal };
            foreach (var item in dirInfo.GetFileSystemInfos("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                item.Attributes = FileAttributes.Normal;
            }
            dirInfo.Delete(recursive);
        }

        /// <summary>
        /// Ensures the permissions.
        /// </summary>
        /// <param name="path">The path.</param>
        private static void EnsurePermissions(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var sanitizedCmd = path.Replace("\"", "\\\"");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"chmod +x {sanitizedCmd}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();
            }
        }

        /// <summary>
        /// Gets the name of the main executable file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        private static string GetMainExeFileName(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(path, "IronyModManager.exe");
            }
            return Path.Combine(path, "IronyModManager");
        }

        /// <summary>
        /// Gets the main executable file name parameter.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        private static string GetMainExeFileNameParam(string path)
        {
            return $"\"{GetMainExeFileName(path)}\"";
        }

        /// <summary>
        /// Gets the name of the updater executable file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        private static string GetUpdaterExeFileName(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(path, "IronyModManager.Updater.exe");
            }
            return Path.Combine(path, "IronyModManager.Updater");
        }

        /// <summary>
        /// Gets the updater executable file name parameter.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        private static string GetUpdaterExeFileNameParam(string path)
        {
            return $"\"{GetUpdaterExeFileName(path)}\"";
        }

        /// <summary>
        /// main as an asynchronous operation.
        /// </summary>
        private static async Task MainAsync()
        {
            Console.WriteLine("Waiting for Irony process to exit...");
            await Task.Delay(5000);

            Console.WriteLine("Reading settings...");
            var text = await File.ReadAllTextAsync(SettingsPath);
            var settings = JsonConvert.DeserializeObject<UpdateSettings>(text);
            if (settings.Updated)
            {
                Console.WriteLine("Already updated closing...");
                return;
            }
            Console.WriteLine("Cleaning updates...");
            CleanupFailedUpdates();
            Console.WriteLine("Updates cleaned...");
            Console.WriteLine("Copying updates...");
            try
            {
                await CopyUpdateAsync(AppDomain.CurrentDomain.BaseDirectory, settings.Path);
            }
            catch (UnauthorizedAccessException)
            {
                // If windows probably UAC is blocking, attempt to elevate the process
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var updaterFileName = GetUpdaterExeFileName(AppDomain.CurrentDomain.BaseDirectory);
                    var procInfo = new ProcessStartInfo()
                    {
                        UseShellExecute = true,
                        Verb = "runas",
                        FileName = updaterFileName
                    };
                    Process.Start(procInfo);
                }
                Environment.Exit(1);
            }
            Console.WriteLine("Updates copied...");

            Console.WriteLine("Settings permissions...");
            EnsurePermissions(GetMainExeFileNameParam(settings.Path));
            EnsurePermissions(GetUpdaterExeFileNameParam(settings.Path));
            Console.WriteLine("Permissions set...");

            settings.Updated = true;
            text = JsonConvert.SerializeObject(settings);
            await File.WriteAllTextAsync(SettingsPath, text);

            Console.WriteLine("Relaunching Irony...");
            Process.Start(GetMainExeFileName(settings.Path));
            Console.WriteLine("Closing updater...");
            Environment.Exit(0);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class UpdateSettings.
        /// </summary>
        private class UpdateSettings
        {
            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether this instance is installer.
            /// </summary>
            /// <value><c>true</c> if this instance is installer; otherwise, <c>false</c>.</value>
            public bool IsInstaller { get; set; }

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            public string Path { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="UpdateSettings" /> is updated.
            /// </summary>
            /// <value><c>true</c> if updated; otherwise, <c>false</c>.</value>
            public bool Updated { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}
