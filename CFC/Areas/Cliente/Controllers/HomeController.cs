using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BoletoNet;
using CFC.Models;
using CFC.Uteis;

namespace CFC.Areas.Cliente.Controllers
{
    public class HomeController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();
        
        [AllowAnonymous]
        public ActionResult Index(string id)
        {
            ViewBag.Erro = id;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(string usuario, string password, string returnUrl)
        {
            var numeros = new Regex(@"[^\d]");
            usuario = numeros.Replace(usuario, "");

            if (!Response.LoginCliente(usuario, password, false)) return RedirectToAction("Index", new { id = 1 });
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }            
            return RedirectToAction("Painel", "Home");
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            DadosSeguros.LogOff();
            return RedirectToAction("Index", "Home");
        }

        // GET: Cliente/Home
        [Authorize(Roles = "Cliente")]
        public ActionResult Painel()
        {
            if (DadosSeguros.Codigo == 0)
            {
                DadosSeguros.LogOff();
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            var itens = db.Itens;
            ViewBag.CotasAtivas = itens.Any(x => x.DataDaDesativacao == null) ? (itens.Where(x => x.DataDaDesativacao == null).Sum(x => x.Valor) / 10000) : 0;
            ViewBag.CotasInativas = itens.Any(x => x.DataDaDesativacao != null) ? itens.Where(x => x.DataDaDesativacao != null).Sum(x => x.Valor) / 10000 : 0;

            ViewBag.ItensAtivos = itens.Count(x => x.DataDaDesativacao == null);
            ViewBag.ItensInativos = itens.Count(x => x.DataDaDesativacao != null);

            ViewBag.Cfc = itens.Count(x => !x.Particular && x.DataDaDesativacao == null);
            ViewBag.Particular = itens.Count(x => x.Particular && x.DataDaDesativacao == null);

            var clientes = db.Clientes;
            ViewBag.ClientesAtivos = clientes.Count(x => x.ClienteAtivo);
            ViewBag.ClientesInativos = clientes.Count(x => !x.ClienteAtivo);
            return View();
        }

        [Authorize(Roles = "Cliente")]
        public ActionResult Cliente()
        {
            if (DadosSeguros.Codigo == 0)
            {
                DadosSeguros.LogOff();
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            Clientes clientes = db.Clientes.Find(DadosSeguros.Codigo);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // GET: Admin/Itens
        [Authorize(Roles = "Cliente")]
        public ActionResult Veiculos()
        {
            if (DadosSeguros.Codigo == 0)
            {
                DadosSeguros.LogOff();
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            ViewBag.Cliente = DadosSeguros.Codigo;
            return View();
        }

        [Authorize(Roles = "Cliente")]
        public JsonResult Lista(jQueryDataTableParamModel param, int id = 0)
        {


            var data = (from x in db.Itens.ToList()
                        select new
                        {
                            x.CodigoDoItem,
                            x.Placa1DoVeiculo,
                            x.DescricaoDoModelo,
                            x.Clientes.NomeCFC,
                            x.CodigoDocliente,
                            Valor = $"{x.Valor:N2}",
                            Ativo = x.DataDaDesativacao != null ? "D " + x.DataDaDesativacao.Value.ToShortDateString() : "A " + x.DataDaAtivacao.Value.ToShortDateString()
                        });
            
            if (id != 0)
            {
                data = data.Where(x => x.CodigoDocliente == id);
            }
            else
            {
                return Json("");
            }

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                param.sSearch = param.sSearch.ToUpper();
                data = data.Where(c => c.NomeCFC.Contains(param.sSearch) || c.Placa1DoVeiculo.Contains(param.sSearch) || c.DescricaoDoModelo.Contains(param.sSearch));
            }

            var result = data;
            if (param.iDisplayLength != -1)
            {
                result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            }

            return Json(new
            {
                param.sEcho,
                iTotalRecords = db.Itens.Count(x => x.CodigoDocliente == id),
                iTotalDisplayRecords = data.Count(),
                aaData = from x in result
                         select new[]
                        {
                             x.CodigoDoItem.ToString(),
                             x.Placa1DoVeiculo,
                             x.DescricaoDoModelo,
                             x.NomeCFC,
                             x.Valor,
                             x.Ativo
                        }
            },
                    JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Cliente")]
        public ActionResult Financeiro()
        {
            if (DadosSeguros.Codigo == 0)
            {
                DadosSeguros.LogOff();
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            ViewBag.Cliente = DadosSeguros.Codigo;
            return View();
        }

        // GET: Admin/Itens/Details/5
        [Authorize(Roles = "Cliente")]
        public ActionResult DetalheVeiculo(int? id)
        {
            if (DadosSeguros.Codigo == 0)
            {
                DadosSeguros.LogOff();
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Itens itens = db.Itens.FirstOrDefault(x => x.CodigoDocliente == DadosSeguros.Codigo && x.CodigoDoItem == id);

            if (itens == null)
            {
                return HttpNotFound();
            }
            return View(itens);
        }

        [Authorize(Roles = "Cliente")]
        public JsonResult ListaBoletos(jQueryDataTableParamModel param, string cancelado, string recebido, string descricao, string vInicio, string vFim, string rInicio, string rFim, int codigo = 0, int id = 0)
        {


            var data = (from x in db.Movimentacao.Where(x => x.Boleto && x.GeradoBoleto).ToList()
                        select new
                        {
                           Codigo = x.CodigoDoTitulo,
                            x.CodigoDoCliente,
                            x.DescricaoDoTitulo,
                            x.DataDoVencimento,
                            x.DataDoRecebimento,
                            x.ValorDoTitulo,
                            x.Cancelado
                        });

            if (id != 0)
            {
                data = data.Where(x => x.CodigoDoCliente == id);
            }
            else
            {
                return Json("");
            }

            //buscas
            if (data.Any())
            {
                if (codigo != 0)
                {
                    data = data.Where(x => x.Codigo == codigo);
                }

                if (descricao != "")
                {                    
                    data = data.Where(x => x.DescricaoDoTitulo.Contains(descricao));
                }

                if (cancelado != "")
                {
                    if (cancelado == "S")
                    {
                        data = data.Where(x => x.Cancelado);
                    }
                    if (cancelado == "N")
                    {
                        data = data.Where(x => !x.Cancelado);
                    }
                }
                if (recebido != "")
                {
                    if (recebido == "S")
                    {
                        data = data.Where(x => x.DataDoRecebimento != null);
                    }
                    if (recebido == "N")
                    {
                        data = data.Where(x => x.DataDoRecebimento == null);
                    }
                }

                if (rInicio != "")
                {
                    var datas = Convert.ToDateTime(rInicio);
                    data = data.Where(x => x.DataDoRecebimento >= datas);
                }
                if (rFim != "")
                {
                    var datas = Convert.ToDateTime(rFim);
                    data = data.Where(x => x.DataDoRecebimento <= datas);
                }
                if (vInicio != "")
                {
                    var datas = Convert.ToDateTime(vInicio);
                    data = data.Where(x => x.DataDoVencimento >= datas);
                }
                if (vFim != "")
                {
                    var datas = Convert.ToDateTime(vFim);
                    data = data.Where(x => x.DataDoVencimento <= datas);
                }
            }

            data = param.sSortDir_0 == "desc" ? data.OrderBy(x => x.Codigo) : data.OrderByDescending(x => x.Codigo);


            var result = data;
            if (param.iDisplayLength != -1)
            {
                result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            }

            return Json(new
            {
                param.sEcho,
                iTotalRecords = db.Movimentacao.Count(x =>  x.Boleto && x.GeradoBoleto && x.CodigoDoCliente == id),
                iTotalDisplayRecords = data.Count(),
                aaData = from x in result
                         select new[]
                        {
                             x.Codigo.ToString(),
                             x.Codigo.ToString(),
                             x.DescricaoDoTitulo,
                             x.DataDoVencimento.ToShortDateString(),
                             x.DataDoRecebimento?.ToShortDateString() ?? "",
                             x.ValorDoTitulo.ToString(CultureInfo.CurrentCulture),
                             x.Cancelado ? "S" : "N"
                        }
            },
                    JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImprimirBoletos(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Titulo = "Selecione pelo menos um Boleto na lista [x]";
                return View();
            }

            var stringkeys = id.Split(',');
            var keys = stringkeys.Select(int.Parse).ToArray();

            var titulo = "";
            var count = 0;

            foreach (var itens in keys)
            {
                count = count + 1;
                var os = "";                
                titulo = titulo + os + Sicredi(itens) + ((count % 3 == 0) ? "<div style='page-break-before: always'></div>" : "");
            }

            ViewBag.Titulo = titulo;
            return View();
        }

        //boleto
        //public BoletoBancario BoletoBancario { get; set; }
        public string Sicredi(int id)
        {
            #region Código Antigo
            //BoletoBancario = new BoletoBancario();
            //BoletoBancario.CodigoBanco = 748;

            //var titulo = db.Movimentacao.FirstOrDefault(x => x.CodigoDoTitulo == id && x.Boleto);
            //if (titulo?.CodigoDoCliente == null || titulo.CodigoDoCliente == 0) return "Marcado como um boleto mas não possui um Cliente verifique! Cod. " + id;

            //var emp = db.Configuracoes.FirstOrDefault();

            //var cliente = db.Clientes.Find(titulo.CodigoDoCliente);

            //Instrucao_Sicredi item1 = new Instrucao_Sicredi();
            //Instrucao_Sicredi item2 = new Instrucao_Sicredi();
            //Instrucao_Sicredi item3 = new Instrucao_Sicredi();
            //Instrucao_Sicredi item4 = new Instrucao_Sicredi();
            //Instrucao_Sicredi item5 = new Instrucao_Sicredi();
            //Instrucao_Sicredi item6 = new Instrucao_Sicredi();
            //Instrucao_Sicredi item7 = new Instrucao_Sicredi();

            //Cedente c = new Cedente(emp.CnpjDaEmpresa, emp.RazaoSocialDaEmpresa, emp.NumeroDaAgenciaBancaria, emp.DigitoDaAgenciaBancaria, emp.NumeroDaContaBancaria, emp.DigitoDaContaBancaria);
            //c.Codigo = emp.NumeroDaContaBancaria;
            //c.ContaBancaria.OperacaConta = "03";

            //Boleto b = new Boleto(Convert.ToDateTime(titulo.DataDoVencimento), Convert.ToDecimal(titulo.ValorDoTitulo), "1", titulo.NossoNumero, c);
            //b.NumeroDocumento = titulo.NossoNumero;

            //b.Sacado = new Sacado(cliente.CPFCNPJDoCliente, cliente.NomeCompletoDoCliente.ToUpper());
            //b.Sacado.Endereco.End = cliente.EnderecoDoCliente.ToUpper() + "," + cliente.NumeroDoCliente;
            //b.Sacado.Endereco.Bairro = cliente.BairroDoCliente.ToUpper();
            //b.Sacado.Endereco.Cidade = cliente.Cidades.NomeDaCidade.ToUpper();
            //b.Sacado.Endereco.CEP = cliente.CEPDoCliente;
            //b.Sacado.Endereco.UF = cliente.Estados.NomeDoEstado;
            //b.ContaBancaria.OperacaConta = "03";

            //// adicionar mais informações ao sacado
            //item1.Descricao += titulo.DescricaoDoTitulo;
            //b.Instrucoes.Add(item1);
            //item2.Descricao += "SR. CAIXA NÃO RECEBER APÓS O 10º DIA, PERDE-SE A COBERTURA CFC GRUPO";
            //b.Instrucoes.Add(item2);
            //item3.Descricao += emp.TextoMulta;
            //b.Instrucoes.Add(item3);
            //item4.Descricao += emp.TextoMora;
            //b.Instrucoes.Add(item4);
            //item5.Descricao += titulo.Observacao1;
            //b.Instrucoes.Add(item5);
            //item6.Descricao += titulo.Observacao2;
            //b.Instrucoes.Add(item6);
            //item7.Descricao += titulo.Observacao3;
            //b.Instrucoes.Add(item7);

            //BoletoBancario.Boleto = b;
            //BoletoBancario.FormatoCarne = true;
            ////boletoBancario.MostrarEnderecoCedente = false;
            //BoletoBancario.OcultarEnderecoSacado = true;
            //BoletoBancario.OcultarInstrucoes = true;
            //BoletoBancario.Boleto.Valida();

            //titulo.GeradoBoleto = true;
            //titulo.NossoNumero = titulo.NossoNumero;
            ////db.Entry(titulo).State = EntityState.Modified;
            ////db.SaveChanges();
            #endregion

            var boleto = GerarBoleto.Boleto(id);
            boleto.Boleto.Valida();
            return boleto.MontaHtmlEmbedded();            

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}