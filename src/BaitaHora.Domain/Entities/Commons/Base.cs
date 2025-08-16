namespace BaitaHora.Domain.Entities.Commons
{
    public abstract class Base
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }

        public void Touch() => UpdatedAtUtc = DateTime.UtcNow;

        public void MarkCreated() => CreatedAtUtc = DateTime.UtcNow;
    }
}