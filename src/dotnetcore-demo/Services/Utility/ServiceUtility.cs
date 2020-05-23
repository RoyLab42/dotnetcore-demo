using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RoyLab.DotNet.Core.Demo.Services.Interfaces;

namespace RoyLab.DotNet.Core.Demo.Services.Utility
{
    internal static class ServiceUtility
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static async void FireAndForget(this Task task, Action<Exception> onException = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex) when (onException != null)
            {
                onException(ex);
            }
        }

        public static async Task LoadDataAsync()
        {
            await Task.Delay(5000); // simulate long running task
            Console.WriteLine($"Second thread: {Thread.CurrentThread.ManagedThreadId}");
            var authenticationService = ServiceProvider.GetRequiredService<ISomeSharedService>();
            Console.WriteLine("Second thread finished execution.");
        }
    }
}