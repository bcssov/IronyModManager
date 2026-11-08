// ***********************************************************************
// Assembly         : IronyModManager.Services
// ***********************************************************************
using System;
using System.Reflection;
using IronyModManager.DI.Extensions;
using IronyModManager.Services.Common;
using SimpleInjector;

namespace IronyModManager.Services
{
    /// <summary>
    /// Registers declarative game-state safety enforcement.
    /// </summary>
    public static class GameStateSafetyInterceptionExtensions
    {
        /// <summary>
        /// Intercepts the explicitly marked service contracts.
        /// </summary>
        public static void InterceptGameStateSafetyContracts(this Container container)
        {
            ArgumentNullException.ThrowIfNull(container);
            container.Register<GameStateSafetyInterceptor>();
            container.InterceptWith<GameStateSafetyInterceptor>(
                type => type.IsInterface && type.GetCustomAttribute<GameStateSafetyContractAttribute>() != null,
                true);
        }
    }
}
