namespace ITManager.Domain.Entities
{
    public class Tarea
    {
        private Tarea()
        {
        }

        public Tarea(string uen, string nombreTarea, string json, string taskType = "outbound")
        {
            Uen = uen;
            NombreTarea = nombreTarea;
            Json = json;
            TaskType = ValidarTaskType(taskType);
            CreatedAt = DateTime.UtcNow;
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

        public void MarcarComoEnviada()
        {
            Status = "sent";
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarcarComoFallida(string error)
        {
            Status = "failed";
            LastError = error;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementarIntento()
        {
            Attempts++;
        }

        public void MarcarComoEnProceso()
        {
            Status = "in_progress";
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarcarComoSuperseded()
        {
            Status = "superseded";
            UpdatedAt = DateTime.UtcNow;
        }

        public void RegistrarRespuestaWooCommerce(string response)
        {
            WooCommerceResponse = response;
        }

        public void AsignarDeduplicationKey(string key)
        {
            DeduplicationKey = key;
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
