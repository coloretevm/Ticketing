namespace Tecnidro.Ticketing.Domain;

public class EmailLog
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public Ticket? Ticket { get; set; }

    public EmailLogType Tipo { get; set; }

    public string AsuntoEnviado { get; set; } = string.Empty;

    public string Destinatario { get; set; } = string.Empty;

    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;

    public string Estado { get; set; } = string.Empty;
}
