namespace EventBus.InProcess.Benckmark.Events
{
    internal class UserInfoUpdatedEvent : IntegrationEvent
    {
        public User UpdatedUser { get; set; }
    }
}