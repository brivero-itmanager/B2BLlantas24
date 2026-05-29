namespace ITManager.Domain.Entities
{
    public class Tarea
    {
        private readonly List<TareaLog> _logs = new();

        private Tarea()
        {
        }

        public Tarea(string uen, string nombreTarea, string json, string taskType = "outbound")
        {
            Uen = uen;
            NombreTarea = nombreTarea;
            Json = json;
            TaskType = ValidarTaskType(taskType);
            CreatedAt = DateTime.Now;
            Status = "pending";
            Attempts = 0;
        }

        public long Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string Uen { get; private set; }
        public string NombreTarea { get; private set; }
        public string Json { get; private set; }
        public string Status { get; private set; }
        public int Attempts { get; private set; }
        public string? LastError { get; private set; }
        public DateTime? ProcessedAt { get; private set; }
        public string? WooCommerceResponse { get; private set; }
        public string TaskType { get; private set; }
        public string? DeduplicationKey { get; private set; }
        public IReadOnlyList<TareaLog> Logs => _logs.AsReadOnly();

        public void MarcarComoEnviada()
        {
            Status = "sent";
            ProcessedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            AgregarLog("ENVIADA", null);
        }

        public void MarcarComoFallida(string error)
        {
            Status = "failed";
            LastError = error;
            ProcessedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            AgregarLog("FALLIDA", error);
        }

        public void IncrementarIntento()
        {
            Attempts++;
            AgregarLog("INTENTO", $"Intento #{Attempts}");
        }

        public void MarcarComoEnProceso()
        {
            Status = "in_progress";
            UpdatedAt = DateTime.Now;
            AgregarLog("EN_PROCESO", null);
        }

        public void MarcarComoSuperseded()
        {
            Status = "superseded";
            UpdatedAt = DateTime.Now;
            AgregarLog("SUPERSEDED", null);
        }

        public void RegistrarRespuestaWooCommerce(string response)
        {
            WooCommerceResponse = response;
            AgregarLog("RESPUESTA_WOOCOMMERCE", response);
        }

        public void AsignarDeduplicationKey(string key)
        {
            DeduplicationKey = key;
            AgregarLog("DEDUPLICATION_KEY_ASIGNADA", key);
        }

        private void AgregarLog(string evento, string? detalle)
        {
            _logs.Add(new TareaLog(Id, evento, detalle));
        }

        private static string ValidarTaskType(string taskType)
        {
            if (taskType != "outbound" && taskType != "inbound")
            {
                throw new ArgumentException("TaskType debe ser 'outbound' o 'inbound'.", nameof(taskType));
            }

            return taskType;
        }
    }
}
