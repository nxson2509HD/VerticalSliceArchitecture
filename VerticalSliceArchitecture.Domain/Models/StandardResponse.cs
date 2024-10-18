using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerticalSliceArchitecture.Domain.Models
{
    public class StandardResponse<TData, TErrorData>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public TData? Data { get; set; }
        public TErrorData? ErrorData { get; set; }
        public string? ExceptionMessage { get; set; }
        public Paging? Paging { get; set; }
        public string? ClientRequestId { get; set; }

        public StandardResponse(bool success = true)
        {
            this.Success = success;
        }
    }

    public class StandardResponse<TData> : StandardResponse<TData, object>
    {
        public StandardResponse(bool success = true)
          : base(success)
        {
        }
    }

    public class StandardResponse : StandardResponse<object, object>
    {
        public StandardResponse(bool success = true)
          : base(success)
        {
        }
    }

    public class Paging
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
    }
}
