namespace BuildingBlock.Exceptions;
public class InvalidServerEcxeption : Exception
{
    public InvalidServerEcxeption(string message) : base(message)
    {
    }
    public InvalidServerEcxeption(string message, string details)
        : base(message)
    {
        Details = details;
    }
    public string? Details { get; set; }
}
