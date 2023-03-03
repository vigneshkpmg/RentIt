using System.Text.Json.Serialization;

namespace Kernal.Events;

public record IntegrationEvent
{        
    public IntegrationEvent()
    {
        EventName = "eventName";
        EventId = Guid.NewGuid().ToString();
        EventDate = DateTime.UtcNow;
    }

    [JsonConstructor]
    public IntegrationEvent(string id, DateTime createDate, string eventName)
    {
        EventId = id;
        EventDate = createDate;
        EventName = eventName;
    }

    [JsonInclude]
    public string EventId { get; private init; }

    [JsonInclude]
    public DateTime EventDate { get; private init; }
    
    [JsonInclude]
    public string EventName { get; private init; }
}