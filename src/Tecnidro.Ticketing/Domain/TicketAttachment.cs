namespace Tecnidro.Ticketing.Domain;

public class TicketAttachment
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public Ticket? Ticket { get; set; }

    public string NombreArchivo { get; set; } = string.Empty;

    public string Ruta { get; set; } = string.Empty;

    public long TamanoBytes { get; set; }
}
