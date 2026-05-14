using Microsoft.Extensions.Options;

namespace Tecnidro.Ticketing.Services;

public sealed class OutlookMailboxBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<GraphMailboxOptions> options,
    ILogger<OutlookMailboxBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(Math.Max(1, options.Value.SyncIntervalSeconds));

        logger.LogInformation(
            "Sincronizacion automatica de Outlook iniciada. Intervalo: {IntervalSeconds} segundos.",
            interval.TotalSeconds);

        await SyncOnceAsync(stoppingToken);

        using var timer = new PeriodicTimer(interval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await SyncOnceAsync(stoppingToken);
        }
    }

    private async Task SyncOnceAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var mailbox = scope.ServiceProvider.GetRequiredService<OutlookMailboxService>();

            if (!mailbox.IsConfigured)
            {
                logger.LogDebug("Outlook no configurado. Se omite la sincronizacion automatica.");
                return;
            }

            var result = await mailbox.SyncUnreadInboxAsync(cancellationToken);

            if (result.Created > 0)
            {
                logger.LogInformation(
                    "Sincronizacion Outlook completada. Tickets creados: {Created}. Omitidos: {Skipped}.",
                    result.Created,
                    result.Skipped);
            }
            else
            {
                logger.LogDebug(
                    "Sincronizacion Outlook sin tickets nuevos. Omitidos: {Skipped}.",
                    result.Skipped);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error durante la sincronizacion automatica de Outlook.");
        }
    }
}
