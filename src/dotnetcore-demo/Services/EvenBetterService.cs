using System;
using System.Threading;
using RoyLab.DotNet.Core.Demo.Services.Interfaces;
using RoyLab.DotNet.Core.Demo.Services.Utility;

namespace RoyLab.DotNet.Core.Demo.Services
{
    /// <summary>
    /// An even better approach it to use the FireAndForget extension method.
    ///
    /// As we can not await an async function here, so we cannot catch the exception in async function either.
    /// Use the FireAndForget, we can make sure exceptions handled, and resources released properly.
    /// </summary>
    public class EvenBetterService : IService
    {
        public EvenBetterService()
        {
            Console.WriteLine($"Caller thread: {Thread.CurrentThread.ManagedThreadId}");
            ServiceUtility.LoadDataAsync().FireAndForget(e => { Console.WriteLine($"failed loading data: {e}"); });
            Console.WriteLine("Caller thread finished execution.");
        }
    }
}