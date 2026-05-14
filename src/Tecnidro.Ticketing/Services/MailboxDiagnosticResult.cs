namespace Tecnidro.Ticketing.Services;

public sealed record MailboxDiagnosticResult(
    bool IsConfigured,
    string MailboxAddress,
    IReadOnlyList<MailboxFolderDiagnostic> Folders,
    string? FoldersError,
    IReadOnlyList<MailboxMessageDiagnostic> InboxMessages,
    string? InboxMessagesError);

public sealed record MailboxFolderDiagnostic(
    string? Id,
    string? DisplayName,
    int? TotalItemCount,
    int? UnreadItemCount);

public sealed record MailboxMessageDiagnostic(
    string? Id,
    string? Subject,
    string? From,
    DateTimeOffset? ReceivedDateTime,
    bool? IsRead);
