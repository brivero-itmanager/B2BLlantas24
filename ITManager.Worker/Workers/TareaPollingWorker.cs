using ITManager.Domain.Entities;
using ITManager.Domain.Interfaces;

namespace ITManager.Worker.Workers;

public sealed class TareaPollingWorker : BackgroundService
{
    private readonly ILogger<TareaPollingWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly int _pollingIntervalSeconds;
    private readonly int _batchSize;

    public TareaPollingWorker(
        ILogger<TareaPollingWorker> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _pollingIntervalSeconds = configuration.GetValue("Worker:PollingIntervalSeconds", 10);
        _batchSize = configuration.GetValue("Worker:BatchSize", 50);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "TareaPollingWorker iniciado — intervalo {PollingIntervalSeconds}s, batch {BatchSize}",
            _pollingIntervalSeconds,
            _batchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ITareaRepository>();

                var tareas = await repository.GetPendingBatchAsync(_batchSize);
                if (!tareas.Any())
                {
                    _logger.LogDebug("Sin tareas pending");
                }
                else
                {
                    ProcesarBatch(tareas, out var candidatas, out var superseded);

                    var todasAfectadas = superseded.Concat(candidatas);
                    await repository.UpdateRangeAsync(todasAfectadas);
                    _logger.LogInformation(
                        "Ciclo completado — {Candidatas} in_progress, {Superseded} superseded",
                        candidatas.Count,
                        superseded.Count);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el ciclo de polling de tareas");
            }

            await Task.Delay(TimeSpan.FromSeconds(_pollingIntervalSeconds), stoppingToken);
        }
    }

    private void ProcesarBatch(
        List<Tarea> tareas,
        out List<Tarea> candidatas,
        out List<Tarea> superseded)
    {
        _logger.LogInformation("Batch leído: {Count} tareas pending", tareas.Count);

        var grupos = tareas
            .Where(t => !string.IsNullOrEmpty(t.DeduplicationKey))
            .GroupBy(t => t.DeduplicationKey);

        superseded = new List<Tarea>();
        candidatas = new List<Tarea>();

        foreach (var grupo in grupos)
        {
            var ordenadas = grupo.OrderByDescending(t => t.CreatedAt).ToList();
            candidatas.Add(ordenadas.First());
            superseded.AddRange(ordenadas.Skip(1));
        }

        candidatas.AddRange(tareas.Where(t => string.IsNullOrEmpty(t.DeduplicationKey)));

        foreach (var tarea in superseded)
        {
            tarea.MarcarComoSuperseded();
            _logger.LogInformation(
                "Tarea {Id} marcada como superseded por {Key}",
                tarea.Id,
                tarea.DeduplicationKey);
        }

        foreach (var tarea in candidatas)
        {
            tarea.MarcarComoEnProceso();
            _logger.LogInformation(
                "Tarea {Id} marcada como in_progress — {NombreTarea}",
                tarea.Id,
                tarea.NombreTarea);
        }
    }
}
