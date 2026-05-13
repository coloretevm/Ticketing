using Tecnidro.Ticketing.Domain;

namespace Tecnidro.Ticketing.Services;

public class TicketWorkspaceService
{
    private readonly List<Ticket> tickets =
    [
        new()
        {
            Id = 1,
            Numero = "TK-2026-0001",
            Asunto = "Pressostato linea osmosi in allarme",
            CuerpoOriginal = "Buongiorno, il pressostato della linea osmosi segnala allarme da questa mattina.",
            EmailCliente = "manutenzione@cliente-demo.it",
            NombreCliente = "Cliente Demo",
            Estado = TicketStatus.Nuevo,
            Prioridad = TicketPriority.Urgente,
            CreadoEn = DateTimeOffset.Now.AddHours(-2),
            MessageIdOutlook = "<demo-0001@outlook.office365.com>",
            EmailLogs =
            [
                new()
                {
                    Id = 1,
                    TicketId = 1,
                    Tipo = EmailLogType.Entrante,
                    AsuntoEnviado = "Pressostato linea osmosi in allarme",
                    Destinatario = "supporto@tecnidro.it",
                    CreadoEn = DateTimeOffset.Now.AddHours(-2),
                    Estado = "Ricevuto"
                }
            ]
        },
        new()
        {
            Id = 2,
            Numero = "TK-2026-0002",
            Asunto = "Richiesta ricambio pompa dosatrice",
            CuerpoOriginal = "Serve disponibilita e prezzo per una pompa dosatrice compatibile con impianto esistente.",
            EmailCliente = "acquisti@industria-demo.it",
            NombreCliente = "Industria Demo",
            Estado = TicketStatus.Asignado,
            Prioridad = TicketPriority.Alta,
            CreadoEn = DateTimeOffset.Now.AddHours(-7),
            Comentarios =
            [
                new()
                {
                    Id = 1,
                    TicketId = 2,
                    Texto = "Verificare codice ricambio e disponibilita a magazzino.",
                    CreadoEn = DateTimeOffset.Now.AddHours(-6)
                }
            ]
        },
        new()
        {
            Id = 3,
            Numero = "TK-2026-0003",
            Asunto = "Manutenzione programmata addolcitore",
            CuerpoOriginal = "Vorremmo programmare il controllo periodico dell'addolcitore entro fine mese.",
            EmailCliente = "ufficio.tecnico@cliente-demo.it",
            NombreCliente = "Ufficio Tecnico Demo",
            Estado = TicketStatus.EnProgreso,
            Prioridad = TicketPriority.Normal,
            CreadoEn = DateTimeOffset.Now.AddDays(-1)
        }
    ];

    public IReadOnlyList<Ticket> GetTickets()
    {
        return tickets
            .OrderByDescending(ticket => ticket.Prioridad)
            .ThenBy(ticket => ticket.CreadoEn)
            .ToList();
    }

    public Ticket? GetTicket(int id)
    {
        return tickets.FirstOrDefault(ticket => ticket.Id == id);
    }

    public Ticket CloseTicket(int id, string resolution)
    {
        if (string.IsNullOrWhiteSpace(resolution))
        {
            throw new ArgumentException("La resolucion es obligatoria para cerrar el ticket.", nameof(resolution));
        }

        var ticket = GetTicket(id) ?? throw new InvalidOperationException("Ticket no encontrado.");
        ticket.Estado = TicketStatus.Cerrado;
        ticket.CerradoEn = DateTimeOffset.Now;
        ticket.ResolucionTexto = resolution.Trim();
        ticket.Comentarios.Add(new TicketComment
        {
            Id = ticket.Comentarios.Count + 1,
            TicketId = ticket.Id,
            Texto = $"Cierre: {ticket.ResolucionTexto}",
            CreadoEn = DateTimeOffset.Now
        });
        ticket.EmailLogs.Add(new EmailLog
        {
            Id = ticket.EmailLogs.Count + 1,
            TicketId = ticket.Id,
            Tipo = EmailLogType.Saliente,
            AsuntoEnviado = $"Re: {ticket.Asunto}",
            Destinatario = ticket.EmailCliente,
            CreadoEn = DateTimeOffset.Now,
            Estado = "Pendiente Graph"
        });

        return ticket;
    }

    public DashboardSummary GetSummary()
    {
        return new DashboardSummary(
            tickets.Count,
            tickets.Count(ticket => ticket.Estado is TicketStatus.Nuevo or TicketStatus.Asignado),
            tickets.Count(ticket => ticket.Prioridad is TicketPriority.Alta or TicketPriority.Urgente),
            tickets.Count(ticket => ticket.Estado == TicketStatus.Cerrado));
    }
}

public sealed record DashboardSummary(int Total, int Abiertos, int Importantes, int Cerrados);
