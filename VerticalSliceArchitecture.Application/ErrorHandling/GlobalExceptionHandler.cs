using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using VerticalSliceArchitecture.Domain.Models;

public class GlobalExceptionHandler<TRequest, TResponse, TException> 
    : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TException : Exception
{
    private readonly ILogger<GlobalExceptionHandler<TRequest, TResponse, TException>> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        var response = new StandardResponse
        {
            Success = false,
            Message = "An unexpected error occurred",
            ExceptionMessage = exception.Message
        };

        // Ghi log thông tin chi tiết ngoại lệ
        _logger.LogError(exception, "An error occurred while processing the request for {RequestType}", typeof(TRequest).Name);

        state.SetHandled((TResponse)(object)response);
        return Task.CompletedTask;
    }
}
