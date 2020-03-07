// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-07-2020
// ***********************************************************************
// <copyright file="NotificationManager.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary>Based on Avalonia's inbuilt WindowNotificationManager.</summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Rendering;
using Avalonia.VisualTree;
using IronyModManager.Shared;

namespace IronyModManager.Controls
{
    /// <summary>
    /// Class NotificationManager.
    /// Implements the <see cref="Avalonia.Controls.Primitives.TemplatedControl" />
    /// Implements the <see cref="Avalonia.Controls.Notifications.IManagedNotificationManager" />
    /// Implements the <see cref="Avalonia.Rendering.ICustomSimpleHitTest" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Primitives.TemplatedControl" />
    /// <seealso cref="Avalonia.Controls.Notifications.IManagedNotificationManager" />
    /// <seealso cref="Avalonia.Rendering.ICustomSimpleHitTest" />
    [ExcludeFromCoverage("UI Elements should be tested in functional testing.")]
    public class NotificationManager : TemplatedControl, IManagedNotificationManager, ICustomSimpleHitTest
    {
        #region Fields

        /// <summary>
        /// The maximum items property
        /// </summary>
        public static readonly StyledProperty<int> MaxItemsProperty =
          AvaloniaProperty.Register<NotificationManager, int>(nameof(MaxItems), 5);

        /// <summary>
        /// Gets the position property.
        /// </summary>
        /// <value>The position property.</value>
        public static readonly StyledProperty<NotificationPosition> PositionProperty =
          AvaloniaProperty.Register<NotificationManager, NotificationPosition>(nameof(Position), NotificationPosition.TopRight);

        /// <summary>
        /// The items
        /// </summary>
        private IList _items;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="NotificationManager" /> class.
        /// </summary>
        static NotificationManager()
        {
            PseudoClass<NotificationManager, NotificationPosition>(PositionProperty, x => x == NotificationPosition.TopLeft, ":topleft");
            PseudoClass<NotificationManager, NotificationPosition>(PositionProperty, x => x == NotificationPosition.TopRight, ":topright");
            PseudoClass<NotificationManager, NotificationPosition>(PositionProperty, x => x == NotificationPosition.BottomLeft, ":bottomleft");
            PseudoClass<NotificationManager, NotificationPosition>(PositionProperty, x => x == NotificationPosition.BottomRight, ":bottomright");

            HorizontalAlignmentProperty.OverrideDefaultValue<NotificationManager>(Avalonia.Layout.HorizontalAlignment.Stretch);
            VerticalAlignmentProperty.OverrideDefaultValue<NotificationManager>(Avalonia.Layout.VerticalAlignment.Stretch);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationManager" /> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public NotificationManager(Window host)
        {
            if (VisualChildren.Count != 0)
            {
                Install(host);
            }
            else
            {
                Observable.FromEventPattern<TemplateAppliedEventArgs>(host, nameof(host.TemplateApplied)).Take(1)
                    .Subscribe(_ =>
                    {
                        Install(host);
                    });
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the maximum items.
        /// </summary>
        /// <value>The maximum items.</value>
        public int MaxItems
        {
            get { return GetValue(MaxItemsProperty); }
            set { SetValue(MaxItemsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public NotificationPosition Position
        {
            get { return GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Hits the test.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool HitTest(Point point) => VisualChildren.HitTestCustom(point);

        /// <summary>
        /// Shows the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void Show(INotification content)
        {
            Show(content as object);
        }

        /// <summary>
        /// Shows the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        public async void Show(object content)
        {
            var notification = content as INotification;

            var notificationControl = new NotificationCard
            {
                Content = content
            };

            if (notification != null)
            {
                notificationControl.NotificationClosed += (sender, args) =>
                {
                    notification.OnClose?.Invoke();

                    _items.Remove(sender);
                };
            }

            notificationControl.PointerPressed += (sender, args) =>
            {
                if (notification != null && notification.OnClick != null)
                {
                    notification.OnClick.Invoke();
                }

                (sender as NotificationCard)?.Close();
            };

            _items.Add(notificationControl);

            if (_items.OfType<NotificationCard>().Count(i => !i.IsClosing) > MaxItems)
            {
                _items.OfType<NotificationCard>().First(i => !i.IsClosing).Close();
            }

            if (notification != null && notification.Expiration == TimeSpan.Zero)
            {
                return;
            }

            await Task.Delay(notification?.Expiration ?? TimeSpan.FromSeconds(5));

            notificationControl.Close();
        }

        /// <summary>
        /// Handles the <see cref="E:TemplateApplied" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs" /> instance containing the event data.</param>
        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            base.OnTemplateApplied(e);

            var itemsControl = e.NameScope.Find<Panel>("PART_Items");
            _items = itemsControl?.Children;
        }

        /// <summary>
        /// Installs the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        private void Install(Window host)
        {
            var adornerLayer = host.GetVisualDescendants()
                .OfType<VisualLayerManager>()
                .FirstOrDefault()
                ?.AdornerLayer;

            if (adornerLayer != null)
            {
                adornerLayer.Children.Add(this);
            }
        }

        #endregion Methods
    }
}
