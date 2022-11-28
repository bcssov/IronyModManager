// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-28-2022
//
// Last Modified By : Mario
// Last Modified On : 11-28-2022
// ***********************************************************************
// <copyright file="AboutDialog.axaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Shared;

namespace IronyModManager.Views
{
    /// <summary>
    /// Class AboutDialog.
    /// Implements the <see cref="Window" />
    /// </summary>
    /// <seealso cref="Window" />
    public partial class AboutDialog : Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutDialog" /> class.
        /// </summary>
        public AboutDialog()
        {
            if (string.IsNullOrWhiteSpace(Data))
            {
                var appTitle = IronyFormatter.Format(DIResolver.Get<ILocalizationManager>().GetResource(LocalizationResources.App.Title),
                new
                {
                    AppVersion = FileVersionInfo.GetVersionInfo(GetType().Assembly.Location).ProductVersion.Split("+")[0]
                });
                Data = appTitle;
            }
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public static string Data { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Opens the browser.
        /// </summary>
        /// <param name="url">The URL.</param>
        public static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // If no associated application/json MimeType is found xdg-open opens retrun error
                // but it tries to open it anyway using the console editor (nano, vim, other..)
                ShellExec($"xdg-open {url}", waitForExit: false);
            }
            else
            {
                using Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? url : "open",
                    Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? $"{url}" : "",
                    CreateNoWindow = true,
                    UseShellExecute = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                });
            }
        }

        /// <summary>
        /// Shells the execute.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="waitForExit">if set to <c>true</c> [wait for exit].</param>
        private static void ShellExec(string cmd, bool waitForExit = true)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            using var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "/bin/sh",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            );
            if (waitForExit)
            {
                process.WaitForExit();
            }
        }

        #endregion Methods
    }
}
