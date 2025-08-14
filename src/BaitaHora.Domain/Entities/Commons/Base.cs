namespace BaitaHora.Domain.Entities.Commons
{
    public class Base
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
    }
}