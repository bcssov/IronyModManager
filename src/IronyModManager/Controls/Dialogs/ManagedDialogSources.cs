// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 09-08-2020
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
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Shared;
using SmartFormat;

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
            Environment.SpecialFolder.MyVideos,
        };

        /// <summary>
        /// The localization manager
        /// </summary>
        private static ILocalizationManager localizationManager;

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
            InitLocalizationManager();
            return MountedVolumes
                   .Select(x =>
                   {
                       var displayName = x.VolumeLabel;

                       if (displayName == null & x.VolumeSizeBytes > 0)
                       {
                           displayName = Smart.Format(localizationManager.GetResource(LocalizationResources.FileDialog.Volume), new { Size = ByteSizeHelper.ToString(x.VolumeSizeBytes) });
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
            return s_folders.Select(s => KeyValuePair.Create(s, Environment.GetFolderPath(s))).GroupBy(p => p.Value).Select(p => p.First())
                .Where(d => !string.IsNullOrWhiteSpace(d.Value))
                .Where(d => Directory.Exists(d.Value))
                .Select(d => new ManagedDialogNavigationItem
                {
                    ItemType = ManagedFileChooserItemType.Folder,
                    Path = d.Value,
                    DisplayName = LocalizeFolder(d.Key, Path.GetFileName(d.Value))
                }).ToArray();
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>ManagedDialogNavigationItem[].</returns>
        public ManagedDialogNavigationItem[] GetAllItems() => GetAllItemsDelegate(this);

        /// <summary>
        /// Initializes the localization manager.
        /// </summary>
        private static void InitLocalizationManager()
        {
            if (localizationManager == null)
            {
                localizationManager = DIResolver.Get<ILocalizationManager>();
            }
        }

        /// <summary>
        /// Localizes the folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        private static string LocalizeFolder(Environment.SpecialFolder folder, string path)
        {
            InitLocalizationManager();
            string localized = string.Empty;
            switch (folder)
            {
                case Environment.SpecialFolder.Desktop:
                    localized = localizationManager.GetResource(LocalizationResources.FileDialog.Folders.Desktop);
                    break;

                case Environment.SpecialFolder.MyDocuments:
                    localized = localizationManager.GetResource(LocalizationResources.FileDialog.Folders.Documents);
                    break;

                case Environment.SpecialFolder.MyMusic:
                    localized = localizationManager.GetResource(LocalizationResources.FileDialog.Folders.Music);
                    break;

                case Environment.SpecialFolder.MyPictures:
                    localized = localizationManager.GetResource(LocalizationResources.FileDialog.Folders.Pictures);
                    break;

                case Environment.SpecialFolder.MyVideos:
                    localized = localizationManager.GetResource(LocalizationResources.FileDialog.Folders.Videos);
                    break;

                default:
                    break;
            }
            if (!string.IsNullOrWhiteSpace(localized))
            {
                return localized;
            }
            return path;
        }

        #endregion Methods
    }
}
