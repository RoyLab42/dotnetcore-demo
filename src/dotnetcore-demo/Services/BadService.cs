using System;
using System.Threading;
using RoyLab.DotNet.Core.Demo.Services.Interfaces;
using RoyLab.DotNet.Core.Demo.Services.Utility;

namespace RoyLab.DotNet.Core.Demo.Services
{
    /// <summary>
    /// This class is to demo how deadlock happens when executing an async operation and then
    /// waiting for it finished execution.
    ///
    /// We should always be cautious when waiting for other threads.
    ///
    /// The example showed below is one thread waiting for another, this is "common" when people want to initialize
    /// some data asynchronously in constructor. (Maybe the data initialization is time consuming?). BUT, it does not
    /// make sense why one thread create another to do some task, and waiting for it.
    /// </summary>
    public class BadService : IService
    {
        public BadService()
        {
            Console.WriteLine($"Caller thread: {Thread.CurrentThread.ManagedThreadId}");
            ServiceUtility.LoadDataAsync().Wait();
            // caller thread locked
        }
    }
}