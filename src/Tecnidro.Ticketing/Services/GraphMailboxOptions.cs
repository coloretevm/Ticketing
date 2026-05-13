namespace Tecnidro.Ticketing.Services;

public class GraphMailboxOptions
{
    public const string SectionName = "GraphMailbox";

    public string TenantId { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string MailboxAddress { get; set; } = string.Empty;
}
