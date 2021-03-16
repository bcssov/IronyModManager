// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 03-16-2021
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="NotificationPositionRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services.Registrations
{
    /// <summary>
    /// Class NotificationPositionRegistration.
    /// Implements the <see cref="IronyModManager.Shared.PostStartup" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.PostStartup" />
    public class NotificationPositionRegistration : PostStartup
    {
        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            var storage = DIResolver.Get<IStorageProvider>();
            foreach (NotificationPosition type in Enum.GetValues(typeof(NotificationPosition)))
            {
                var model = DIResolver.Get<INotificationPositionType>();
                model.IsDefault = type == NotificationPosition.BottomRight;
                model.Position = type;
                storage.RegisterNotificationPosition(model);
            }
        }

        #endregion Methods
    }
}
