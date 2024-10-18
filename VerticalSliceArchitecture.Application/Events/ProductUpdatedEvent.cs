using VerticalSliceArchitecture.Application.Event;

namespace VerticalSliceArchitecture.Application.Events
{
    public class ProductUpdatedEvent : BaseEvent
    {
        public ProductUpdatedEvent()
        {
            EventType = nameof(ProductUpdatedEvent);
        }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
    }
}
