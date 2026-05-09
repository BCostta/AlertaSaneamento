namespace AlertaSaneamento.Models.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public TipoUsuario Tipo { get; set; }

        // Relatos criados por este usuário (apenas Cidadao cria relatos)
        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();

        // Atualizações enviadas por este usuário (apenas Fiscal envia atualizações)
        public ICollection<Atualizacao> Atualizacoes { get; set; } = new List<Atualizacao>();
    }
}
