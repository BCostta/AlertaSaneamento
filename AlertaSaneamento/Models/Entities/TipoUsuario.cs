namespace AlertaSaneamento.Models.Entities
{
    public enum TipoUsuario
    {
        Cidadao,    // pode criar e ver apenas seus próprios relatos
        Fiscal      // visualiza todos os relatos e envia atualizações de andamento
    }
}
