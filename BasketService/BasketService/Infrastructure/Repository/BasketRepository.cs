using System.Text.Json;
using StackExchange.Redis;
using web.Model;

namespace web.Infrastructure.Repository;

public class BasketRepository:IBasketRepository
{
    private readonly ILogger<BasketRepository> _logger;

    private readonly ConnectionMultiplexer _connectionMultiplexer;

    private readonly IDatabase _database;

    public BasketRepository(ILogger<BasketRepository> logger, ConnectionMultiplexer connectionMultiplexer)
    {
        _logger = logger;
        _connectionMultiplexer = connectionMultiplexer;
        _database = _connectionMultiplexer.GetDatabase();
    }

    public async Task<CustomerBasket?> GetBasketAsync(string customerId)
    {
        if (string.IsNullOrEmpty(customerId))
            return null;
        
        var data = await _database.StringGetAsync(customerId);
        
        if (data.IsNullOrEmpty || !data.HasValue)
        {
            return null;
        }
        
        return JsonSerializer.Deserialize<CustomerBasket>(data, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public IEnumerable<string> GetUsers()
    {
        var server = GetServer();
        var data = server.Keys();

        return data.Select(k => k.ToString());
    }

    public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
    {
        var updated = await _database.StringSetAsync(basket.BuyerId, JsonSerializer.Serialize(basket));
        if (!updated)
        {
            _logger.LogInformation("Problem occur persisting the item");
            return null;
        }   
        
        _logger.LogInformation("Basket item persisted successfully");

        return await GetBasketAsync(basket.BuyerId);
    }

    public async Task<bool> DeleteBasketAsync(string id)
    {
        return await _database.KeyDeleteAsync(id);
    }
    
    
    private  IServer GetServer()
    {
        var endpoint = _connectionMultiplexer.GetEndPoints();
        return _connectionMultiplexer.GetServer(endpoint.First());
    }
}