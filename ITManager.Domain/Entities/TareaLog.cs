namespace ITManager.Domain.Entities
{
    public class TareaLog
    {
        private TareaLog()
        {
        }

        internal TareaLog(long tareaId, string evento, string? detalle)
        {
            TareaId = tareaId;
            Evento = evento;
            Detalle = detalle;
            OcurridoEn = DateTime.Now;
        }

        public long Id { get; private set; }
        public long TareaId { get; private set; }
        public string Evento { get; private set; } = string.Empty;
        public string? Detalle { get; private set; }
        public DateTime OcurridoEn { get; private set; }
    }
}
