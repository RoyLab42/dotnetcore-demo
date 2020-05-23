using System.Threading;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RoyLab.DotNet.Core.Demo.Services;
using RoyLab.DotNet.Core.Demo.Services.Interfaces;
using RoyLab.DotNet.Core.Demo.Services.Utility;

namespace RoyLab.DotNet.Core.Demo
{
    public class TestAsyncInConstructor
    {
        [SetUp]
        public void Setup()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<BadService>().AsSelf().InstancePerLifetimeScope();
            containerBuilder.RegisterType<BetterService>().AsSelf().InstancePerLifetimeScope();
            containerBuilder.RegisterType<EvenBetterService>().AsSelf().InstancePerLifetimeScope();
            containerBuilder.RegisterType<SomeSharedService>().As<ISomeSharedService>()
                .InstancePerLifetimeScope();
            var container = containerBuilder.Build();
            ServiceUtility.ServiceProvider = new AutofacServiceProvider(container);
        }

        [Test]
        public void TestBadServiceLeadsToDeadlock()
        {
            var service = ServiceUtility.ServiceProvider.GetService<BadService>();
            Assert.NotNull(service);
            // two threads locked, this method will never return
        }

        [Test]
        public void TestBetterService()
        {
            var service = ServiceUtility.ServiceProvider.GetService<BetterService>();
            Assert.NotNull(service);
            Thread.Sleep(10000);
        }

        [Test]
        public void TestEvenBetterService()
        {
            var service = ServiceUtility.ServiceProvider.GetService<EvenBetterService>();
            Assert.NotNull(service);
            Thread.Sleep(10000);
        }
    }
}