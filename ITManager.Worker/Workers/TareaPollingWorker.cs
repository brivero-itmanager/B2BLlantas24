namespace ITManager.Worker.Workers;

public sealed class TareaPollingWorker : BackgroundService
{
    private readonly ILogger<TareaPollingWorker> _logger;

    public TareaPollingWorker(ILogger<TareaPollingWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TareaPollingWorker iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("TareaPollingWorker — ciclo de polling {Time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
