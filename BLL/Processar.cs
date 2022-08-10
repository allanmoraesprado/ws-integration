using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using DAL;
using DTO.Entidades;
using static DTO.Entidades.JsonEnvio;

namespace BLL
{
    public class Processar
    {
        public class PedidoEnvio
        {
            public string pedido { get; set; }
            public string enviojson { get; set; }
            public string retornojson { get; set; }
        }
        public string IntegraSSW()
        {
            try
            {
                var cli = new Query().ConsultaClienteIntegrado();
                if (cli.Count > 0)
                {
                    foreach (var clienteIntegrado in cli)
                    {
                        var dados = new Query().ConsultaDadosIntegracao(clienteIntegrado.cliente);
                        if (dados.cliente > 0)
                        {
                            new Query().ProximoEnvio(dados.cliente);
                            var pedidos = ConsultaPedidos(dados.cliente);
                            if (pedidos.Count > 0)
                            {
                                var cliente = new Query().ConsultaDadosCliente(dados.cliente);
                                var token = GerarToken(dados, cliente);
                                if (!String.IsNullOrEmpty(token))
                                {
                                    var jsonpedido = MontaJson(pedidos, cliente, dados);
                                    var envio = EnviaSSW(token, jsonpedido, dados);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
            }
            return "";
        }

        public List<Pedidos> ConsultaPedidos(int clienteid)
        {
            var retorno = new List<Pedidos>();
            try
            {
                retorno = new Query().GetPedidos(clienteid);
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
                var envialog = new Query();
                envialog.LogErro(clienteid, erro, "ConsultaPedidos");
            }

            return retorno;
        }

        public string GerarToken(Integracao dados, Cliente cliente)
        {
            var ret = "";
            try
            {
                string sJson = null;
                var gettoken = new GetToken()
                {
                    cnpj_edi = cliente.Cnpj,
                    domain = dados.informacaoadicional1,
                    password = dados.senha,
                    username = dados.usuario
                };
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        sJson = JsonHelper.Serializar(gettoken);
                        var baseAddress = dados.webserviceadicional1;
                        var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                        http.Accept = "application/json";
                        http.ContentType = "application/json";
                        http.Method = "POST";
                        string parsedContent = sJson;
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        Byte[] bytes = encoding.GetBytes(parsedContent);
                        Stream newStream = http.GetRequestStream();
                        newStream.Write(bytes, 0, bytes.Length);
                        newStream.Close();

                        var response = http.GetResponse();
                        var stream = response.GetResponseStream();
                        var sr = new StreamReader(stream);
                        var content = sr.ReadToEnd();
                        var retorno = JsonHelper.DeSerializar<RetornoToken>(content);
                        ret = retorno.token;
                    }
                    catch (Exception ex)
                    {
                        var erro = ex.Message;
                        var envialog = new Query();
                        envialog.LogErro(dados.cliente, erro, "GerarToken");
                    }
                }
            }
            catch (Exception ex)
            {
                var erro = ex.Message;
                var envialog = new Query();
                envialog.LogErro(dados.cliente, erro, "GerarToken");
            }
            return ret;
        }

        public List<PedidoEnvio> MontaJson(List<Pedidos> pedidos, Cliente cliente, Integracao dados)
        {
            var sJson = "";
            var lista = new List<PedidoEnvio>();
            try
            {
                foreach (var item in pedidos)
                {
                    var json = new Root()
                    {
                        dados = new List<Dados>()
                    {
                        new Dados()
                        {
                            cnpj = cliente.Cnpj,
                            destinatario = new List<Destinatario>()
                            {
                                new Destinatario()
                                {
                                    nome            = item.Nome,
                                    cnpj            = item.NumeroDocumento.Replace("/", ""),
                                    telefone        = item.Telefone ?? "",
                                    email           = item.Email,
                                    endereco = new Endereco
                                    {
                                        rua      = item.Endereco,
                                        numero   = item.Numero,
                                        complemento     = item.Complemento,
                                        bairro   = item.Bairro,
                                        cidade   = item.Municipio,
                                        uf       = item.UF,
                                        cep      = Convert.ToInt32(item.CEP.Replace("-",""))
                                    },

                                    cnpjExpedidor   =  cliente.Cnpj,

                                    recebedor = new Recebedor
                                    {
                                        bairro  = item.Bairro,
                                        cep     = Convert.ToInt32(item.CEP.Replace("-","")),
                                        cnpj    = item.NumeroDocumento.Replace("/", ""),
                                        local   = $"{item.Endereco}, {item.Numero}",
                                        complemento     = item.Complemento,
                                        nome    = item.Nome,
                                    },

                                    nf = new List<Nf>()
                                    {
                                        new Nf
                                        {
                                            chaveNFe            = ValidaChaveNf(item.ChaveNf),
                                            cnpjPagador         = cliente.Cnpj,
                                            condicaoFrete       = "CIF",
                                            dataEmissao         = item.DataEmissaoNf.ToString("yyyy-MM-dd"),
                                            numero              = Convert.ToInt32(item.NumeroNf),
                                            pesoReal            = Convert.ToDecimal(item.PesoNF),
                                            pedido              = item.NumeroDcto,
                                            centroCusto2        = item.InfAdicional4,
                                            qtdeVolumes         = Convert.ToInt32(item.TotalSkus),
                                            serie               = item.SerieNf,
                                            valorMercadoria     = item.ValorNF,
                                        }
                                    }
                                }
                            }
                        }
                    }

                    };

                    sJson = JsonHelper.Serializar(json);
                    var ped = new PedidoEnvio()
                    {
                        enviojson = sJson,
                        pedido = item.PedidoId,
                    };

                    lista.Add(ped);
                }
            }

            catch (Exception ex)
            {
                var erro = ex.Message;
                var envialog = new Query();
                envialog.LogErro(dados.cliente, erro, "MontaJson");
            }

            return lista;
        }

        public List<PedidoEnvio> EnviaSSW(string token, List<PedidoEnvio> pedidos, Integracao dados)
        {
            var ret = new List<PedidoEnvio>();
            try
            {
                foreach (var item in pedidos)
                {
                    var client = new RestClient(dados.webservice);
                    client.Timeout = -1;
                    var met = Method.POST;

                    var request = new RestRequest(met);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddHeader("Authorization", token);

                    if (!string.IsNullOrWhiteSpace(item.enviojson))                   
                        request.AddParameter("application/json", item.enviojson, ParameterType.RequestBody);

                    var response = client.Execute(request);
                    var jsonRetorno = response.Content;
                    jsonRetorno = jsonRetorno.Replace("[", "").Replace("]", "");
                    item.retornojson = jsonRetorno;

                    if (!String.IsNullOrEmpty(response.Content))
                    {
                        if (response.StatusCode == HttpStatusCode.OK && jsonRetorno.Contains("inserida com sucesso"))
                        {
                            var update = new Query();
                            update.IntegraEDI(Convert.ToInt32(item.pedido));
                            update.RetornoJson(Convert.ToInt32(item.pedido), item.retornojson);
                        }
                        ret.Add(item);
                    }
                }
            }
            catch (WebException ex)
            {
                var erro = ex.Message;
                var envialog = new Query();
                envialog.LogErro(dados.cliente, erro, "EnviaSSW");
            }

            return ret;
        }

        public class JsonHelper
        {
            public static string Serializar<T>(T obj)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream();
                serializer.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            public static T DeSerializar<T>(string json)
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
                return (T)ser.ReadObject(ms);
            }
        }

        public string ValidaChaveNf(string chave)
        {
            chave = chave.Replace("NFe", "");
            return chave;
        }
    }
}