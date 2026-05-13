using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Tecnidro.Ticketing.Data;
using Tecnidro.Ticketing.Domain;

namespace Tecnidro.Ticketing.Services;

public class OutlookMailboxService(
    ApplicationDbContext dbContext,
    IOptions<GraphMailboxOptions> options)
{
    private static readonly string[] GraphScopes = ["https://graph.microsoft.com/.default"];

    public bool IsConfigured => options.Value.IsConfigured;

    public async Task<MailboxSyncResult> SyncUnreadInboxAsync(CancellationToken cancellationToken = default)
    {
        var mailboxOptions = options.Value;

        if (!mailboxOptions.IsConfigured)
        {
            return new MailboxSyncResult(
                0,
                0,
                "Falta configurar GraphMailbox: TenantId, ClientId, ClientSecret y MailboxAddress.");
        }

        var graph = CreateGraphClient(mailboxOptions);
        var messages = await graph.Users[mailboxOptions.MailboxAddress]
            .MailFolders["Inbox"]
            .Messages
            .GetAsync(request =>
            {
                request.QueryParameters.Top = mailboxOptions.MaxMessagesPerSync;
                request.QueryParameters.Filter = "isRead eq false";
                request.QueryParameters.Select =
                [
                    "id",
                    "internetMessageId",
                    "subject",
                    "bodyPreview",
                    "from",
                    "receivedDateTime",
                    "importance"
                ];
                request.QueryParameters.Orderby = ["receivedDateTime desc"];
            }, cancellationToken);

        var created = 0;
        var skipped = 0;
        var year = DateTimeOffset.Now.Year;
        var ticketPrefix = $"TK-{year}-";
        var nextTicketSequence = await dbContext.Tickets
            .CountAsync(ticket => ticket.Numero.StartsWith(ticketPrefix), cancellationToken) + 1;

        foreach (var message in messages?.Value ?? [])
        {
            var messageKey = GetMessageKey(message);
            var alreadyExists = await dbContext.Tickets
                .AnyAsync(ticket => ticket.MessageIdOutlook == messageKey, cancellationToken);

            if (alreadyExists)
            {
                skipped++;
                continue;
            }

            var senderAddress = message.From?.EmailAddress?.Address ?? string.Empty;
            var senderName = message.From?.EmailAddress?.Name ?? senderAddress;

            var ticket = new Ticket
            {
                Numero = $"{ticketPrefix}{nextTicketSequence++:0000}",
                Asunto = string.IsNullOrWhiteSpace(message.Subject) ? "(Sin asunto)" : message.Subject,
                CuerpoOriginal = message.BodyPreview ?? string.Empty,
                EmailCliente = senderAddress,
                NombreCliente = senderName,
                Estado = TicketStatus.Nuevo,
                Prioridad = MapPriority(message.Importance),
                CreadoEn = message.ReceivedDateTime ?? DateTimeOffset.Now,
                MessageIdOutlook = messageKey,
                EmailLogs =
                [
                    new EmailLog
                    {
                        Tipo = EmailLogType.Entrante,
                        AsuntoEnviado = message.Subject ?? "(Sin asunto)",
                        Destinatario = mailboxOptions.MailboxAddress,
                        CreadoEn = DateTimeOffset.Now,
                        Estado = "Importado desde Outlook"
                    }
                ]
            };

            dbContext.Tickets.Add(ticket);
            created++;

            if (mailboxOptions.MarkMessagesAsRead && !string.IsNullOrWhiteSpace(message.Id))
            {
                await graph.Users[mailboxOptions.MailboxAddress]
                    .Messages[message.Id]
                    .PatchAsync(new Message { IsRead = true }, cancellationToken: cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new MailboxSyncResult(
            created,
            skipped,
            $"Sincronizacion completada. Creados: {created}. Omitidos: {skipped}.");
    }

    private static GraphServiceClient CreateGraphClient(GraphMailboxOptions mailboxOptions)
    {
        var credential = new ClientSecretCredential(
            mailboxOptions.TenantId,
            mailboxOptions.ClientId,
            mailboxOptions.ClientSecret);

        return new GraphServiceClient(credential, GraphScopes);
    }

    private static string GetMessageKey(Message message)
    {
        return !string.IsNullOrWhiteSpace(message.InternetMessageId)
            ? message.InternetMessageId
            : message.Id ?? Guid.NewGuid().ToString("N");
    }

    private static TicketPriority MapPriority(Importance? importance)
    {
        return importance switch
        {
            Importance.High => TicketPriority.Alta,
            Importance.Low => TicketPriority.Baja,
            _ => TicketPriority.Normal
        };
    }
}
