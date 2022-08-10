using System;
using System.Collections.Generic;
using DTO.Entidades;

namespace DAL
{
    public class Query : Conexao
    {
        private string tipointegracao = System.Configuration.ConfigurationManager.AppSettings["tipointegracao"];

        public Integracao ConsultaDadosIntegracao(int clienteid)
        {
            var retorno = new Integracao();
            try
            {
                string sqlQuery = $@"select * from Integracoes WITH(NOLOCK) where cliente = {clienteid} and tipointegracao = '{tipointegracao}';";
                retorno = ExecutaSelect<Integracao>(sqlQuery);
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
                LogErro(clienteid, erro, "ConsultaDadosIntegracao");
            }

            return retorno;
        }

        public Cliente ConsultaDadosCliente(int clienteid)
        {
            var retorno = new Cliente();
            try
            {
                string sqlQuery = $@"select Cnpj from Cliente WITH(NOLOCK) where ClienteId = '{clienteid}';";
                retorno = ExecutaSelect<Cliente>(sqlQuery);
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
                LogErro(clienteid, erro, "ConsultaCnpj");
            }

            return retorno;
        }

        public List<Pedidos> GetPedidos(int clienteid)
        {
            var retorno = new List<Pedidos>();

            try
            {
                string sqlQuery = $@"select top 10
                                           cdest.Nome, cdest.Telefone, cdest.Bairro, cdest.CEP, 
                                           cdest.Municipio, cdest.Endereco, cdest.Numero, cdest.Email, cdest.Complemento, 
                                           cdest.UF, cdest.NumeroDocumento, cdest.CodigoExterno, 
                                           pn.NumeroNf, pn.SerieNf, pn.ChaveNf, 
                                           pn.DataEmissaoNf, p.TotalSkus, pn.PesoNf,
                                           pn.ValorNF, p.NumeroDcto, pn.PedidoId, p.InfAdicional4
                                           From 
                                           Gaiola g
                                           join GaiolaPedido gp on g.gaiolaid = gp.GaiolaId 
                                           join pedido p on gp.pedidoid = p.pedidoid
                                           join ClienteDestinatario cdest on 
                                           p.DestinatorioEntregaId = cdest.ClienteDestinatarioId
                                           join pedidonota pn on p.pedidoid = pn.pedidoid
                                           join cliente cli on p.clienteid = cli.clienteid
                                           join Transportadora tr on tr.TransportadoraId = g.TransportadoraId
                                           where 
                                           tr.TransportadoraId = 2333 and g.clienteid = {clienteid} and g.status = 60 and pn.PesoNF > 0 and
                                           pn.PesoNF < 99999.999 and gp.IntegrouEdiTransportadora = 0";

                retorno = ExecutaSelectLista<Pedidos>(sqlQuery);

                return retorno;
            }

            catch (Exception ex)
            {
                var erro = ex.Message;
                LogErro(clienteid, erro, "GetPedidos");
            }

            return retorno;
        }
        public List<ClientesIntegrados> ConsultaClienteIntegrado()
        {
            var retorno = new List<ClientesIntegrados>();

            try
            {
                string sqlQuery = $@"select cliente from Integracoes WITH(NOLOCK) WHERE tipointegracao = '{tipointegracao}'";
                retorno = ExecutaSelectLista<ClientesIntegrados>(sqlQuery);
                return retorno;
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
            }

            return retorno;
        }

        public void IntegraEDI(int pedidoid)
        {
            try
            {
                var sqlQuery = $@"UPDATE GaiolaPedido SET IntegrouEdiTransportadora = 1 WHERE PedidoId = {pedidoid};";
                ExecutaComando(sqlQuery);
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
            }
        }

        public void RetornoJson(int pedidoid, string enviojson)
        {
            try
            {
                var sqlQuery = $@"UPDATE Pedido SET RetornoTMS = '{enviojson}' WHERE PedidoId = {pedidoid};";
                ExecutaComando(sqlQuery);
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
            }
        }

        public void ProximoEnvio(int clienteid)
        {
            try
            {
                var dt = DateTime.Now.AddMinutes(10);
                var data = dt.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var sqlQuery = $@"UPDATE Integracoes SET proximoenvio = '{data}'
                                WHERE cliente = {clienteid} AND tipointegracao = '{tipointegracao}'";
                ExecutaComando(sqlQuery);
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }
        public void LogErro(int clienteid, string erro, string metodo)
        {
            var data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            erro = erro.Replace("'", "");

            try
            {
                var sqlQuery = $@"INSERT INTO logerro (clienteid,data,erro,origem,metodo)
                                  VALUES ({clienteid}, '{data}', '{erro}', 'IntegracaoSSW', '{metodo}')";
                ExecutaComando(sqlQuery);
            }

            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }
    }
}
