using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using EventBus.InProcess.Benckmark.Events;
using EventBus.InProcess.Benckmark.Handlers;
using EventBus.InProcess.Internals;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EventBus.InProcess.Benckmark
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 3, warmupCount: 5, targetCount: 50)]
    [RPlotExporter, RankColumn]
    public class EventBusInProcessBenchmark
    {
        private EventRecorder _eventRecorder;
        private Task[] _pulishTasks;
        private IEventBus _bus;

        [Params(100, 10000)]
        public int MessageQty;

        public const int OneSub = 1;
        public const int FiveSubs = 5;

        [Params(OneSub, FiveSubs)]
        public int SubsQty;

        [GlobalSetup]
        public void Setup()
        {
            _eventRecorder = new EventRecorder();
            _pulishTasks = new Task[MessageQty];
            _bus = SetupEventBus();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _bus.Dispose();
        }

        [Benchmark]
        public int EventBusInProcess()
        {
            for (var i = 0; i < MessageQty; i++)
            {
                _pulishTasks[i] = Task.Run(() =>
                _bus.Publish(new UserInfoUpdatedEvent
                {
                    UpdatedUser = new User()
                }));
            }

            Task.WaitAll(_pulishTasks);

            var handledEventsNumber = MessageQty * SubsQty;
            while (_eventRecorder.NumberHandledEvents < handledEventsNumber)
            {
            }
            return _eventRecorder.NumberHandledEvents;
        }

        private IEventBus SetupEventBus()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var bus = serviceProvider.GetRequiredService<IEventBus>();
            AddSubsctibers(bus);
            return bus;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => _eventRecorder);
            services.AddSingleton<IChannelManager,ChannelManager>();
            services.AddSingleton<IEventBusSubscriptionManager ,InMemorySubscriptionManager>();
            services.AddEventBus();
            RegisterHandlers(services);
        }

        private void RegisterHandlers(IServiceCollection services)
        {
            switch (SubsQty)
            {
                case OneSub:
                    {
                        services.AddScoped<UserInfoUpdatedHandler>();
                        break;
                    }
                case FiveSubs:
                    {
                        services.AddScoped<UserInfoUpdatedHandler>();
                        services.AddScoped<UserInfoUpdatedHandlerOne>();
                        services.AddScoped<UserInfoUpdatedHandlerTwo>();
                        services.AddScoped<UserInfoUpdatedHandlerThree>();
                        services.AddScoped<UserInfoUpdatedHandlerFour>();
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException(nameof(SubsQty));
            }
        }

        private void AddSubsctibers(IEventBus bus)
        {
            switch (SubsQty)
            {
                case OneSub:
                    {
                        bus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandler>();
                        break;
                    }
                case FiveSubs:
                    {
                        bus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandler>();
                        bus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandlerOne>();
                        bus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandlerTwo>();
                        bus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandlerThree>();
                        bus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandlerFour>();
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException(nameof(SubsQty));
            }
        }
    }
}