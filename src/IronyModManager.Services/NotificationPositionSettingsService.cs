// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 03-16-2021
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="NotificationPositionSettingsService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Class NotificationPositionSettingsService.
    /// Implements the <see cref="IronyModManager.Services.BaseService" />
    /// Implements the <see cref="IronyModManager.Services.Common.INotificationPositionSettingsService" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.BaseService" />
    /// <seealso cref="IronyModManager.Services.Common.INotificationPositionSettingsService" />
    public class NotificationPositionSettingsService : BaseService, INotificationPositionSettingsService
    {
        #region Fields

        /// <summary>
        /// The preferences service
        /// </summary>
        private readonly IPreferencesService preferencesService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationPositionSettingsService" /> class.
        /// </summary>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="mapper">The mapper.</param>
        public NotificationPositionSettingsService(IPreferencesService preferencesService, IStorageProvider storageProvider, IMapper mapper) : base(storageProvider, mapper)
        {
            this.preferencesService = preferencesService;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;INotificationPosition&gt;.</returns>
        public IEnumerable<INotificationPosition> Get()
        {
            var preferences = preferencesService.Get();

            var registeredItems = StorageProvider.GetNotificationPositions();
            var result = new List<INotificationPosition>();

            foreach (var item in registeredItems)
            {
                var model = InitModel(item, preferences.NotificationPosition);
                result.Add(model);
            }

            EnsureSelection(result, registeredItems);

            return result;
        }

        /// <summary>
        /// Saves the specified notification.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="InvalidOperationException">Not selected.</exception>
        public bool Save(INotificationPosition notification)
        {
            if (!notification.IsSelected)
            {
                throw new InvalidOperationException("Not selected.");
            }
            var preference = preferencesService.Get();

            return preferencesService.Save(Mapper.Map(notification, preference));
        }

        /// <summary>
        /// Ensures the selection.
        /// </summary>
        /// <param name="notifications">The notifications.</param>
        /// <param name="notificationTypes">The notification types.</param>
        protected virtual void EnsureSelection(IEnumerable<INotificationPosition> notifications, IEnumerable<INotificationPositionType> notificationTypes)
        {
            if (notifications.Any() && !notifications.Any(p => p.IsSelected))
            {
                notifications.FirstOrDefault(p => p.Type == notificationTypes.FirstOrDefault(s => s.IsDefault).Position).IsSelected = true;
            }
        }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        /// <param name="notiItem">The noti item.</param>
        /// <param name="notificationPosition">The notification position.</param>
        /// <returns>INotificationPosition.</returns>
        protected virtual INotificationPosition InitModel(INotificationPositionType notiItem, NotificationPosition notificationPosition)
        {
            var model = GetModelInstance<INotificationPosition>();
            model.Name = notiItem.Position.ToString();
            model.Type = notiItem.Position;
            model.IsSelected = notiItem.Position == notificationPosition;
            return model;
        }

        #endregion Methods
    }
}
