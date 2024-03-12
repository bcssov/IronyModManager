// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-10-2024
//
// Last Modified By : Mario
// Last Modified On : 03-11-2024
// ***********************************************************************
// <copyright file="SingleInstance.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace IronyModManager.Implementation.SingleInstance
{
    /// <summary>
    /// Class SingleInstance.
    /// </summary>
    internal static class SingleInstance
    {
        #region Fields

        /// <summary>
        /// The delay
        /// </summary>
        private const int Delay = 100;

        /// <summary>
        /// The object lock
        /// </summary>
        private static readonly object objLock = new();

        /// <summary>
        /// The mutex
        /// </summary>
        private static Mutex mutex;

        /// <summary>
        /// The mutex name
        /// </summary>
        private static string mutexName;

        /// <summary>
        /// The thread
        /// </summary>
        private static Thread thread;

        #endregion Fields

        #region Delegates

        /// <summary>
        /// Delegate ArgsDelegate
        /// </summary>
        /// <param name="args">The arguments.</param>
        public delegate void ArgsDelegate(Args args);

        #endregion Delegates

        #region Events

        /// <summary>
        /// Occurs when [instance launched].
        /// </summary>
        public static event ArgsDelegate InstanceLaunched;

        #endregion Events

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Initialize()
        {
            lock (objLock)
            {
                var initial = false;
                try
                {
                    mutex = new Mutex(true, $"Global\\{GetMutexName()}", out initial);
                    if (initial)
                    {
                        thread = new Thread(Monitor) { Name = typeof(SingleInstance).FullName, IsBackground = true };
                        thread.Start();
                    }
                    else
                    {
                        var data = new Args { CommandLineArgs = Environment.GetCommandLineArgs() };
                        var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
                        using var pipe = new NamedPipeClientStream(".", GetMutexName(), PipeDirection.Out, PipeOptions.CurrentUserOnly | PipeOptions.WriteThrough);
                        pipe.Connect();
                        pipe.Write(bytes, 0, bytes.Length);
                    }
                }
                catch
                {
                }

                if (!initial)
                {
                    Environment.Exit(0);
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Monitors this instance.
        /// </summary>
        public static void Monitor()
        {
            using var pipe = new NamedPipeServerStream(GetMutexName(), PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.CurrentUserOnly | PipeOptions.WriteThrough);
            while (mutex != null)
            {
                try
                {
                    if (!pipe.IsConnected)
                    {
                        pipe.WaitForConnection();
                    }

                    var buffer = new byte[1024];
                    var sb = new StringBuilder();
                    while (true)
                    {
                        var readBytes = pipe.Read(buffer, 0, buffer.Length);
                        if (readBytes == 0)
                        {
                            break;
                        }

                        sb.Append(Encoding.UTF8.GetString(buffer));
                    }

                    var args = JsonConvert.DeserializeObject<Args>(sb.ToString());
                    pipe.Disconnect();
                    if (args != null)
                    {
                        InstanceLaunched?.Invoke(args);
                    }
                }
                catch
                {
                    Thread.Sleep(Delay);
                }
            }
        }

        /// <summary>
        /// Gets the name of the mutex.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetMutexName()
        {
            lock (objLock)
            {
                if (string.IsNullOrWhiteSpace(mutexName))
                {
                    var name = nameof(IronyModManager);
                    var sb = new StringBuilder();
                    sb.Append(name, 0, Math.Min(name.Length, 31));
                    sb.Append('.');
                    var hash = new StringBuilder();
                    hash.AppendLine(Environment.MachineName);
                    hash.AppendLine(Environment.UserName);
                    var data = SHA256.HashData(Encoding.UTF8.GetBytes(hash.ToString()));
                    foreach (var item in data)
                    {
                        if (sb.Length >= 64)
                        {
                            break;
                        }

                        sb.Append($"{item:X2}");
                    }

                    mutexName = sb.ToString();
                }

                return mutexName;
            }
        }

        #endregion Methods
    }
}
