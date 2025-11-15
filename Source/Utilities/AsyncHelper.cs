using Godot;
using System;
using System.Threading.Tasks;

namespace Core.Utilities
{
    /// <summary>
    /// Helper utilities for async operations in Godot
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// Executes an async task and calls a callback on completion
        /// Safe to call from Godot's main thread
        /// </summary>
        public static async void RunAsync(Func<Task> asyncMethod, Action onComplete = null, Action<Exception> onError = null)
        {
            try
            {
                await asyncMethod();
                onComplete?.Invoke();
            }
            catch (Exception ex)
            {
                if (onError != null)
                {
                    onError(ex);
                }
                else
                {
                    GD.PrintErr($"Unhandled async exception: {ex}");
                }
            }
        }

        /// <summary>
        /// Executes an async task with a return value
        /// </summary>
        public static async void RunAsync<T>(Func<Task<T>> asyncMethod, Action<T> onComplete, Action<Exception> onError = null)
        {
            try
            {
                var result = await asyncMethod();
                onComplete?.Invoke(result);
            }
            catch (Exception ex)
            {
                if (onError != null)
                {
                    onError(ex);
                }
                else
                {
                    GD.PrintErr($"Unhandled async exception: {ex}");
                }
            }
        }
    }
}
