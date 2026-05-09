namespace AlertaSaneamento.Models.Entities
{
    public class Alerta
    {
        public Guid Id { get; set; }
        public string relato { get; set; }
        public byte[] imagem { get; set; }
        public string imagemTipo { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        // Localização
        public string Endereco { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Cidadão que criou o relato
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        // Atualizações enviadas pelos fiscais sobre este relato
        public ICollection<Atualizacao> Atualizacoes { get; set; } = new List<Atualizacao>();
    }
}
