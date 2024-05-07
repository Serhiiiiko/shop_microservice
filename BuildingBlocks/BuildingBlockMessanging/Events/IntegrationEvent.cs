namespace BuildingBlockMessanging.Events;
public record IntegrationEvent
{
    public Guid Id => Guid.NewGuid();
    public DateTime DateTime => DateTime.Now;
    public string EventyType => GetType().AssemblyQualifiedName;
}
