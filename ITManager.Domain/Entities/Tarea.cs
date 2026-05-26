namespace ITManager.Domain.Entities
{
    public class Tarea
    {
        private Tarea()
        {
        }

        public Tarea(string uen, string nombreTarea, string json)
        {
            Uen = uen;
            NombreTarea = nombreTarea;
            Json = json;
            CreatedAt = DateTime.UtcNow;
            Status = "pending";
            Attempts = 0;
        }

        public long Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Uen { get; private set; }
        public string NombreTarea { get; private set; }
        public string Json { get; private set; }
        public string Status { get; private set; }
        public int Attempts { get; private set; }
        public string? LastError { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        public void MarcarComoEnviada()
        {
            Status = "sent";
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarcarComoFallida(string error)
        {
            Status = "failed";
            LastError = error;
            ProcessedAt = DateTime.UtcNow;
        }

        public void IncrementarIntento()
        {
            Attempts++;
        }
    }
}
