// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Avalonia
// Created          : 05-07-2020
//
// Last Modified By : Mario
// Last Modified On : 05-07-2020
// ***********************************************************************
// <copyright file="ManagedDialogItemViewModel.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>
// Based on Avalonia ManagedFileChooserItemViewModel. Why of why would
// the Avalonia guys expose some of this stuff?
// </summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using Avalonia.Dialogs;
using IronyModManager.Shared;

namespace IronyModManager.Controls.Dialogs
{
    /// <summary>
    /// Class ManagedDialogItemViewModel.
    /// Implements the <see cref="IronyModManager.Controls.Dialogs.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Controls.Dialogs.BaseViewModel" />
    [ExcludeFromCoverage("External logic.")]
    public class ManagedDialogItemViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The display name
        /// </summary>
        private string _displayName;

        /// <summary>
        /// The item type
        /// </summary>
        private ManagedFileChooserItemType _itemType;

        /// <summary>
        /// The modified
        /// </summary>
        private DateTime _modified;

        /// <summary>
        /// The path
        /// </summary>
        private string _path;

        /// <summary>
        /// The size
        /// </summary>
        private long _size;

        /// <summary>
        /// The type
        /// </summary>
        private string _type;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedDialogItemViewModel" /> class.
        /// </summary>
        public ManagedDialogItemViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedDialogItemViewModel" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ManagedDialogItemViewModel(ManagedDialogNavigationItem item)
        {
            ItemType = item.ItemType;
            Path = item.Path;
            DisplayName = item.DisplayName;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get => _displayName;
            set => RaiseAndSetIfChanged(ref _displayName, value);
        }

        /// <summary>
        /// Gets the icon key.
        /// </summary>
        /// <value>The icon key.</value>
        public string IconKey
        {
            get
            {
                return ItemType switch
                {
                    ManagedFileChooserItemType.Folder => "Icon_Folder",
                    ManagedFileChooserItemType.Volume => "Icon_Volume",
                    _ => "Icon_File",
                };
            }
        }

        /// <summary>
        /// Gets or sets the type of the item.
        /// </summary>
        /// <value>The type of the item.</value>
        public ManagedFileChooserItemType ItemType
        {
            get => _itemType;
            set => RaiseAndSetIfChanged(ref _itemType, value);
        }

        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        /// <value>The modified.</value>
        public DateTime Modified
        {
            get => _modified;
            set => RaiseAndSetIfChanged(ref _modified, value);
        }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get => _path;
            set => RaiseAndSetIfChanged(ref _path, value);
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public long Size
        {
            get => _size;
            set => RaiseAndSetIfChanged(ref _size, value);
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type
        {
            get => _type;
            set => RaiseAndSetIfChanged(ref _type, value);
        }

        #endregion Properties
    }
}
