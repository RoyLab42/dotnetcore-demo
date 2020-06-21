using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using RoyLab.DotNet.Core.Demo.Proxies;

namespace RoyLab.DotNet.Core.Demo
{
    public interface IUserRepository
    {
        Task<string> GetUserNameAsync(Guid userId);
    }

    public class UserRepository : IUserRepository
    {
        public async Task<string> GetUserNameAsync(Guid userId)
        {
            await Task.Delay(3000);
            return "roylab";
        }
    }

    [TestFixture]
    public class TestCacheDispatchProxy
    {
        [Test]
        public async Task TestCache()
        {
            var userRepository = new UserRepository();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var userName = await userRepository.GetUserNameAsync(Guid.Empty);
            stopwatch.Stop();
            Console.WriteLine($"userName {userName} was loaded in {stopwatch.ElapsedMilliseconds}ms");
            Assert.True(stopwatch.ElapsedMilliseconds >= 3000);

            var cachedRepository = CacheDispatchProxy<IUserRepository>.Create(userRepository,
                new MemoryCache(new MemoryCacheOptions()), TimeSpan.FromDays(1));
            stopwatch.Restart();
            userName = await cachedRepository.GetUserNameAsync(Guid.Empty);
            stopwatch.Stop();
            var firstLoadTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"[cache proxy] userName {userName} was loaded in {firstLoadTime}ms - first time");

            stopwatch.Restart();
            userName = await cachedRepository.GetUserNameAsync(Guid.Empty);
            stopwatch.Stop();
            Console.WriteLine(
                $"[cache proxy] userName {userName} was loaded in {stopwatch.ElapsedMilliseconds}ms - second time");
        }

        [Test]
        public async Task TestPerformance()
        {
            // arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(u => u.GetUserNameAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult("roylab"));
            var userRepository = mockUserRepository.Object;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var cachedUserRepository = CacheDispatchProxy<IUserRepository>.Create(userRepository,
                new MemoryCache(new MemoryCacheOptions()), TimeSpan.FromDays(1));
            stopwatch.Stop();
            Console.WriteLine($"dynamic proxy takes {stopwatch.ElapsedMilliseconds}ms to create");
            // act
            stopwatch.Restart();
            for (var i = 0; i < 100; i++)
            {
                await userRepository.GetUserNameAsync(Guid.Empty);
            }

            stopwatch.Stop();
            var backendRepositoryTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            for (var i = 0; i < 100; i++)
            {
                await cachedUserRepository.GetUserNameAsync(Guid.Empty);
            }

            stopwatch.Stop();
            var cachedRepositoryTime = stopwatch.ElapsedMilliseconds;
            // assert
            Console.WriteLine($"backend repository takes {backendRepositoryTime}ms to complete");
            Console.WriteLine($"cached repository takes {cachedRepositoryTime}ms to complete");
            var timeOverhead = cachedRepositoryTime - backendRepositoryTime;
            Console.WriteLine($"dispatch proxy has {timeOverhead}ms time overhead.");
        }
    }
}