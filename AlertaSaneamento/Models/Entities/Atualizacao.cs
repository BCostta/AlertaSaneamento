namespace AlertaSaneamento.Models.Entities
{
    public class Atualizacao
    {
        public Guid Id { get; set; }
        public string Mensagem { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // Alerta ao qual esta atualização pertence
        public Guid AlertaId { get; set; }
        public Alerta Alerta { get; set; }

        // Fiscal que enviou a atualização
        public Guid FiscalId { get; set; }
        public Usuario Fiscal { get; set; }
    }
}
