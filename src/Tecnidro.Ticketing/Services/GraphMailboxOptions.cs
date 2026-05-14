namespace Tecnidro.Ticketing.Services;

public class GraphMailboxOptions
{
    public const string SectionName = "GraphMailbox";

    public string TenantId { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string MailboxAddress { get; set; } = string.Empty;

    public int MaxMessagesPerSync { get; set; } = 20;

    public int SyncIntervalSeconds { get; set; } = 10;

    public bool MarkMessagesAsRead { get; set; } = true;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(TenantId) &&
        !string.IsNullOrWhiteSpace(ClientId) &&
        !string.IsNullOrWhiteSpace(ClientSecret) &&
        !string.IsNullOrWhiteSpace(MailboxAddress);
}
