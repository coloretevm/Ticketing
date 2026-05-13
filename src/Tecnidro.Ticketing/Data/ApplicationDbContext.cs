using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tecnidro.Ticketing.Domain;

namespace Tecnidro.Ticketing.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();

    public DbSet<TicketComment> TicketComentarios => Set<TicketComment>();

    public DbSet<TicketAttachment> TicketAdjuntos => Set<TicketAttachment>();

    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Ticket>()
            .HasIndex(ticket => ticket.Numero)
            .IsUnique();

        builder.Entity<Ticket>()
            .HasMany(ticket => ticket.Comentarios)
            .WithOne(comment => comment.Ticket)
            .HasForeignKey(comment => comment.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Ticket>()
            .HasMany(ticket => ticket.Adjuntos)
            .WithOne(attachment => attachment.Ticket)
            .HasForeignKey(attachment => attachment.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Ticket>()
            .HasMany(ticket => ticket.EmailLogs)
            .WithOne(email => email.Ticket)
            .HasForeignKey(email => email.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
