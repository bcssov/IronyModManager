// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 05-12-2023
//
// Last Modified By : Mario
// Last Modified On : 05-12-2023
// ***********************************************************************
// <copyright file="WindowsMountedVolumeInfoListener.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Literally copied to fix a CTD as described here: https:github.com/AvaloniaUI/Avalonia/issues/8437.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls.Platform;
using Avalonia.Logging;

namespace IronyModManager.Platform.Drives
{
    /// <summary>
    /// Class WindowsMountedVolumeInfoListener.
    /// Implements the <see cref="IDisposable" />
    /// </summary>
    /// <seealso cref="IDisposable" />
    internal class WindowsMountedVolumeInfoListener : IDisposable
    {
        #region Fields

        /// <summary>
        /// The disposables
        /// </summary>
        private readonly CompositeDisposable disposables;

        /// <summary>
        /// The been disposed
        /// </summary>
        private bool beenDisposed = false;

        /// <summary>
        /// The mounted drives
        /// </summary>
        private readonly ObservableCollection<MountedVolumeInfo> mountedDrives;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsMountedVolumeInfoListener" /> class.
        /// </summary>
        /// <param name="mountedDrives">The mounted drives.</param>
        public WindowsMountedVolumeInfoListener(ObservableCollection<MountedVolumeInfo> mountedDrives)
        {
            this.mountedDrives = mountedDrives;
            disposables = new CompositeDisposable();

            var pollTimer = Observable.Interval(TimeSpan.FromSeconds(1))
                                      .Subscribe(Poll);

            disposables.Add(pollTimer);

            Poll(0);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!beenDisposed)
            {
                if (disposing)
                {
                }
                beenDisposed = true;
            }
        }

        /// <summary>
        /// Polls the specified .
        /// </summary>
        /// <param name="_">The .</param>
        private void Poll(long _)
        {
            var allDrives = DriveInfo.GetDrives();

            var mountVolInfos = allDrives
                                .Where(p =>
                                {
                                    try
                                    {
                                        var ret = p.IsReady;
                                        return ret;
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(this, $"Error in Windows drive enumeration: {e.Message}");
                                    }
                                    return false;
                                })
                                .Select(ParseDrive)
                                .Where(p => p.IsValid)
                                .ToArray();

            if (mountedDrives.SequenceEqual(mountVolInfos))
                return;
            else
            {
                mountedDrives.Clear();

                foreach (var i in mountVolInfos)
                    mountedDrives.Add(i);
            }
        }

        /// <summary>
        /// Parses the drive.
        /// </summary>
        /// <param name="drive">The drive.</param>
        /// <returns>IronyModManager.Platform.Drives.SafeMountedVolumeInfo.</returns>
        private SafeMountedVolumeInfo ParseDrive(DriveInfo drive)
        {
            try
            {
                return new SafeMountedVolumeInfo()
                {
                    VolumeLabel = string.IsNullOrEmpty(drive.VolumeLabel.Trim()) ? drive.RootDirectory.FullName
                                                                                                             : $"{drive.VolumeLabel} ({drive.Name})",
                    VolumePath = drive.RootDirectory.FullName,
                    VolumeSizeBytes = (ulong)drive.TotalSize,
                    IsValid = true
                };
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(this, $"Error in Windows drive enumeration, drive probably not ready: {ex.Message}");
                return new SafeMountedVolumeInfo()
                {
                    IsValid = false
                };
            }
        }

        /// <summary>
        /// Class SafeMountedVolumeInfo.
        /// Implements the <see cref="MountedVolumeInfo" />
        /// </summary>
        /// <seealso cref="MountedVolumeInfo" />
        private class SafeMountedVolumeInfo : MountedVolumeInfo
        {
            /// <summary>
            /// Returns true if ... is valid.
            /// </summary>
            /// <value>The is valid.</value>
            public bool IsValid { get; set; }
        }
    }

    #endregion Methods
}
