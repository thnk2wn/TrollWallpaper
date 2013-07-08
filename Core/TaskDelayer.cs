using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// Start a task delayed
    /// </summary>
    /// <remarks>
    ///     <see cref="http://stackoverflow.com/questions/4229923/how-to-schedule-a-task-for-future-execution-in-task-parallel-library"/>
    /// </remarks>
    public static class TaskDelayer
    {
        public static Task<T> RunDelayed<T>(int millisecondsDelay, Func<T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }
            if (millisecondsDelay < 0)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }

            var taskCompletionSource = new TaskCompletionSource<T>();

            var timer = new Timer(self =>
            {
                ((Timer)self).Dispose();
                try
                {
                    var result = func();
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception exception)
                {
                    taskCompletionSource.SetException(exception);
                }
            });
            timer.Change(millisecondsDelay, millisecondsDelay);

            return taskCompletionSource.Task;
        }
    }
}
