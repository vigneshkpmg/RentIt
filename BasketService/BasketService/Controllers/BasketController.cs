using System.Net;
using System.Security.Claims;
using Kernal.Events;
using Microsoft.AspNetCore.Mvc;
using web.Infrastructure.Repository;
using web.IntegrationEvents.Events;
using web.Model;
using web.Services;

namespace web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType((int)HttpStatusCode.InternalServerError)]
public class BasketController : ControllerBase
{
    private readonly ILogger<BasketController> _logger;
    private readonly IBasketRepository _basketRepository;
    private readonly IIdentityServices _identityServices;
    private readonly IIntegrationEventHandler<ProductPriceChangedIntegrationEvent> _integrationEventHandler;

    public BasketController(ILogger<BasketController> logger, IBasketRepository basketRepository, IIdentityServices identityServices, IIntegrationEventHandler<ProductPriceChangedIntegrationEvent> integrationEventHandler)
    {
        _logger = logger;
        _basketRepository = basketRepository;
        _identityServices = identityServices;
        _integrationEventHandler = integrationEventHandler;
    }
    // GET
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
    {
        _logger.LogInformation("testing from get call");
        var basket = await _basketRepository.GetBasketAsync(id);
        return Ok(basket ?? new CustomerBasket(id));
    }
    
    //put
    [HttpPut]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasket customerBasket)
    {
        var basket = await _basketRepository.UpdateBasketAsync(customerBasket);
        return Ok(basket);
    }
    
    
    //put
    [HttpPatch]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateProductPriceInBasket([FromBody] ProductPriceChangedIntegrationEvent @event)
    {
         await _integrationEventHandler.Handle(@event);
        return Ok();
    }
    //post
    [HttpPost("checkout")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout)
    { 
        _logger.LogInformation("testing from checkout");
        var userId = _identityServices.GetUserIdentity();
        
        var basket = await _basketRepository.GetBasketAsync(userId);

        if (basket == null)
        {
            return BadRequest();
        }

        var userName = this.HttpContext.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

        var eventMessage = new UserCheckoutAcceptedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street,
            basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName,
            basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basketCheckout.RequestId, basket);

        // Once basket is checkout, sends an integration event to
        // ordering.api to convert basket to order and proceeds with
        // order creation process
        try
        {
            //_eventBus.Publish(eventMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration event: {IntegrationEventId} from {AppName}", eventMessage.EventId, "BasketService");

            throw;
        }

        return Accepted();
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeleteBasketById(string id)
    {
        await _basketRepository.DeleteBasketAsync(id);
    }
}