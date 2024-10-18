using VerticalSliceArchitecture.Application.Event;

namespace VerticalSliceArchitecture.Application.Events
{
    public class ProductDeletedEvent : BaseEvent
    {
        public ProductDeletedEvent()
        {
            EventType = nameof(ProductDeletedEvent);
        }

        public int Id { get; set; }
    }
}
