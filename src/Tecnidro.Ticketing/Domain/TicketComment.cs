using Tecnidro.Ticketing.Data;

namespace Tecnidro.Ticketing.Domain;

public class TicketComment
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public Ticket? Ticket { get; set; }

    public string? UserId { get; set; }

    public ApplicationUser? User { get; set; }

    public string Texto { get; set; } = string.Empty;

    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;
}
