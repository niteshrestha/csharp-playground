using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parallel
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            List<Task<int>> tasks = new List<Task<int>>();
            int[] n = new int[] { 1, 2, 3, 4, 5 };
            foreach (var item in n)
            {
                tasks.Add(Get(item));
            }

            await Task.WhenAll(tasks);

            foreach (var item in tasks)
            {
                Console.WriteLine(item.Result);
            }
        }

        private static async Task<int> Get(int num)
        {
            await Task.Delay(5000);
            return num;
        }
    }
}