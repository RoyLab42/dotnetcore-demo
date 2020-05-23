using System;
using System.Threading;
using RoyLab.DotNet.Core.Demo.Services.Interfaces;
using RoyLab.DotNet.Core.Demo.Services.Utility;

namespace RoyLab.DotNet.Core.Demo.Services
{
    /// <summary>
    /// And better approach to avoid Task.Wait() or Task.GetAwaiter().GetResult() is let it run asynchronously.
    /// </summary>
    public class BetterService : IService
    {
        public BetterService()
        {
            Console.WriteLine($"Caller thread: {Thread.CurrentThread.ManagedThreadId}");
            ServiceUtility.LoadDataAsync();
            Console.WriteLine("Caller thread finished execution.");
        }
    }
}