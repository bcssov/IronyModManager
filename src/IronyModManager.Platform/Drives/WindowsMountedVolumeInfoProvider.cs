// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 05-12-2023
//
// Last Modified By : Mario
// Last Modified On : 05-12-2023
// ***********************************************************************
// <copyright file="WindowsMountedVolumeInfoProvider.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls.Platform;

namespace IronyModManager.Platform.Drives
{
    /// <summary>
    /// Class WindowsMountedVolumeInfoProvider.
    /// Implements the <see cref="IMountedVolumeInfoProvider" />
    /// </summary>
    /// <seealso cref="IMountedVolumeInfoProvider" />
    public class WindowsMountedVolumeInfoProvider : IMountedVolumeInfoProvider
    {
        #region Methods

        /// <summary>
        /// Listens to any changes in volume mounts and
        /// forwards updates to the referenced
        /// <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" />.
        /// </summary>
        /// <param name="mountedDrives">The mounted drives.</param>
        /// <returns>IDisposable.</returns>
        public IDisposable Listen(ObservableCollection<MountedVolumeInfo> mountedDrives)
        {
            Contract.Requires<ArgumentNullException>(mountedDrives != null);
            return new WindowsMountedVolumeInfoListener(mountedDrives);
        }

        #endregion Methods
    }
}
