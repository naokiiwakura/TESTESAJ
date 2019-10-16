using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using BoletoNet;
using BoletoNet.RelatorioValoresBoleto;
using CFC.Models;

namespace CFC.Uteis
{
    public class GerarBoleto : Controller
    {        

        public static BoletoBancario Boleto(int id)
        {
            var db = new cfcadminEntities();
            Thread.Sleep(100);
            var tit = db.Movimentacao.Find(id);
            var emp = db.Configuracoes.FirstOrDefault();
            //var conta = tit.TaxaAdmin ? db.ContaCorrente.Find(emp.CodigoBancoAdministrativo) : db.ContaCorrente.Find(emp.CodigoBancoGrupo);
            var byt = "2";
            var posto = "03";


            Cedente c = new Cedente(emp.CnpjDaEmpresa, emp.RazaoSocialDaEmpresa, emp.NumeroDaAgenciaBancaria, emp.DigitoDaAgenciaBancaria, emp.NumeroDaContaBancaria, emp.DigitoDaContaBancaria);
            c.Codigo = emp.NumeroDaAgenciaBancaria + posto + emp.NumeroDaContaBancaria;
            c.ContaBancaria.OperacaConta = posto;

            //nosso numero
            string nossoNumero = string.Concat(byt, tit.CodigoDoTitulo.ToString("D5"));
            var carteira = "1";

            //boleto
            Boleto b = new Boleto(Convert.ToDateTime(tit.DataDoVencimento), Convert.ToDecimal(tit.ValorDoTitulo), carteira, nossoNumero, c);
            b.NumeroDocumento = tit.CodigoDoTitulo.ToString("D10");
            b.DataProcessamento = tit.DataDoTitulo;
            b.DataDocumento = tit.DataDoTitulo;
            b.PercMulta = Convert.ToDecimal("2,00");
            b.PercJurosMora = Convert.ToDecimal("0,033");

            //cliente
            var client = db.Clientes.Find(tit.CodigoDoCliente);
            b.Sacado = new Sacado(client.CPFCNPJDoCliente, client.NomeCompletoDoCliente?.ToUpper() ?? "");
            b.Sacado.Endereco.End = client.EnderecoDoCliente == null ? "" : client.EnderecoDoCliente?.ToUpper() + " nº " + client.NumeroDoCliente;
            b.Sacado.Endereco.Bairro = client.BairroDoCliente.ToUpper();
            b.Sacado.Endereco.Cidade = client.Cidades.NomeDaCidade.ToUpper();
            b.Sacado.Endereco.CEP = client.CEPDoCliente;
            b.Sacado.Endereco.UF = client.Estados.NomeDoEstado;

            //informações ao sacado
            Instrucao_Sicredi item1 = new Instrucao_Sicredi();
            Instrucao_Sicredi item2 = new Instrucao_Sicredi();
            Instrucao_Sicredi item3 = new Instrucao_Sicredi();
            Instrucao_Sicredi item4 = new Instrucao_Sicredi();
            Instrucao_Sicredi item5 = new Instrucao_Sicredi();
            Instrucao_Sicredi item6 = new Instrucao_Sicredi();
            Instrucao_Sicredi item7 = new Instrucao_Sicredi();
            Instrucao_Sicredi item8 = new Instrucao_Sicredi();
            Instrucao_Sicredi item9 = new Instrucao_Sicredi();

            item1.Descricao += tit.DescricaoDoTitulo;
            b.Instrucoes.Add(item1);
            item2.Descricao += "SR. CAIXA NÃO RECEBER APÓS O 5º DIA, PERDE-SE A COBERTURA CFC GRUPO";
            b.Instrucoes.Add(item2);
            item3.Descricao += emp.TextoMulta;
            b.Instrucoes.Add(item3);
            item4.Descricao += emp.TextoMora;
            b.Instrucoes.Add(item4);
            item5.Descricao += "PROCESSAMENTO BANCÁRIO " + $"{emp.TaxaDoBoleto:N2}";
            b.Instrucoes.Add(item5);
            item6.Descricao += "";
            b.Instrucoes.Add(item6);
            item7.Descricao += tit.Observacao1;
            b.Instrucoes.Add(item7);
            item8.Descricao += tit.Observacao2;
            b.Instrucoes.Add(item8);
            item9.Descricao += tit.Observacao3;
            b.Instrucoes.Add(item9);            

            tit.GeradoBoleto = true;
            tit.NossoNumero = tit.NossoNumero ?? string.Concat(tit.DataDoTitulo.ToString("yy"), b.NossoNumero);
            db.SaveChanges();

            b.Remessa = new Remessa(TipoOcorrenciaRemessa.EntradaDeTitulos);

            BoletoBancario boletoBancario = new BoletoBancario();
            boletoBancario.CodigoBanco = 748;
            boletoBancario.Boleto = b;
            //boletoBancario.FormatoCarne = true;
            //boletoBancario.OcultarEnderecoSacado = true;
            boletoBancario.OcultarInstrucoes = true;

            var demonstrativo = db.Demonstrativo.Where(x => x.CodigoDaMovimentacao == id);
            if (!demonstrativo.Any()) return boletoBancario;

            boletoBancario.ExibirDemonstrativo = true;
            boletoBancario.OcultarReciboSacado = true;

            var demo = demonstrativo.GroupBy(item => item.TituloDoGrupo)
                .Select(group => new {Grupo = @group.Key, Itens = @group.ToList()})
                .ToList();

            foreach (var grupo in demo)
            {
                var grupoTemp =
                    new BoletoNet.DemonstrativoValoresBoleto.GrupoDemonstrativo {Descricao = grupo.Grupo};

                foreach (var itens in grupo.Itens)
                {
                    var itemTemp = new ItemDemonstrativo
                    {
                        Referencia = itens.ItemReferencia,
                        Descricao = itens.ItemDescricao,
                        Valor = itens.ItemValor ?? 0
                    };
                    grupoTemp.Itens.Add(itemTemp);
                }
                b.Demonstrativos.Add(grupoTemp);
            }
            return boletoBancario;
        }
    }
}