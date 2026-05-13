using System.ComponentModel.DataAnnotations;
using Tecnidro.Ticketing.Data;

namespace Tecnidro.Ticketing.Domain;

public class Ticket
{
    public int Id { get; set; }

    [MaxLength(32)]
    public string Numero { get; set; } = string.Empty;

    [MaxLength(240)]
    public string Asunto { get; set; } = string.Empty;

    public string CuerpoOriginal { get; set; } = string.Empty;

    [MaxLength(320)]
    public string EmailCliente { get; set; } = string.Empty;

    [MaxLength(160)]
    public string NombreCliente { get; set; } = string.Empty;

    public TicketStatus Estado { get; set; } = TicketStatus.Nuevo;

    public TicketPriority Prioridad { get; set; } = TicketPriority.Normal;

    public string? AsignadoAUserId { get; set; }

    public ApplicationUser? AsignadoAUser { get; set; }

    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? CerradoEn { get; set; }

    public string? ResolucionTexto { get; set; }

    [MaxLength(512)]
    public string? MessageIdOutlook { get; set; }

    public List<TicketComment> Comentarios { get; set; } = [];

    public List<TicketAttachment> Adjuntos { get; set; } = [];

    public List<EmailLog> EmailLogs { get; set; } = [];
}
