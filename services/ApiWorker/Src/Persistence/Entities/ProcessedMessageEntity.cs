namespace ApiWorker.Persistence.Entities;

public sealed class ProcessedMessageEntity
{
    public int Id { get; set; }

    public string MessageId { get; set; } = string.Empty;
}
