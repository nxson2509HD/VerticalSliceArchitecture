
using VerticalSliceArchitecture.Domain.Event;

namespace VerticalSliceArchitecture.Domain.Services
{
    public interface IKafkaProducerService
    {
        Task ProduceAsync(string topic, IEvent @event);
    }
}
