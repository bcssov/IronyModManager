// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************
using System;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Records that a method on a safety-participating service contract intentionally uses no standardized interceptor policy.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public sealed class GameStateSafetyExemptAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameStateSafetyExemptAttribute" /> class.
        /// </summary>
        /// <param name="reason">The optional reason for the exemption.</param>
        public GameStateSafetyExemptAttribute(string reason = null)
        {
            Reason = reason;
        }

        /// <summary>
        /// Gets the reason for the exemption.
        /// </summary>
        public string Reason { get; }
    }
}
