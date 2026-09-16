// ***********************************************************************
// Assembly         : IronyModManager.Services
// ***********************************************************************
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using IronyModManager.IO.Common.FileSystem;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;

namespace IronyModManager.Services
{
    /// <summary>
    /// Enforces the standardized portion of game-state safety contracts.
    /// </summary>
    public class GameStateSafetyInterceptor(
        IGameService gameService,
        IGameStateSafetyService gameStateSafetyService,
        IFileSystemStateProbe fileSystemStateProbe) : IInterceptor
    {
        private static readonly MethodInfo interceptGenericTaskMethod = typeof(GameStateSafetyInterceptor)
            .GetMethod(nameof(InterceptGenericTaskAsync), BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo taskFromResultMethod = typeof(Task).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(p => p.Name == nameof(Task.FromResult));

        /// <summary>
        /// Intercepts a service invocation.
        /// </summary>
        public virtual void Intercept(IInvocation invocation)
        {
            var safety = invocation.Method.GetCustomAttribute<GameStateSafetyAttribute>(true);
            if (safety == null)
            {
                invocation.Proceed();
                return;
            }

            Validate(safety);

            var game = ResolveGame(invocation);
            if ((game == null && safety.RejectWhenGameUnavailable) ||
                (game != null && safety.RejectWhenLocked && gameStateSafetyService.IsLocked(game) &&
                 !OwnsCurrentRevalidation(invocation, game, safety)))
            {
                SetRejectedResult(invocation, safety.Rejection);
                return;
            }

            try
            {
                invocation.Proceed();
            }
            catch (Exception exception) when (ShouldLock(game, safety, exception))
            {
                Lock(game, safety, exception);
                SetRejectedResult(invocation, safety.Rejection);
                return;
            }

            if (!safety.LockOnFileSystemFailure || game == null)
            {
                return;
            }

            var returnType = invocation.Method.ReturnType;
            if (returnType == typeof(Task))
            {
                invocation.ReturnValue = InterceptTaskAsync((Task)invocation.ReturnValue, game, safety);
            }
            else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                invocation.ReturnValue = interceptGenericTaskMethod.MakeGenericMethod(returnType.GenericTypeArguments[0])
                    .Invoke(this, [invocation.ReturnValue, game, safety]);
            }
        }

        private async Task InterceptTaskAsync(Task task, IGame game, GameStateSafetyAttribute safety)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception exception) when (ShouldLock(game, safety, exception))
            {
                Lock(game, safety, exception);
            }
        }

        private async Task<T> InterceptGenericTaskAsync<T>(Task<T> task, IGame game, GameStateSafetyAttribute safety)
        {
            try
            {
                return await task.ConfigureAwait(false);
            }
            catch (Exception exception) when (ShouldLock(game, safety, exception))
            {
                Lock(game, safety, exception);
                return (T)CreateRejectedValue(typeof(T), safety.Rejection);
            }
        }

        private static object CreateRejectedValue(Type resultType, GameStateSafetyRejection rejection)
        {
            return rejection switch
            {
                GameStateSafetyRejection.False when resultType == typeof(bool) => false,
                GameStateSafetyRejection.Null when !resultType.IsValueType || Nullable.GetUnderlyingType(resultType) != null => null,
                _ => throw new InvalidOperationException($"Rejection strategy '{rejection}' is not valid for result type '{resultType}'.")
            };
        }

        private static IGame ResolveExplicitGame(IInvocation invocation)
        {
            var parameters = invocation.Method.GetParameters();
            for (var index = 0; index < parameters.Length; index++)
            {
                if (typeof(IGame).IsAssignableFrom(parameters[index].ParameterType))
                {
                    return invocation.Arguments[index] as IGame;
                }
            }
            return null;
        }

        private IGame ResolveGame(IInvocation invocation)
        {
            var hasExplicitGame = invocation.Method.GetParameters()
                .Any(parameter => typeof(IGame).IsAssignableFrom(parameter.ParameterType));
            return hasExplicitGame ? ResolveExplicitGame(invocation) : gameService.GetSelected();
        }

        private bool OwnsCurrentRevalidation(IInvocation invocation, IGame game, GameStateSafetyAttribute safety)
        {
            if (!safety.AllowCurrentRevalidation)
            {
                return false;
            }

            var revalidationLock = invocation.Arguments.OfType<GameStateLockInfo>().SingleOrDefault();
            return gameStateSafetyService.IsCurrentRevalidation(game, revalidationLock);
        }

        private bool ShouldLock(IGame game, GameStateSafetyAttribute safety, Exception exception)
        {
            return game != null && safety.LockOnFileSystemFailure && fileSystemStateProbe.IsFileSystemAccessFailure(exception);
        }

        private void Lock(IGame game, GameStateSafetyAttribute safety, Exception exception)
        {
            gameStateSafetyService.Lock(game, safety.FailureReason!.Value, safety.Context, exception);
        }

        private static void Validate(GameStateSafetyAttribute safety)
        {
            if (safety.LockOnFileSystemFailure != safety.FailureReason.HasValue)
            {
                throw new InvalidOperationException(
                    $"Game-state safety context '{safety.Context}' must declare a failure reason exactly when filesystem-failure locking is enabled.");
            }
        }

        private static void SetRejectedResult(IInvocation invocation, GameStateSafetyRejection rejection)
        {
            var returnType = invocation.Method.ReturnType;
            if (returnType == typeof(Task))
            {
                if (rejection != GameStateSafetyRejection.CompletedTask)
                {
                    throw new InvalidOperationException($"Rejection strategy '{rejection}' is not valid for a non-generic Task.");
                }
                invocation.ReturnValue = Task.CompletedTask;
                return;
            }

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = returnType.GenericTypeArguments[0];
                var rejected = CreateRejectedValue(resultType, rejection);
                invocation.ReturnValue = taskFromResultMethod.MakeGenericMethod(resultType).Invoke(null, [rejected]);
                return;
            }

            invocation.ReturnValue = CreateRejectedValue(returnType, rejection);
        }
    }
}
