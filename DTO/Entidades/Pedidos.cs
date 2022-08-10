using System;

namespace DTO.Entidades
{
    public class Pedidos
    {
        public string PedidoId { get; set; }
        public string NumeroDcto { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Bairro { get; set; }
        public string CEP { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string NumeroDocumento { get; set; }
        public string NumeroNf { get; set; }
        public string SerieNf { get; set; }
        public string ChaveNf { get; set; }
        public DateTime DataEmissaoNf { get; set; }
        public string TotalSkus { get; set; }
        public decimal ValorNF { get; set; }
        public decimal PesoNF { get; set; }
        public string InfAdicional4 { get; set; }
    }
}
