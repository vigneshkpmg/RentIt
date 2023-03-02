using Kernal.Events;
using Serilog.Context;
using web.Infrastructure.Repository;
using web.IntegrationEvents.Events;

namespace web.IntegrationEvents.EventHandler;


public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
{
    private readonly IBasketRepository _repository;
    private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

    public OrderStartedIntegrationEventHandler(
        IBasketRepository repository,
        ILogger<OrderStartedIntegrationEventHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(OrderStartedIntegrationEvent @event)
    {
        using (LogContext.PushProperty("IntegrationEventContext", $"{@event.EventId}-BasketService"))
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.EventId, "BasketService", @event);

            await _repository.DeleteBasketAsync(@event.UserId);
        }
    }
}