// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************
using System;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Marks a service contract whose methods must explicitly declare their game-state safety policy.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false)]
    public sealed class GameStateSafetyContractAttribute : Attribute
    {
    }
}
