namespace BaitaHora.Application.Common.Time;

public interface IClock
{
    DateTime UtcNow { get; }
}