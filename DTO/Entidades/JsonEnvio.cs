using System.Collections.Generic;

namespace DTO.Entidades
{
    public class JsonEnvio
    {
        public class Endereco
        {
            public string rua { get; set; }
            public string numero { get; set; }
            public string complemento { get; set; }
            public string bairro { get; set; }
            public string cidade { get; set; }
            public string uf { get; set; }
            public int cep { get; set; }
        }

        public class Recebedor
        {
            public string cnpj { get; set; }
            public string nome { get; set; }
            public string local { get; set; }
            public string complemento { get; set; }
            public string bairro { get; set; }
            public int cep { get; set; }
        }

        public class Nf
        {
            public string cnpjPagador { get; set; }
            public string condicaoFrete { get; set; }
            public int numero { get; set; }
            public string serie { get; set; }
            public string dataEmissao { get; set; }
            public int qtdeVolumes { get; set; }
            public decimal valorMercadoria { get; set; }
            public decimal pesoReal { get; set; }
            public string pedido { get; set; }
            public string centroCusto2 { get; set; }
            public string chaveNFe { get; set; }
        }

        public class Destinatario
        {
            public string nome { get; set; }
            public string cnpj { get; set; }
            public string telefone { get; set; }
            public string email { get; set; }
            public Endereco endereco { get; set; }
            public string cnpjExpedidor { get; set; }
            public Recebedor recebedor { get; set; }
            public List<Nf> nf { get; set; }
        }

        public class Dados
        {
            public string cnpj { get; set; }
            public List<Destinatario> destinatario { get; set; }
        }

        public class Root
        {
            public List<Dados> dados { get; set; }
        }
    }
}
