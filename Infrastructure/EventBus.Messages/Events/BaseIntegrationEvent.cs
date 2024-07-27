﻿namespace EventBus.Messages.Events;

public class BaseIntegrationEvent
{
    public string CorrelationId { get; set; }
    public DateTime CreationDate { get; private set; }

    public BaseIntegrationEvent()
    {
        CorrelationId = Guid.NewGuid().ToString();
        CreationDate = DateTime.UtcNow;
    }

    public BaseIntegrationEvent(Guid id, DateTime createDate)
    {
        CorrelationId = id.ToString();
        CreationDate = createDate;
    }
}
