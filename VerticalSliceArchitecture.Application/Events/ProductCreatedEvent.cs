

using System.ComponentModel.DataAnnotations;
using VerticalSliceArchitecture.Application.Event;

namespace VerticalSliceArchitecture.Application.Events
{
    public class ProductCreatedEvent : BaseEvent
    {
        public ProductCreatedEvent()
        {
            EventType = nameof(ProductCreatedEvent);
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
