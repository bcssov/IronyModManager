// ***********************************************************************
// Assembly         : IronyModManager.GameLauncher
// Author           : Mario
// Created          : 10-26-2022
//
// Last Modified By : Mario
// Last Modified On : 10-26-2022
// ***********************************************************************
// <copyright file="Program.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Threading.Tasks;
using CommandLine;
using IronyModManager.IO.Platforms;

namespace IronyModManager.GameLauncher
{
    /// <summary>
    /// Class Program.
    /// </summary>
    internal class Program
    {
        #region Fields

        /// <summary>
        /// The command line arguments
        /// </summary>
        private static CommandLineArgs commandLineArgs;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ParseArguments(args);

            MainAsync().Wait();
        }

        /// <summary>
        /// Main as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static async Task MainAsync()
        {
            if (!commandLineArgs.SteamAppId.HasValue)
            {
                return;
            }
            var logger = new Logger();
            var handler = new SteamHandler(logger);
            if (commandLineArgs.UseAlternateLaunchMethod)
            {
                await handler.InitAlternateAsync();
            }
            else
            {
                await handler.InitAsync(commandLineArgs.SteamAppId.GetValueOrDefault());
            }
        }

        /// <summary>
        /// Parses the arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void ParseArguments(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineArgs>(args).WithParsed(a =>
            {
                commandLineArgs = a;
            });
        }

        #endregion Methods
    }
}
