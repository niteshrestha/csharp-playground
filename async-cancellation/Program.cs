using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace async_cancellation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("PROGRAM STARTED");
            CancellationTokenSource source = new CancellationTokenSource();
            Task<bool> status = ShouldIRun4Tasks();
            List<Task<string>> taskList2 = new List<Task<string>>();
            taskList2.Add(LongRunningCancelableTask("Task 1", source));
            taskList2.Add(LongRunningCancelableTask("Task 2", source));
            taskList2.Add(LongRunningCancelableTask("Task 3", source));
            taskList2.Add(LongRunningCancelableTask("Task 4", source));
            await Task.WhenAll(status);
            Console.WriteLine(await status);
            if (!await status)
            {
                source.Cancel();
            }
            await Task.WhenAll(taskList2);
            taskList2?.ForEach(t =>
            {
                Console.WriteLine(t.Result);
            });
        }

        private static Task<bool> ShouldIRun4Tasks()
        {
            return Task.Run(() =>
            {
                Thread.Sleep(1000);
                return true;
            });
        }

        private static Task<String> LongRunningCancelableTask(string input, CancellationTokenSource cancellationToken)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(2000);
                return input;
            });
        }
    }
}
