namespace Tecnidro.Ticketing.Services;

public sealed record MailboxSyncResult(int Created, int Skipped, string Message);
