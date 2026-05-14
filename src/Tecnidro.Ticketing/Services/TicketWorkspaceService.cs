using Microsoft.EntityFrameworkCore;
using Tecnidro.Ticketing.Data;
using Tecnidro.Ticketing.Domain;

namespace Tecnidro.Ticketing.Services;

public class TicketWorkspaceService(ApplicationDbContext dbContext)
{
    public async Task<IReadOnlyList<Ticket>> GetTicketsAsync(CancellationToken cancellationToken = default)
    {
        var tickets = await dbContext.Tickets
            .Include(ticket => ticket.Comentarios)
            .Include(ticket => ticket.EmailLogs)
            .ToListAsync(cancellationToken);

        return tickets
            .OrderByDescending(ticket => ticket.Prioridad)
            .ThenByDescending(ticket => ticket.CreadoEn)
            .ToList();
    }

    public async Task<Ticket?> GetTicketAsync(int id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Tickets
            .Include(ticket => ticket.Comentarios)
            .Include(ticket => ticket.EmailLogs)
            .FirstOrDefaultAsync(ticket => ticket.Id == id, cancellationToken);
    }

    public async Task<Ticket> CloseTicketAsync(int id, string resolution, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(resolution))
        {
            throw new ArgumentException("La resolucion es obligatoria para cerrar el ticket.", nameof(resolution));
        }

        var ticket = await GetTicketAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Ticket no encontrado.");

        ticket.Estado = TicketStatus.Cerrado;
        ticket.CerradoEn = DateTimeOffset.Now;
        ticket.ResolucionTexto = resolution.Trim();
        ticket.Comentarios.Add(new TicketComment
        {
            Texto = $"Cierre: {ticket.ResolucionTexto}",
            CreadoEn = DateTimeOffset.Now
        });
        ticket.EmailLogs.Add(new EmailLog
        {
            Tipo = EmailLogType.Saliente,
            AsuntoEnviado = $"Re: {ticket.Asunto}",
            Destinatario = ticket.EmailCliente,
            CreadoEn = DateTimeOffset.Now,
            Estado = "Pendiente Graph"
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return ticket;
    }

    public async Task<int> DeleteTicketsAsync(IReadOnlyCollection<int> ticketIds, CancellationToken cancellationToken = default)
    {
        if (ticketIds.Count == 0)
        {
            return 0;
        }

        var tickets = await dbContext.Tickets
            .Where(ticket => ticketIds.Contains(ticket.Id))
            .ToListAsync(cancellationToken);

        dbContext.Tickets.RemoveRange(tickets);
        await dbContext.SaveChangesAsync(cancellationToken);

        return tickets.Count;
    }

    public async Task<DashboardSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var total = await dbContext.Tickets.CountAsync(cancellationToken);
        var abiertos = await dbContext.Tickets
            .CountAsync(ticket => ticket.Estado == TicketStatus.Nuevo || ticket.Estado == TicketStatus.Asignado, cancellationToken);
        var importantes = await dbContext.Tickets
            .CountAsync(ticket => ticket.Prioridad == TicketPriority.Alta || ticket.Prioridad == TicketPriority.Urgente, cancellationToken);
        var cerrados = await dbContext.Tickets
            .CountAsync(ticket => ticket.Estado == TicketStatus.Cerrado, cancellationToken);

        return new DashboardSummary(total, abiertos, importantes, cerrados);
    }

    public async Task<string> GetNextTicketNumberAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTimeOffset.Now.Year;
        var prefix = $"TK-{year}-";
        var existingThisYear = await dbContext.Tickets
            .CountAsync(ticket => ticket.Numero.StartsWith(prefix), cancellationToken);

        return $"{prefix}{existingThisYear + 1:0000}";
    }
}

public sealed record DashboardSummary(int Total, int Abiertos, int Importantes, int Cerrados);
