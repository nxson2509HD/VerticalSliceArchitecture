namespace VerticalSliceArchitecture.Domain.Event
{
    public interface IEvent
    {
        string EventType { get; }
        DateTime Timestamp { get; }
    }
}
