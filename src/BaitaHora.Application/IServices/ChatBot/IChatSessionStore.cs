namespace BaitaHora.Application.IServices.IChatbot
{
    public interface IChatSessionStore
    {
        Task<ChatSession?> GetAsync(string key, CancellationToken ct = default);
        Task SetAsync(ChatSession session, CancellationToken ct = default);
        Task ClearAsync(string key, CancellationToken ct = default);
    }

    public sealed class ChatSession
    {
        public string Key { get; init; } = "";
        public string Stage { get; set; } = "start";
        public Guid CompanyId { get; init; }
        public string PhoneE164 { get; init; } = "";
        public Guid? CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? RoleName { get; set; }
        public Guid? ProfessionalUserId { get; set; }
        public Guid? ServiceId { get; set; }
        public DateTime? WeekStartUtc { get; set; }
    }
}