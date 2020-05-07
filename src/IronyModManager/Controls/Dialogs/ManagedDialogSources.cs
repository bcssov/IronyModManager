// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 05-07-2020
// ***********************************************************************
// <copyright file="ManagedDialogSources.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ManagedFileChooserSources. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls.Platform;
using Avalonia.Dialogs;
using IronyModManager.Shared;

namespace IronyModManager.Controls.Dialogs
{
    /// <summary>
    /// Class ManagedDialogSources.
    /// </summary>
    [ExcludeFromCoverage("External logic.")]
    public class ManagedDialogSources
    {
        #region Fields

        /// <summary>
        /// The mounted volumes
        /// </summary>
        public static readonly ObservableCollection<MountedVolumeInfo> MountedVolumes = new ObservableCollection<MountedVolumeInfo>();

        /// <summary>
        /// The s folders
        /// </summary>
        private static readonly Environment.SpecialFolder[] s_folders = new[]
        {
            Environment.SpecialFolder.Desktop,
            Environment.SpecialFolder.UserProfile,
            Environment.SpecialFolder.MyDocuments,
            Environment.SpecialFolder.MyMusic,
            Environment.SpecialFolder.MyPictures,
            Environment.SpecialFolder.MyVideos
        };

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the get all items delegate.
        /// </summary>
        /// <value>The get all items delegate.</value>
        public Func<ManagedDialogSources, ManagedDialogNavigationItem[]> GetAllItemsDelegate { get; set; }
            = DefaultGetAllItems;

        /// <summary>
        /// Gets or sets the get file system roots.
        /// </summary>
        /// <value>The get file system roots.</value>
        public Func<ManagedDialogNavigationItem[]> GetFileSystemRoots { get; set; }
            = DefaultGetFileSystemRoots;

        /// <summary>
        /// Gets or sets the get user directories.
        /// </summary>
        /// <value>The get user directories.</value>
        public Func<ManagedDialogNavigationItem[]> GetUserDirectories { get; set; }
            = DefaultGetUserDirectories;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Defaults the get all items.
        /// </summary>
        /// <param name="sources">The sources.</param>
        /// <returns>ManagedDialogNavigationItem[].</returns>
        public static ManagedDialogNavigationItem[] DefaultGetAllItems(ManagedDialogSources sources)
        {
            return sources.GetUserDirectories().Concat(sources.GetFileSystemRoots()).ToArray();
        }

        /// <summary>
        /// Defaults the get file system roots.
        /// </summary>
        /// <returns>ManagedDialogNavigationItem[].</returns>
        public static ManagedDialogNavigationItem[] DefaultGetFileSystemRoots()
        {
            return MountedVolumes
                   .Select(x =>
                   {
                       var displayName = x.VolumeLabel;

                       if (displayName == null & x.VolumeSizeBytes > 0)
                       {
                           displayName = $"{ByteSizeHelper.ToString(x.VolumeSizeBytes)} Volume";
                       };

                       try
                       {
                           Directory.GetFiles(x.VolumePath);
                       }
                       catch (UnauthorizedAccessException)
                       {
                           return null;
                       }

                       return new ManagedDialogNavigationItem
                       {
                           ItemType = ManagedFileChooserItemType.Volume,
                           DisplayName = displayName,
                           Path = x.VolumePath
                       };
                   })
                   .Where(x => x != null)
                   .ToArray();
        }

        /// <summary>
        /// Defaults the get user directories.
        /// </summary>
        /// <returns>ManagedDialogNavigationItem[].</returns>
        public static ManagedDialogNavigationItem[] DefaultGetUserDirectories()
        {
            return s_folders.Select(Environment.GetFolderPath).Distinct()
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .Where(Directory.Exists)
                .Select(d => new ManagedDialogNavigationItem
                {
                    ItemType = ManagedFileChooserItemType.Folder,
                    Path = d,
                    DisplayName = Path.GetFileName(d)
                }).ToArray();
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>ManagedDialogNavigationItem[].</returns>
        public ManagedDialogNavigationItem[] GetAllItems() => GetAllItemsDelegate(this);

        #endregion Methods
    }
}
