namespace EventBus.InProcess.Benchmark.Events
{
    internal class UserInfoUpdatedEvent : IntegrationEvent
    {
        public User UpdatedUser { get; set; }
    }
}