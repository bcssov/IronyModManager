// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// ***********************************************************************
using System;

namespace IronyModManager.Services.Common
{
    /// <summary>
    /// Declares standardized game-state safety enforcement for a service operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public sealed class GameStateSafetyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameStateSafetyAttribute" /> class.
        /// </summary>
        public GameStateSafetyAttribute(GameStateSafetyOperation operation, GameStateSafetyRejection rejection, string context,
            GameStateLockReason failureReason, GameStateSafetyEnforcement enforcement = GameStateSafetyEnforcement.Standard)
        {
            Operation = operation;
            Rejection = rejection;
            Context = context;
            Enforcement = enforcement;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Initializes a new entry-only instance of the <see cref="GameStateSafetyAttribute" /> class.
        /// </summary>
        public GameStateSafetyAttribute(GameStateSafetyOperation operation, GameStateSafetyRejection rejection, string context,
            GameStateSafetyEnforcement enforcement)
        {
            Operation = operation;
            Rejection = rejection;
            Context = context;
            Enforcement = enforcement;
        }

        /// <summary>
        /// Gets the operation kind.
        /// </summary>
        public GameStateSafetyOperation Operation { get; }

        /// <summary>
        /// Gets the rejection behavior.
        /// </summary>
        public GameStateSafetyRejection Rejection { get; }

        /// <summary>
        /// Gets the diagnostic context.
        /// </summary>
        public string Context { get; }

        /// <summary>
        /// Gets the standardized enforcement capabilities.
        /// </summary>
        public GameStateSafetyEnforcement Enforcement { get; }

        /// <summary>
        /// Gets the reason used when this contract converts a classified filesystem failure into a lock.
        /// </summary>
        public GameStateLockReason? FailureReason { get; }

        /// <summary>
        /// Gets a value indicating whether an invocation is rejected when its game is already locked.
        /// </summary>
        public bool RejectWhenLocked => (Enforcement & GameStateSafetyEnforcement.RejectWhenLocked) != 0;

        /// <summary>
        /// Gets a value indicating whether an invocation is rejected when no game can be resolved.
        /// </summary>
        public bool RejectWhenGameUnavailable => (Enforcement & GameStateSafetyEnforcement.RejectWhenGameUnavailable) != 0;

        /// <summary>
        /// Gets a value indicating whether classified filesystem failures lock the captured game.
        /// </summary>
        public bool LockOnFileSystemFailure => (Enforcement & GameStateSafetyEnforcement.LockOnFileSystemFailure) != 0;

        /// <summary>
        /// Gets a value indicating whether the current controlled-revalidation generation may enter while locked.
        /// </summary>
        public bool AllowCurrentRevalidation => (Enforcement & GameStateSafetyEnforcement.AllowCurrentRevalidation) != 0;
    }
}
