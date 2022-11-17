// ***********************************************************************
// Assembly         : IronyModManager.GameHandler
// Author           : Mario
// Created          : 10-26-2022
//
// Last Modified By : Mario
// Last Modified On : 11-17-2022
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

namespace IronyModManager.GameHandler
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

            Console.WriteLine("Preparing to launch game.");
            MainAsync().Wait();
        }

        /// <summary>
        /// Main as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static async Task MainAsync()
        {
            try
            {
                if (!commandLineArgs.SteamAppId.HasValue)
                {
                    Console.WriteLine("Invalid parameters sent, no app id quitting.");
                    return;
                }
                Console.WriteLine("Preparing to check whether steam is running, this might take some time.");
                var logger = new Logger();
                var handler = new SteamHandler(logger);
                if (commandLineArgs.UseAlternateLaunchMethod)
                {
                    Console.WriteLine("Using alternate launch method to see whether steam is running. You can just turn off launcher usage in appSettings.json.");
                    await handler.InitAlternateAsync();
                }
                else
                {
                    Console.WriteLine("Checking whether steam is running using direct steam integration method.");
                    await handler.InitAsync(commandLineArgs.SteamAppId.GetValueOrDefault());
                }
                Console.WriteLine("Done exiting.");
                Environment.Exit(0);
            }
            catch
            {
                Console.WriteLine("Exit on error.");
                Environment.Exit(1);
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
