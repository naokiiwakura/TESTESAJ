using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CFC.Models;
using BoletoNet;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.DynamicData;
using System.Web.Routing;
using System.Web.Security;
using CFC.Uteis;
using Microsoft.Ajax.Utilities;

namespace CFC.Areas.Admin.Controllers
{    
    [Authorize(Roles = "Admin")]
    public class MovimentacaoController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();
        
        public ActionResult Index()
        {
            var lista = (from x in db.Clientes
                         select new
                         {
                             x.CodigoDoCliente,
                             descricao = x.CodigoDoCliente + " " + x.NomeCFC.ToUpper() + " | " + x.Cidades.NomeDaCidade.ToUpper() + "/" + x.Estados.NomeDoEstado
                         }).ToList();
            ViewBag.Cliente = new SelectList(lista, "CodigoDoCliente", "descricao");
            return View(db.Movimentacao.ToList());
        }
        
        public JsonResult Lista(jQueryDataTableParamModel param, string vInicio, string vFim, string rInicio, string rFim, string boleto, string admin, string cancelado, string recebido, int id = 0, int cliente = 0, int codigo = 0)
        {

            var movimentacao = db.Movimentacao.OrderByDescending(x => x.CodigoDoTitulo);

            if (param.sSortDir_0 == "desc")
            {
                movimentacao = movimentacao.OrderBy(x => x.CodigoDoTitulo);
            }

            var data = (from x in movimentacao.ToList()      
                        join c in db.Clientes on x.CodigoDoCliente equals c.CodigoDoCliente into c1
                        join f in db.Fornecedores on x.CodigoDoFornecedor equals f.CodigoDoFornecedor into f1
                        let c = db.Clientes
                        //let f = db.Fornecedores
                select new
                {
                    Codigo = x.CodigoDoTitulo,
                    Titulo = c1.Any() ? c1.First().NomeCFC +" - " +c1.First().Cidades.NomeDaCidade + "/" + c1.First().Estados.NomeDoEstado + "<br>" + c1.First().NomeCompletoDoCliente : f1.Any() ? f1.First().NomeDoFornecedor : "", //x.CodigoDoCliente == null ? "" : c.CodigoDoCliente + " " + c.NomeCFC + " - " + c.Cidades.NomeDaCidade + "/" + c.Estados.NomeDoEstado,
                    DataDoRecebimento = x.DataDoRecebimento?.ToShortDateString() ?? "",
                    DataDoVencimento = x.DataDoVencimento.ToShortDateString(),
                    DataVencimento = x.DataDoVencimento,
                    DataRecebimento = x.DataDoRecebimento,
                    x.Boleto ,
                    x.BoletoAdmin,
                    x.Cancelado,
                    x.CodigoDoCliente,
                    x.NumeroParcela,
                    x.TipoDoTitulo,
                    x.DescricaoDoTitulo,
                    x.GeradoBoleto,
                    x.GeradoRemessa,
                    ValorDoTitulo = $"{x.ValorDoTitulo:N2}"

                });

            //buscas
            if (data.Any())
            {
                if (codigo != 0)
                {
                    data = data.Where(x => x.Codigo == codigo);
                }
                if (cliente != 0)
                {
                    data = data.Where(x => x.CodigoDoCliente == cliente);
                }
                if (boleto != "")
                {
                    if (boleto == "S")
                    {
                        data = data.Where(x => x.Boleto);
                    }
                    if (boleto == "N")
                    {
                        data = data.Where(x => !x.Boleto);
                    }
                }
                if (admin != "")
                {
                    if (admin == "S")
                    {
                        data = data.Where(x => x.BoletoAdmin);
                    }
                    if (boleto == "N")
                    {
                        data = data.Where(x => !x.BoletoAdmin);
                    }
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
                    data = data.Where(x => x.DataRecebimento >= datas);                                                       
                }
                if (rFim != "")
                {
                    var datas = Convert.ToDateTime(rFim);
                    data = data.Where(x => x.DataRecebimento <= datas);
                }
                if (vInicio != "")
                {
                    var datas = Convert.ToDateTime(vInicio);
                    data = data.Where(x => x.DataVencimento >= datas);
                }
                if (vFim != "")
                {
                    var datas = Convert.ToDateTime(vFim);
                    data = data.Where(x => x.DataVencimento <= datas);
                }
            }
            
            //else
            //{
            //    data = data.OrderByDescending(x => x.Codigo);
            //}

            var result = data;
            if (param.iDisplayLength != -1)
            {
                result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            }

            return Json(new
                {
                    param.sEcho,
                    iTotalRecords = db.Movimentacao.Count(),
                    iTotalDisplayRecords = data.Count(),
                    aaData = from x in result
                    select new[]
                    {
                        x.Codigo.ToString(),
                        x.Codigo.ToString(),
                        x.Titulo,
                        x.DescricaoDoTitulo,
                        x.TipoDoTitulo,
                        x.DataDoVencimento,
                        x.DataDoRecebimento,
                        x.ValorDoTitulo.ToString(CultureInfo.InvariantCulture),
                        x.GeradoRemessa ? "R" : x.GeradoBoleto ? "G": x.Boleto ? "B" : "N",
                        x.BoletoAdmin ? "S" : "N",
                        x.Cancelado ? "S" : "N"
                    }
                },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Titulos()
        {
            var lista = (from x in db.Clientes.Where(x => x.ClienteAtivo)
                select new
                {
                    x.CodigoDoCliente,
                    descricao = x.CodigoDoCliente + " " + x.NomeCFC.ToUpper() + " | " + x.Cidades.NomeDaCidade.ToUpper() + "/" + x.Estados.NomeDoEstado
                }).ToList();
            ViewBag.Cliente = new SelectList(lista, "CodigoDoCliente", "descricao");
            return View(db.Movimentacao.ToList());            
        }

        public JsonResult ListaTitulos(jQueryDataTableParamModel param, string vInicio, string vFim, string rInicio,string rFim, string boleto, string admin, string cancelado, string recebido, int id = 0, int cliente = 0,int codigo = 0)
        {
            return Json("");
        }

        public ActionResult Create()
        {
            var lista = (from x in db.Clientes.Where(x => x.ClienteAtivo)
                         select new
                         {
                             x.CodigoDoCliente,
                             descricao = x.NomeCFC.ToUpper() + " | " + x.Cidades.NomeDaCidade.ToUpper() + "/" + x.Estados.NomeDoEstado
                         }).ToList();

            var sinistro = (from x in db.Sinistros.Where(x => x.DataDeConclusaoDoSinistro == null && x.DataDoFaturamento == null)
                select new
                {
                    x.CodigoDoSinistro,
                    descricao = x.CodigoDoSinistro + " " + x.Itens.DescricaoDoModelo + " - " + x.Itens.Placa1DoVeiculo
                }).ToList();

            ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao");
            ViewBag.CodigoDoHistorico = new SelectList(db.Historicos, "CodigoDoHistorico", "DescricaoDoHistorico");
            ViewBag.CodigoDoFornecedor = new SelectList(db.Fornecedores, "CodigoDoFornecedor", "NomeDoFornecedor");
            ViewBag.CodigoDoSinistro = new SelectList(sinistro, "CodigoDoSinistro", "descricao");
            return View();
        }

        //
        // POST: /movimentacao/Create        
        [HttpPost]
        public ActionResult Create(Movimentacao movimentacao, string tipodemovimentacao)
        {
            switch (tipodemovimentacao)
            {
                case "1":
                    movimentacao.Credito = true;
                    break;
                case "2":
                    movimentacao.Debito = true;
                    break;
            }

            var lista = (from x in db.Clientes.Where(x => x.ClienteAtivo)
                select new
                {
                    x.CodigoDoCliente,
                    descricao = x.CodigoDoCliente + " " + x.NomeCFC.ToUpper() + " - " + x.Cidades.NomeDaCidade.ToUpper() + "/" + x.Estados.NomeDoEstado
                }).ToList();
            var sinistro = (from x in db.Sinistros.Where(x => x.DataDeConclusaoDoSinistro == null && x.DataDoFaturamento == null)
                select new
                {
                    x.CodigoDoSinistro,
                    descricao = x.CodigoDoSinistro + " " + x.Itens.DescricaoDoModelo + " - " + x.Itens.Placa1DoVeiculo
                }).ToList();

            if (movimentacao.DataDoVencimento.AddDays(-7) < movimentacao.DataDoTitulo)
            {
                TempData["erro"] = "Data de Vencimento deve ter no mínimo sete dias a mais que a data de emissão";
                ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao", movimentacao.CodigoDoCliente);
                ViewBag.CodigoDoHistorico = new SelectList(db.Historicos, "CodigoDoHistorico", "DescricaoDoHistorico", movimentacao.CodigoDoHistorico);
                ViewBag.CodigoDoFornecedor = new SelectList(db.Fornecedores, "CodigoDoFornecedor", "NomeDoFornecedor", movimentacao.CodigoDoFornecedor);
                ViewBag.CodigoDoSinistro = new SelectList(sinistro, "CodigoDoSinistro", "descricao", movimentacao.CodigoDoSinistro);
                return View(movimentacao);
            }
            if (ModelState.IsValid)
            {
                movimentacao.Foto1 = UploadImagens.UploadRename(Request.Files["f1"], "Foto1", null);
                movimentacao.Foto2 = UploadImagens.UploadRename(Request.Files["f2"], "Foto2", null);
                movimentacao.Foto3 = UploadImagens.UploadRename(Request.Files["f3"], "Foto3", null);
                movimentacao.Foto4 = UploadImagens.UploadRename(Request.Files["f4"], "Foto4", null);               

                db.Movimentacao.Add(movimentacao);
                db.SaveChanges();                

                return RedirectToAction("Index");
            }

            ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao", movimentacao.CodigoDoCliente);
            ViewBag.CodigoDoHistorico = new SelectList(db.Historicos, "CodigoDoHistorico", "DescricaoDoHistorico", movimentacao.CodigoDoHistorico);
            ViewBag.CodigoDoFornecedor = new SelectList(db.Fornecedores, "CodigoDoFornecedor", "NomeDoFornecedor", movimentacao.CodigoDoFornecedor);
            ViewBag.CodigoDoSinistro = new SelectList(sinistro, "CodigoDoSinistro", "descricao", movimentacao.CodigoDoSinistro);

            return View(movimentacao);
        }

        //
        // GET: /movimentacao/Edit/5
        public ActionResult Edit(int id)
        {
            Movimentacao movimentacao = db.Movimentacao.Find(id);

            var lista = (from x in db.Clientes.Where(x => x.CodigoDoCliente == movimentacao.CodigoDoCliente)
                select new
                {
                    x.CodigoDoCliente,
                    descricao = x.CodigoDoCliente + " " + x.NomeCFC.ToUpper() + " - " + x.Cidades.NomeDaCidade.ToUpper() + "/" + x.Estados.NomeDoEstado
                }).ToList();
            var sinistro = (from x in db.Sinistros.Where(x => x.CodigoDoSinistro == movimentacao.CodigoDoSinistro)
                select new
                {
                    x.CodigoDoSinistro,
                    descricao = x.CodigoDoSinistro + " " + x.Itens.DescricaoDoModelo + " - " + x.Itens.Placa1DoVeiculo
                }).ToList();


            ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao", movimentacao.CodigoDoCliente);
            ViewBag.CodigoDoHistorico = new SelectList(db.Historicos, "CodigoDoHistorico", "DescricaoDoHistorico", movimentacao.CodigoDoHistorico);
            ViewBag.CodigoDoFornecedor = new SelectList(db.Fornecedores, "CodigoDoFornecedor", "NomeDoFornecedor", movimentacao.CodigoDoFornecedor);
            ViewBag.CodigoDoSinistro = new SelectList(sinistro, "CodigoDoSinistro", "descricao", movimentacao.CodigoDoSinistro);

            //if (movimentacao.CodigoDoCliente != 0 || movimentacao.CodigoDoCliente != null)
            //{
            //    ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao", movimentacao.CodigoDoCliente);
            //}
            return View(movimentacao);
        }

        //
        // POST: /movimentacao/Edit/5        
        [HttpPost]
        public ActionResult Edit(Movimentacao movimentacao, string tipodemovimentacao)
        {
            switch (tipodemovimentacao)
            {
                case "1":
                    movimentacao.Credito = true;
                    break;
                case "2":
                    movimentacao.Debito = true;
                    break;
            }
            if (ModelState.IsValid)
            {
                movimentacao.Foto1 = UploadImagens.UploadRename(Request.Files["f1"], "Foto1", movimentacao.Foto1);
                movimentacao.Foto2 = UploadImagens.UploadRename(Request.Files["f2"], "Foto2", movimentacao.Foto2);
                movimentacao.Foto3 = UploadImagens.UploadRename(Request.Files["f3"], "Foto3", movimentacao.Foto3);
                movimentacao.Foto4 = UploadImagens.UploadRename(Request.Files["f4"], "Foto4", movimentacao.Foto4); 

                db.Entry(movimentacao).State = EntityState.Modified;
                db.SaveChanges();                

                return RedirectToAction("Index");
            }
            var lista = (from x in db.Clientes.Where(x => x.CodigoDoCliente == movimentacao.CodigoDoCliente)
                select new
                {
                    x.CodigoDoCliente,
                    descricao = x.CodigoDoCliente + " " + x.NomeCFC.ToUpper() + " - " + x.Cidades.NomeDaCidade.ToUpper() + "/" + x.Estados.NomeDoEstado
                }).ToList();
            var sinistro = (from x in db.Sinistros.Where(x => x.CodigoDoSinistro == movimentacao.CodigoDoSinistro)
                select new
                {
                    x.CodigoDoSinistro,
                    descricao = x.CodigoDoSinistro + " " + x.Itens.DescricaoDoModelo + " - " + x.Itens.Placa1DoVeiculo
                }).ToList();


            ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao", movimentacao.CodigoDoCliente);
            ViewBag.CodigoDoHistorico = new SelectList(db.Historicos, "CodigoDoHistorico", "DescricaoDoHistorico", movimentacao.CodigoDoHistorico);
            ViewBag.CodigoDoFornecedor = new SelectList(db.Fornecedores, "CodigoDoFornecedor", "NomeDoFornecedor", movimentacao.CodigoDoFornecedor);
            ViewBag.CodigoDoSinistro = new SelectList(sinistro, "CodigoDoSinistro", "descricao", movimentacao.CodigoDoSinistro);

            //if (movimentacao.CodigoDoCliente != 0 || movimentacao.CodigoDoCliente != null)
            //{
            //    ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao", movimentacao.CodigoDoCliente);
            //}
            return View(movimentacao);
        }
        
        [HttpGet]
        public ActionResult Cancelar(int id, string opt)
        {
            var movimentacao = db.Movimentacao.Find(id);
            switch (opt)
            {
                case "ativa":
                    movimentacao.Cancelado = false;
                    break;
                case "cancela":
                    movimentacao.Cancelado = true;
                    break;
            }
            
            db.Entry(movimentacao).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");            
        }

        //
        // POST: /movimentacao/Delete/5        
        public JsonResult DeleteConfirmed(int id)
        {
            var response = new
            {
                success = true,
                messages = "Registro excluido!"

            };

            try
            {
                foreach (Demonstrativo demo in db.Demonstrativo.Where(x => x.CodigoDaMovimentacao == id))
                {
                    db.Demonstrativo.Remove(demo);
                }

                Movimentacao movimentacao = db.Movimentacao.Find(id);

                movimentacao.Foto1 = UploadImagens.UploadRename(null, null, movimentacao.Foto1);
                movimentacao.Foto2 = UploadImagens.UploadRename(null, null, movimentacao.Foto2);
                movimentacao.Foto3 = UploadImagens.UploadRename(null, null, movimentacao.Foto3);
                movimentacao.Foto4 = UploadImagens.UploadRename(null, null, movimentacao.Foto4);

                db.Movimentacao.Remove(movimentacao);
                db.SaveChanges();
            }
            catch
            {
                response = new
                {
                    success = false,
                    messages = "Algo de ERRADO aconteceu, procure o Administrador!"

                };

            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public string Sicredi(int id)
        {

            var boleto = GerarBoleto.Boleto(id);
            boleto.Boleto.Valida();

            return boleto.MontaHtmlEmbedded();
        }        

        public void GeraDadosSicredi()
        {

            var emp = db.Configuracoes.FirstOrDefault();

            Boletos boletos = new Boletos();
            var remessas = db.Movimentacao.Where(x => !x.GeradoRemessa && x.GeradoBoleto && x.Boleto && !x.Cancelado);

            foreach (Movimentacao rem in remessas.ToList())
            {
                //boleto
                var boleto = GerarBoleto.Boleto(rem.CodigoDoTitulo);
                boleto.Boleto.Valida();
                boletos.Add(boleto.Boleto);

                //grava titulo como remessa gerada
                rem.GeradoRemessa = true;
                db.SaveChanges();

            }

            emp.UltimaRemessa = emp.UltimaRemessa + 1;
            var remessa = emp.UltimaRemessa;
            db.SaveChanges();

            GeraArquivoCnab400(boletos, remessa);
        }

        public void GeraArquivoCnab400(Boletos boletos, int remessa)
        {
            var itensRemessa = boletos;

            var banco = itensRemessa.First().Banco;
            var cedente = itensRemessa.First().Cedente;

            ArquivoRemessa arquivoRemessa = new ArquivoRemessa(TipoArquivo.CNAB400);

            var path = System.Web.HttpContext.Current.Server.MapPath("~/Boletos/Adm-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            var fi = new FileInfo(path);
            if (fi.Exists)
            {
                fi.Delete();
            }

            using (var fs = fi.Create())
            {
                arquivoRemessa.GerarArquivoRemessa("1", banco, cedente, boletos, fs, remessa);

                var mes = DateTime.Now.Month == 10 ? "O" : DateTime.Now.Month == 11 ? "N" : DateTime.Now.Month == 12 ? "D" : DateTime.Now.Month.ToString();

                HttpContext.Response.Clear();
                HttpContext.Response.AddHeader("content-disposition", $"attachment;filename={cedente.ContaBancaria.Conta}{mes}{DateTime.Now:dd}.CRM");

                HttpContext.Response.ContentType = "text/plain";
                HttpContext.Response.ContentEncoding = Encoding.Default;

                HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                Response.WriteFile(fi.FullName);
                Response.End();

            }
        }

        public ActionResult Retorno()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult LeituraDoRetorno()
        {
            if (Request.Files[0] == null || Request.Files[0].ContentLength <= 0) return PartialView();

            var empresa = db.Configuracoes.FirstOrDefault();

            var banco = new Banco(748);
            var retorno = new ArquivoRetornoCNAB400();

            //le o retorno
            retorno.LerArquivoRetorno(banco, Request.Files[0].InputStream);

            var tabela = "<div class='col-lg-12 col-md-12'><div class='panel panel-default toggle'><div class='panel-heading white-bg'><h4>PROCESSAMENTO DE RETORNO " +
                         Request.Files[0].FileName + " <a class='button blue' href='#' data-retorno='" + Request.Files[0].FileName + "' id='save-link'>Download</a></h2></div>" +
                         "<div class='panel-body'><table class='table table-hover'>" +
                         "<thead>" +
                         "<tr>" +
                         "<th>TIPO</th>" +
                         "<th>OCORRÊNCIA</th>" +
                         "<th>NUMERO</th>" +
                         "<th>VALOR</th>" +
                         "<th>LANÇAMENTO</th>" +
                         "<th>MOTIVO</th>" +
                         "<th>MENSAGEM</th>" +
                         "</tr></thead><tbody>";

            foreach (var r in retorno.ListaDetalhe.OrderBy(x => x.NumeroDocumento))
            {
                if (empresa.NumeroDaContaBancaria != (retorno.HeaderRetorno.Conta.ToString() + retorno.HeaderRetorno.DACConta))
                {
                    tabela = tabela +
                             "<tr><td colspan='7'><strong style='color:red'>CONTA CORRENTE DO RETORNO NÃO É A MESMA DO PROCESSAMENTO!</strong></td></tr>";
                    break;
                }
                var motivo = r.MotivoCodigoOcorrencia.Substring(0, 2);
                var numero = r.NumeroDocumento ?? r.NossoNumero.Substring(3, 5).TrimStart('0');


                if (r.CodigoOcorrencia == 2)
                {
                    tabela = tabela + "<tr>" +
                             "<td><span class='badge badge-primary mr10 mb10'>ENT</span></td>" +
                             "<td>" + Ocorrencia(r.CodigoOcorrencia) + "</td>" +
                             "<td>" + numero + "</td>" +
                             "<td></td>" +
                             "<td></td>" +
                             "<td>00-Aceito pelo Banco</td>" +
                             "<td></td></tr>";
                }
                if (r.CodigoOcorrencia == 3)
                {
                    tabela = tabela + "<tr>" +
                             "<td><span class='badge badge-danger mr10 mb10'>ERR</span></td>" +
                             "<td>" + Ocorrencia(r.CodigoOcorrencia) + "</td>" +
                             "<td>" + numero + "</td>" +
                             "<td></td>" +
                             "<td></td>" +
                             "<td>" + MotivoRejeicao(motivo) + "</td>" +
                             "<td></td></tr>";
                }
                if (r.CodigoOcorrencia == 6)
                {
                    var byt = Convert.ToInt32(r.NossoNumero.Substring(2, 1));
                    var codigo = Convert.ToInt32(r.NossoNumero.Substring(3, 5).TrimStart('0'));
                    var titulos = byt >= 2 ? db.Movimentacao.Find(codigo) : null;
                    string mensagem;

                    if (byt >= 2)
                    {

                        if (titulos == null || !titulos.GeradoBoleto && !titulos.GeradoRemessa)
                        {
                            mensagem = "<strong style='color:red'>Titulo não encontrado no Sistema VERIFIQUE!</strong>";
                        }
                        else if (titulos.DataDoRecebimento != null)
                        {
                            mensagem = "<strong style='color:blue'>Titulo já Processado no Sistema!</strong>";
                        }
                        else
                        {
                            mensagem = "Processado no Sistema";

                            if (titulos.ValorDoTitulo != r.ValorPago)
                            {
                                mensagem = "<strong>Valor divergente Titulo: " + titulos.ValorDoTitulo + " - Recebido:" +
                                           r.ValorPago + "</strong>";
                            }

                            titulos.DataDoRecebimento = r.DataOcorrencia;
                            titulos.TipoDaBaixa = "Automatica";
                            db.SaveChanges();                            

                        }
                    }
                    else
                    {
                        mensagem = "<strong><span style='color:red'>Titulo feito via Banco! CONFERIR!</span><strong>";
                    }

                    tabela = tabela + "<tr>" +
                             "<td><span class='badge badge-success mr10 mb10'>REC</span></td>" +
                             "<td>" + Ocorrencia(r.CodigoOcorrencia) + "</td>" +
                             "<td>" + numero + "</td>" +
                             "<td>R$" + r.ValorPago + "</td>" +
                             "<td>" + r.DataCredito.ToShortDateString() + "</td>" +
                             "<td>" + MotivoRejeicao(motivo) + "</td>" +
                             "<td>" + mensagem +
                             (byt >= 2 && titulos != null ? "<strong style='color:green'> (Recebido)<strong" : "") +
                             "</td></tr>";
                }
                if (r.CodigoOcorrencia == 28)
                {
                    tabela = tabela + "<tr>" +
                             "<td><span class='badge badge-warning mr10 mb10'>TAR</span></td>" +
                             "<td>" + Ocorrencia(r.CodigoOcorrencia) + "</td>" +
                             "<td>" + numero + "</td>" +
                             "<td>R$" + r.ValorPago + "</td>" +
                             "<td>" + r.DataCredito.ToShortDateString() + "</td>" +
                             "<td>" + Tarifa(motivo) + "</td>" +
                             "<td></td></tr>";
                }
                if (r.CodigoOcorrencia == 33)
                {
                    tabela = tabela + "<tr>" +
                             "<td><span class='badge badge-info mr10 mb10'>ALT</span></td>" +
                             "<td>" + Ocorrencia(r.CodigoOcorrencia) + "</td>" +
                             "<td>" + numero + "</td>" +
                             "<td></td>" +
                             "<td></td>" +
                             "<td>00-Confirmação de pedido de alteração de outros dados</td>" +
                             "<td></td></tr>";
                }
            }

            tabela = tabela + "</tbody></table></div></div></div>";

            ViewBag.Retorno = tabela;
            return PartialView();
        }

        public ActionResult Situacao(DateTime? iniciotitulo, DateTime? fimtitulo, DateTime? iniciovencimento, DateTime? fimvencimento, DateTime? iniciorecebimento, DateTime? fimrecebimento, string TipoDaBaixa, string TipoDoTitulo)
        {
            var dados = from x in db.Movimentacao
                        select x;
            if (iniciotitulo != null)
            {
                dados = dados.Where(x => x.DataDoTitulo >= iniciotitulo);
            }

            if (fimtitulo != null)
            {
                dados = dados.Where(x => x.DataDoTitulo <= fimtitulo);
            }
            if (iniciovencimento != null)
            {
                dados = dados.Where(x => x.DataDoVencimento >= iniciovencimento);
            }
            if (fimvencimento != null)
            {
                dados = dados.Where(x => x.DataDoVencimento <= fimvencimento);
            }

            if (iniciorecebimento != null)
            {
                dados = dados.Where(x => x.DataDoRecebimento >= iniciorecebimento);
            }

            if (fimrecebimento != null)
            {
                dados = dados.Where(x => x.DataDoRecebimento <= fimrecebimento);
            }

            if (TipoDaBaixa != "")
            {
                dados = dados.Where(x => x.TipoDaBaixa == TipoDaBaixa);
            }

            if (TipoDoTitulo != "")
            {
                dados = dados.Where(x => x.TipoDoTitulo.Contains(TipoDoTitulo));
            }
            ViewBag.Dados = dados.OrderBy(x => x.DataDoVencimento);
            if (iniciotitulo != null) ViewBag.Inicio = iniciotitulo.Value.ToShortDateString();
            if (fimtitulo != null) ViewBag.Fim = fimtitulo.Value.ToShortDateString();
            return View();
        }

        public ActionResult EmitirBoletoCliente()
        {            
            return View(db.Movimentacao);
        }

        public ActionResult AtualizarBoleto(int id)
        {
            var titulo = db.Movimentacao.Find(id);
            //var emp = db.Configuracoes.FirstOrDefault();

            int dias = (DateTime.Today - titulo.DataDoVencimento).Days;

            decimal juros = (titulo.ValorDoTitulo * 2) / 100;
            decimal taxaMora = Convert.ToDecimal(0.01);
            decimal mora = ((titulo.ValorDoTitulo * taxaMora) / 100) * dias;
            decimal total = (juros + mora);

            if (titulo.Observacao1 == null && titulo.Observacao2 == null && titulo.Observacao3 == null)
            {
                titulo.Observacao1 = "VALOR ORIGINAL: R$" + titulo.ValorDoTitulo;
                titulo.Observacao2 = "JUROS/CORREÇÃO: R$" + decimal.Round(total, 2);
                titulo.Observacao3 = "VCTO ORIGINAL: " + titulo.DataDoVencimento.ToShortDateString();
                titulo.ValorDoTitulo = titulo.ValorDoTitulo + decimal.Round(total, 2);
            }

            titulo.ValorDoTitulo = titulo.ValorDoTitulo + decimal.Round(mora, 2);
            titulo.DataDoVencimento = DateTime.Today;
            titulo.TipoDoTitulo = "Boleto (CA)";

            db.Entry(titulo).State = EntityState.Modified;
            db.SaveChanges();


            return RedirectToAction("EmitirBoletoCliente");
        }
        
        public JsonResult AtualizarBoletoAdmin(string boletos, string vencimento)
        {
            var response = new
            {
                success = true,
                messages = "Boleto(s) Atualizados!"
            };

            if (string.IsNullOrEmpty(boletos))
            {
                response = new
                {
                    success = false,
                    messages = "Selecione pelo menos um Boleto!"

                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            if (vencimento == "")
            {
                response = new
                {
                    success = false,
                    messages = "A nova data de Vencimento não pode ser em Branco!"
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            var data = Convert.ToDateTime(vencimento);

            if (data < DateTime.Now)
            {
                response = new
                {
                    success = false,
                    messages = "Você não pode atualizar um Boleto com data inferior a Hoje!"

                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }               

            var stringkeys = boletos.Split(',');
            var keys = stringkeys.Select(int.Parse).ToArray();
           
            foreach (var itens in keys)
            {
                var cod = Convert.ToInt32(itens);

                var titulo = db.Movimentacao.Find(cod);

                if (!titulo.Boleto)
                {
                    response = new
                    {
                        success = false,
                        messages = "Desculpe mas uma das movimentações selecionadas não é um BOLETO! Cod: " + titulo.CodigoDoTitulo 

                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                if (titulo.DataDoVencimento >= DateTime.Now)
                {
                    response = new
                    {
                        success = false,
                        messages = "Você não pode atualizar um Boleto que não está vencido!"

                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }                

                if (titulo.DataDoRecebimento != null)
                {
                    response = new
                    {
                        success = false,
                        messages = "Você não pode atualizar um Boleto que já foi pago!"

                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                if (titulo.Cancelado)
                {
                    response = new
                    {
                        success = false,
                        messages = "Você não pode atualizar um Boleto Cancelado!"

                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                var dias = (DateTime.Today - titulo.DataDoVencimento).Days;
                
                    if (data > titulo.DataDoVencimento)
                    {
                        dias = (data - titulo.DataDoVencimento).Days;
                    }                

                var juros = (titulo.ValorDoTitulo * 2) / 100;
                var taxaMora = Convert.ToDecimal(0.0333);
                var mora = ((titulo.ValorDoTitulo * taxaMora) / 100) * dias;
                var total = (juros + mora);
                
                titulo.Observacao1 = "VALOR ORIGINAL: R$" + titulo.ValorDoTitulo;
                titulo.Observacao2 = "JUROS/CORREÇÃO: R$" + decimal.Round(total, 2);
                titulo.Observacao3 = "VCTO ORIGINAL: " + titulo.DataDoVencimento.ToShortDateString();
                titulo.ValorDoTitulo = titulo.ValorDoTitulo + decimal.Round(total, 2);
                titulo.DataDoVencimento = data;

                db.Entry(titulo).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AgruparBoleto(string agruparmovimentacao, string agruparData)
        {
            var response = new
            {
                success = true,
                messages = "Boletos Agrupados com Sucesso!"

            };

            if (string.IsNullOrEmpty(agruparData))
            {
                response = new
                {
                    success = false,
                    messages = "A Data de Vencimento não pode ser em Branco!"
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            var data = Convert.ToDateTime(agruparData);

            if (data < DateTime.Now)
            {
                response = new
                {
                    success = false,
                    messages = "A Data de Vencimento é inferior a Hoje!"

                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(agruparmovimentacao))
            {
                response = new
                {
                    success = false,
                    messages = "Selecione pelo menos dois Boletos!"

                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            
            int? codigoDoCliente = 0;

            string[] stringkeys = agruparmovimentacao.Split(',');
            int[] keys = stringkeys.Select(int.Parse).ToArray();

            //verifica mais de um titulo para agrupar
            if (keys.Count() <= 1)
            {
                response = new
                {
                    success = false,
                    messages = "Selecione pelo menos dois Boletos!"

                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            //verifica se o titulo é de um mesmo cliente
            foreach (var itens in db.Movimentacao.OrderBy(x => x.CodigoDoTitulo).Where(x => keys.Any(key => x.CodigoDoTitulo.Equals(key))))
            {                

                if (codigoDoCliente != itens.CodigoDoCliente && codigoDoCliente != 0)
                {
                    response = new
                    {
                        success = false,
                        messages = "Todos os Boletos devem ser de um mesmo Cliente!"

                    };
                    return Json(response, JsonRequestBehavior.AllowGet);                   
                }
                if (itens.Cancelado)
                {
                    response = new
                    {
                        success = false,
                        messages = "Um dos Boletos Selecionados está Cancelado!"

                    };
                    return Json(response, JsonRequestBehavior.AllowGet);                    
                }
                codigoDoCliente = itens.CodigoDoCliente;
            }
            try
            {
                int conta = 1;
                int codigoNovoTitulo = 0;
                decimal somamovimentacao = 0;
                decimal boleto = db.Configuracoes.First().TaxaDoBoleto;

                foreach (var itens in db.Movimentacao.Where(x => keys.Any(key => x.CodigoDoTitulo.Equals(key))))
                {
                    //cria o novo titulo com valor 0
                    if (conta == 1)
                    {
                        cfcadminEntities dba = new cfcadminEntities();

                        Movimentacao novoTitulo = new Movimentacao
                        {
                            Boleto = true,
                            CodigoDoCliente = itens.CodigoDoCliente,
                            TipoDoTitulo = "Boleto (A" + keys.Count() + ")",
                            ValorDoTitulo = 0,
                            DataDoTitulo = DateTime.Today,
                            DataDoVencimento = Convert.ToDateTime(agruparData),
                            TipoDaBaixa = "Não Baixado",
                            Credito = true,
                            DescricaoDoTitulo = "COB.CFC.CLUB.AGRUP " + DateTime.Today.Month + "/" + DateTime.Today.Year + " - Cliente " + itens.CodigoDoCliente,
                            TotalParcelas = 1,
                            NumeroParcela = 1,
                            CodigoDoHistorico = 1
                        };
                        dba.Movimentacao.Add(novoTitulo);
                        dba.SaveChanges();

                        codigoNovoTitulo = novoTitulo.CodigoDoTitulo;

                        conta++;
                    }

                    cfcadminEntities dbb = new cfcadminEntities();
                    //altera movimentacao
                    var alteraTitulo = dbb.Movimentacao.Find(itens.CodigoDoTitulo);
                    alteraTitulo.TipoDoTitulo = "A(" + codigoNovoTitulo + ")";
                    alteraTitulo.Cancelado = true;
                    //soma titulo
                    somamovimentacao = somamovimentacao + (alteraTitulo.Rateio ? (alteraTitulo.ValorDoTitulo - boleto) : alteraTitulo.ValorDoTitulo);
                    dbb.Entry(alteraTitulo).State = EntityState.Modified;
                    dbb.SaveChanges();

                    //adiciona novo Demonstrativo
                    cfcadminEntities dbc = new cfcadminEntities();
                    var demonstrativo = new Demonstrativo
                    {
                        CodigoDaMovimentacao = codigoNovoTitulo,
                        TituloDoGrupo = itens.Caixa ? "CAIXA VEICULO" : itens.BoletoAdmin ? "GESTÃO ONLINE" : itens.Rateio ? "RATEIO" : "OUTROS",
                        ItemReferencia = "MOVIMENTAÇÃO " + itens.CodigoDoTitulo,
                        ItemDescricao = itens.DescricaoDoTitulo,
                        ItemValor = itens.ValorDoTitulo
                    };
                    dbc.Demonstrativo.Add(demonstrativo);
                    dbc.SaveChanges();
                }

                //Altera novo Titulo para adicionar o Valor
                cfcadminEntities dbd = new cfcadminEntities();
                var tituloNovo = dbd.Movimentacao.Find(codigoNovoTitulo);
                tituloNovo.ValorDoTitulo = somamovimentacao + boleto;

                dbd.Entry(tituloNovo).State = EntityState.Modified;
                dbd.SaveChanges();
            }
            catch
            {
                response = new
                {
                    success = false,
                    messages = "Algo de ERRADO aconteceu, procure o Administrador!"

                };
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        public ActionResult ParcelarBoleto(string titulos, DateTime vencimento, int parcelas = 0)
        {
            if (titulos == "")
            {
                TempData["Mensagem"] = "Selecione um Titulo!";
                return RedirectToAction("Index");
            }

            if (parcelas == 0)
            {
                TempData["Mensagem"] = "Selecione a quantidade de Parcelas!";
                return RedirectToAction("Index");
            }

            //if (parcelamentogerada.AddDays(7) > parcelamentovencimento)
            //{
            //    TempData["Mensagem"] = "Data de Vencimento deve ter mais que 7 dias da data de geração!"; ;
            //    return RedirectToAction("Index");
            //}

            int codigoDoTitulo = Convert.ToInt32(titulos);
            var itens = db.Movimentacao.Find(codigoDoTitulo);

            if (itens.Cancelado)
            {
                TempData["Mensagem"] = "O Titulo selecionado Está Cancelado!";
                return RedirectToAction("Index", "movimentacao");
            }

            decimal boleto = db.Configuracoes.First().TaxaDoBoleto;

            for (int i = 0; i < parcelas; i++)
            {
                cfcadminEntities dba = new cfcadminEntities();

                var parc = i + 1;

                        Movimentacao novoTitulo = new Movimentacao();
                        novoTitulo.DescricaoDoTitulo = "COB.CFC.GRUPO.PARCELAMENTO P(" + parc + "/" + parcelas + ")" + DateTime.Today.Month + "/" + DateTime.Today.Year + " - Cliente " + itens.CodigoDoCliente;
                        novoTitulo.CodigoDoCliente = itens.CodigoDoCliente;
                        //novoTitulo.TipoDoTitulo = "Boleto (P" + parc + "/" + parcelas + "-" + codigoDoTitulo + ")";
                        novoTitulo.NumeroParcela = parc;
                        novoTitulo.TotalParcelas = parcelas;
                        novoTitulo.ValorDoTitulo = ((itens.ValorDoTitulo - boleto) / parcelas) + boleto;
                        novoTitulo.DataDoTitulo = DateTime.Today;//parcelamentogerada;
                        novoTitulo.DataDoVencimento = i == 0 ? vencimento : vencimento.AddMonths(i);
                        novoTitulo.TipoDaBaixa = "Não Baixado";                       
                        dba.Movimentacao.Add(novoTitulo);
                        dba.SaveChanges();

                        //adiciona nova OS
                        //gpcgEntities dbc = new gpcgEntities();
                        //var novaOS = new OrdemDeServico();
                        //novaOS.CodigoDoTitulo = itens.CodigoDoTitulo;
                        //novaOS.Mensagem = itens.DescricaoDoTitulo;
                        //novaOS.Valor = itens.ValorDoTitulo;
                        //novaOS.NovoTitulo = novoTitulo.CodigoDoTitulo;

                        //dbc.OrdemDeServico.Add(novaOS);
                        //dbc.SaveChanges();
            }

            //itens.TipoDoTitulo = "P(" + parcelas +")";
            itens.Cancelado = true;
            db.Entry(itens).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Mensagem"] = "Titulo Parcelado com Sucesso!";
            return RedirectToAction("Index", "movimentacao");
        }

        public ActionResult ImprimirBoletos(string id)
       {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Titulo = "Selecione um Titulo";
                return View();
            }

            var stringkeys = id.Split(',');
            var keys = stringkeys.Select(int.Parse).ToArray();

            var titulo = "";
           //var count = 0;

            foreach (var itens in keys)
            {
                //count = count + 1;
                //var os = "";
                //imprime OS
                //ordens de serviço Agrupados ou Parcelados
                //var ordensDeServico = "";
                //foreach (var ordens in db.OrdemDeServico.Where(x => x.NovoTitulo == itens))
                //{
                //    ordensDeServico = ordensDeServico +
                //                      "* - " + ordens.Mensagem + "(R$ " + ordens.Valor + ")<br />";
                //}
                ////titulo = titulo + RenderViewToString(this.ControllerContext, "GerarOS", db.Titulos.Find(itens)) + "<div style='page-break-before: always'></div>";                               
                //var dadosOs = db.Movimentacao.Find(itens);
                //var dadosCliente = db.Clientes.Find(dadosOs.CodigoDoCliente);
                //string mesExtenso = new System.Globalization.CultureInfo("pt-BR").DateTimeFormat.GetMonthName(dadosOs.DataDoTitulo.Month);

                //var os = "<div style='font-family: Helvetica, sans-serif !important;'>" +
                //         "<img style='float: left;margin-right: 20px;' src='/img/logo.png' width='100px' />" +
                //         "<center><h4>CFC GRUPO - A segurança do seu CFC</h4>" +
                //         "<br />Avenida Eduardo Elias Zahran, 953 - Campo Grande/MS" +
                //         "<br />Telefone (67)3027-7640</center>" +
                //         "<center><br /><br /><strong>Ordem de Serviços nº " + dadosOs.CodigoDoTitulo + "</strong><hr></center>" +
                //         "<br />" +
                //         "<strong>Nome:</strong>&nbsp;" + dadosCliente.NomeCompletoDoCliente.ToUpper() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <strong>CPF/CNPJ:</strong>&nbsp;" + dadosCliente.CPFCNPJDoCliente + "<br  />" +
                //         "<strong>Endereço:</strong>&nbsp;" + dadosCliente.EnderecoDoCliente.ToUpper() + "&nbsp; &nbsp;&nbsp;&nbsp;Fone:&nbsp;" + dadosCliente.TelefoneResidencialDoCliente + "<br />" +
                //         "<strong>Cidade:</strong>&nbsp;" + dadosCliente.Cidades.NomeDaCidade.ToUpper() + "&nbsp;&nbsp;&nbsp;&nbsp; <strong>UF:</strong>&nbsp;" + dadosCliente.Estados.NomeDoEstado.ToUpper() + "&nbsp; &nbsp;&nbsp;&nbsp; <strong>CEP:</strong>&nbsp;" + dadosCliente.CEPDoCliente +
                //         "<hr><br /><div style='font-size: 11px;'>" +
                //         "1 - " + dadosOs.DescricaoDoTitulo + " <span style='float: right;'>R$ " + dadosOs.ValorDoTitulo + "</span>" +
                //         "<br /><br />" +
                //         ordensDeServico +
                //         "<hr><strong><span style='float: right;font-size: 15px;'>Total R$: " + dadosOs.ValorDoTitulo + "</span></strong>" +
                //         "</div>" +
                //         "<br /><br /> Campo Grande(MS), " + dadosOs.DataDoTitulo.Day + ", de " + mesExtenso + " de " + dadosOs.DataDoTitulo.Year +
                //         "<br /><br /><br /><br /><center>________________________________________<br />Assinatura</center>" +
                //         "</div><br>" +
                //         "<div style='page-break-before: always'></div>";
                //imprime boleto
                //titulo = titulo + os + Sicredi(itens) + ((count % 3 == 0) ? "<div style='page-break-before: always'></div>" : "");
                titulo = titulo + Sicredi(itens) + "<div style='page-break-before: always'></div>";
            }

            ViewBag.Titulo = titulo;
            return View();
        }

        public static String RenderViewToString(ControllerContext context, String viewPath, object model = null)
        {
            context.Controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(context, viewPath, null);
                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
            //ViewBag.s = RenderViewToString(this.ControllerContext, "Index", null);
        }
        
        public ActionResult Remessa()
        {
            return View();
        }

        public JsonResult ListaRemessa(jQueryDataTableParamModel param)
        {


            var data = (from x in db.Movimentacao.Where(x => !x.GeradoRemessa && x.GeradoBoleto && x.Boleto && !x.Cancelado).ToList()
                        let c = db.Clientes.Find(x.CodigoDoCliente)
                        select new
                        {
                            Codigo = x.CodigoDoTitulo,
                            Titulo = c?.NomeCompletoDoCliente.ToUpper() ?? "",
                            x.DataDoRecebimento,
                            x.DataDoVencimento,                            
                            x.BoletoAdmin,                            
                            x.DescricaoDoTitulo,
                            x.ValorDoTitulo
                        });

 
            var result = data;
            if (param.iDisplayLength != -1)
            {
                result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            }

            return Json(new
            {
                param.sEcho,
                iTotalRecords = db.Movimentacao.Count(x => x.GeradoBoleto && !x.Cancelado && !x.GeradoRemessa),
                iTotalDisplayRecords = data.Count(),
                aaData = from x in result
                         select new[]
                    {                        
                        x.Codigo.ToString(),
                        x.Titulo,
                        x.DescricaoDoTitulo?.ToUpper() ?? "", 
                        x.ValorDoTitulo.ToString(CultureInfo.CurrentCulture),
                        x.DataDoVencimento.ToShortDateString(),                                                
                        x.BoletoAdmin ? "S" : "N"                       
                    }
            },
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveRemessa(int id)
        {
            var response = new
            {
                success = true,
                messages = "Remessa removida da lista!"

            };

            try
            {
                var titulo = db.Movimentacao.Find(id);
                titulo.GeradoBoleto = false;
                db.Entry(titulo).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                response = new
                {
                    success = false,
                    messages = "Algo de ERRADO aconteceu, procure o Administrador!"

                };

            }

            return Json(response, JsonRequestBehavior.AllowGet);            
        }

        public ActionResult GerarCaixa()
        {
            var lista = (from x in db.Clientes.Where(x => x.ClienteAtivo)
                         select new
                         {
                             x.CodigoDoCliente,
                             descricao = x.CodigoDoCliente + " " + x.NomeCFC.ToUpper() + " | " + x.Cidades.NomeDaCidade.ToUpper() + "/" + x.Estados.NomeDoEstado
                         }).ToList();
            ViewBag.Cliente = new SelectList(lista, "CodigoDoCliente", "descricao");
            ViewBag.Itens = new SelectList(db.Itens.Where(x => x.Placa1DoVeiculo == "0"), "CodigoDocliente", "Placa1DoVeiculo");
            return View();
        }

        public ActionResult ListaItens(int codigoDoCliente)
        {
            var lista = (from x in db.Itens.Where(x => x.CodigoDocliente == codigoDoCliente && x.DataDaDesativacao == null)
                         select new
                         {
                             x.CodigoDoItem,
                             descricao = x.CodigoDoItem + " " + x.Placa1DoVeiculo.ToUpper() + " - " + x.DescricaoDoModelo.ToUpper() + " (" + x.Valor / 10000 + ")"
                         }).ToList();
            ViewBag.Itens = new SelectList(lista, "CodigoDoItem", "descricao");
            var itens = db.Itens.Where(x => x.CodigoDocliente == codigoDoCliente);
            ViewBag.Todos = "ITENS " + itens.Count() + " | ATIVOS " + itens.Count(x => x.DataDaDesativacao == null) + " - COTAS " + $"{(itens.Where(x => x.DataDaDesativacao == null).Sum(x => x.Valor) / 10000):N4}" + " | INATIVOS " + itens.Count(x => x.DataDaDesativacao != null) + " - COTAS " + $"{itens.Where(x => x.DataDaDesativacao != null).Sum(x => x.Valor) / 10000:N4}";
            return View();
        }

        [HttpPost]
        public PartialViewResult CaixaGerado(int cliente, int? item, string data, int parcelas)
        {
            var itens = db.Itens.AsQueryable();
            itens = item == null ? itens.Where(x => x.CodigoDocliente == cliente && x.DataDaDesativacao == null) : itens.Where(x => x.CodigoDoItem == item && x.DataDaDesativacao == null);
            var retorno = "";
            var valor = 0;
            var vencimento = Convert.ToDateTime(data);
            if (!itens.Any()) return PartialView();
            var contaMes = 0;
            foreach (var i in itens)
            {
                var calculo = i.Valor / 10000;
                var valorItem = 0;

                if (calculo <= 4)
                {
                    valor = valor + 200;
                    valorItem = 200;
                }
                if (calculo > 4 && calculo <= 8)
                {
                    valor = valor + 400;
                    valorItem = 400;
                }
                if (calculo > 8 && calculo <= 12)
                {
                    valor = valor + 800;
                    valorItem = 800;
                }
                if (calculo > 12 && calculo <= 16)
                {
                    valor = valor + 1200;
                    valorItem = 1200;
                }
                if (calculo > 16 && calculo <= 20)
                {
                    valor = valor + 1600;
                    valorItem = 1600;
                }

  
                var placa = i.Placa1DoVeiculo;
                var parcela = "";
                //decimal boleto = db.Configuracoes.First().TaxaDoBoleto;
                for (var p = 1; p <= parcelas; p++)
                {
                    var movimentacao = new Movimentacao
                    {
                        Boleto = true,
                        CodigoDoCliente = itens.FirstOrDefault().CodigoDocliente,
                        TipoDoTitulo = "Boleto",
                        ValorDoTitulo = parcelas > 1 ? valorItem / parcelas : valorItem,
                        DataDoTitulo = DateTime.Today,
                        DataDoVencimento = vencimento.AddMonths(contaMes),
                        TipoDaBaixa = "Não Baixado",
                        Credito = true,
                        DescricaoDoTitulo = "CAIXA VEICULO  " + placa + (parcelas > 1 ? " (PARCELA " + p + "/" + parcelas + ")" : ""),
                        TotalParcelas = parcelas,
                        NumeroParcela = p,
                        Caixa = true,
                        CodigoDoHistorico = 1
                    };
                    var dbx = new cfcadminEntities();
                    dbx.Movimentacao.Add(movimentacao);
                    dbx.SaveChanges();
                    parcela = parcela + (parcelas > 1 ? " (PARCELA " + p + "/" + parcelas + ") | Valor: " + $"{movimentacao.ValorDoTitulo:N2}" + " | Vecimento: " + movimentacao.DataDoVencimento.ToShortDateString() + "<br>" : "");
                    contaMes = contaMes + 1;
                }
                retorno = retorno + "Item: " + i.Placa1DoVeiculo + " | Valor: R$" + $"{i.Valor:N2}" + " | Cotas: " + $"{i.Valor / 10000:N4}" + " | Caixa: R$" + $"{valorItem:N2}" + " | Parcela(s): " + parcelas + " | " + (parcelas == 1 ? "Vencimento: " + vencimento.AddMonths(contaMes - 1).ToShortDateString() : "<br>" + parcela) + "<br>";
            }

            var cli = itens.FirstOrDefault();
            ViewBag.Retorno =
                "<h5>Cliente: " + cli.Clientes.NomeCFC + " | "
                + cli.Clientes.Cidades.NomeDaCidade + "/" + cli.Clientes.Estados.NomeDoEstado + "</h5>" + retorno + "<h5>Parcelas: " + parcelas + " | Total: R$" + $"{valor:N2}" + "</h5>";
            return PartialView();
        }

        public ActionResult GerarGestao()
        {
            //var lista = (from x in db.Clientes.Where(x => x.ClienteAtivo)
            //             let i = db.Itens.Where(a => a.CodigoDocliente == x.CodigoDoCliente && a.DataDaDesativacao == null)
            //             let c = db.Configuracoes.FirstOrDefault()
            //             select new
            //             {
            //                 x.CodigoDoCliente,
            //                 descricao = x.CodigoDoCliente + " " + x.NomeCFC.ToUpper() + " | " + x.Cidades.NomeDaCidade.ToUpper() + "/" + x.Estados.NomeDoEstado + " | Itens:" + i.Count() + " | Cotas: "+ i.Sum(b => b.Valor)/10000 +" |  Valor: " + i.Count() * (c.ValorDaMensalidade + c.ValorTerceiro)
            //             }).ToList();

            var config = db.Configuracoes.FirstOrDefault();
            ViewBag.Gestao = $"{config.ValorDaMensalidade:N2}";
            ViewBag.Terceiro = $"{config.ValorTerceiro:N2}";

            //var clientes = db.Clientes.Where(x => x.ClienteAtivo);
            //var itens = db.Itens.Where(x => x.DataDaDesativacao == null);           
            //ViewBag.Cliente = new SelectList(lista, "CodigoDoCliente", "descricao");

            //ViewBag.Todos = "-- GERAR TODOS " + clientes.Count() + " | ITENS ATIVOS " + itens.Count() + " | COTAS " + $"{(itens.Sum(x => x.Valor) / 10000):N4}" + " | TOTAL R$ " + $"{itens.Count() * (config.ValorDaMensalidade + config.ValorTerceiro):N2}"+ " --";

            

            //ViewBag.sinistros = new SelectList(lista, "CodigoDoSinistro", "descricao");

            return View();
        }

        [HttpPost]
        public string GestaoGerada(DateTime vencimento, string[] cliente, string[] sinistros, string sinistro, int parcelas = 1)
        {
            if (cliente[0] == "") return "<strong style='color:red'>Selecione um Cliente ou Todos!</strong>";
            if (Convert.ToBoolean(sinistro) && sinistros[0] == "")
            {
                return "<strong style='color:red'>Você optou por adicionar o Rateio a GESTÃO selecione ao menos um Sinistro!</strong>";
            }
            

            var stringkeysItens = cliente; //.Split(',');           
            if (cliente.All(x => x != "0") && Convert.ToBoolean(sinistro))
            {
                return "<strong style='color:red'>Desculpe a opção RATEIO só está disponivel na opção -- TODOS OS CLIENTES --!</strong>";
            }

            if (Convert.ToBoolean(sinistro))
            {
                var stringkeysSinistros = sinistros; //.Split(',');
                var keysSinistros = stringkeysSinistros.Select(int.Parse).ToArray();

                foreach (var sin in keysSinistros)
                {
                    if (db.Sinistros.Find(sin).ValorDoSinistro == 0)
                    {
                        return "<strong style='color:red'>Sinistro nº "+sin+" está com o valor ZERADO! verifique se existem lançamentos para compor o valor na Movimentação VERIFIQUE!</strong>";
                    }
                    
                }
            }

            var keysItens = stringkeysItens.Select(int.Parse).ToArray();            

            var itens = db.Itens.AsQueryable();
            itens = cliente.Any(x => x == "0") ? itens : itens.Where(x => keysItens.Any(key => x.CodigoDoItem.Equals(key)));

            var lista = itens.GroupBy(item => item.CodigoDocliente)
                .Select(group => new { Cliente = group.Key, Itens = group.ToList(), Separa = group.FirstOrDefault().Clientes.SepararVeiculos }).ToList();
            
            if (!itens.Any()) return "<strong style='color:red'>ERR:1 Nenhum Cliente Selecionado!</strong>";
            //var contaMes = 0;

            var valores = db.Configuracoes.First();
            var retorno = "";

            foreach (var i in lista)
            {
                //separa boletos por item
                if (i.Separa)
                {
                    foreach (var s in i.Itens.Where(x => x.CodigoDocliente == i.Cliente))
                    {
                        for (var p = 1; p <= parcelas; p++)
                        {
                            var movimentacao = new Movimentacao
                            {
                                Boleto = true,
                                CodigoDoCliente = i.Cliente,
                                TipoDoTitulo = "Boleto",
                                //ValorDoTitulo = parcelas > 1 ? i.Total / parcelas : i.Total,
                                ValorDoTitulo = 0,
                                DataDoTitulo = DateTime.Today,
                                DataDoVencimento = parcelas == 1 ? vencimento : vencimento.AddMonths(p - 1),
                                TipoDaBaixa = "Não Baixado",
                                Credito = true,
                                //DescricaoDoTitulo = i.TextoGestao.ToUpper() + " R$" + $"{i.Gestao:N2}" + " | " + i.TextoTerceiro.ToUpper() + " R$" + $"{i.Terceiro:N2}" + " | VEICULO(S) " + i.Veiculos + (parcelas > 1 ? " (PARCELA " + p + "/" + parcelas + ")" : ""),
                                DescricaoDoTitulo = "BOLETO ",
                                TotalParcelas = parcelas,
                                //NumeroParcela = p,
                                NumeroParcela = 1,
                                //Caixa = false,
                                //BoletoAdmin = true,
                                CodigoDoHistorico = 1
                            };
                            var dbx = new cfcadminEntities();
                            dbx.Movimentacao.Add(movimentacao);
                            dbx.SaveChanges();

                            //var itensLista = i.Itens.Where(x => x.CodigoDocliente == i.Cliente && x.DataDaDesativacao == null);

                            //if (itensLista.Any())
                            //{
                            var gestao = new Demonstrativo
                            {
                                CodigoDaMovimentacao = movimentacao.CodigoDoTitulo,
                                TituloDoGrupo = "GESTÃO ON-LINE ",
                                ItemDescricao = "PLACAS " + s.Placa1DoVeiculo,
                                ItemReferencia = "VEICULOS " + "1",
                                ItemValor = valores.ValorDaMensalidade
                            };
                            dbx.Demonstrativo.Add(gestao);

                            var terceriro = new Demonstrativo
                            {
                                CodigoDaMovimentacao = movimentacao.CodigoDoTitulo,
                                TituloDoGrupo = "COBERTURA  DE TERCEIRO",
                                ItemDescricao = "PLACAS " + s.Placa1DoVeiculo,
                                ItemReferencia = "VEICULOS " + "1",
                                ItemValor = valores.ValorTerceiro
                            };
                            dbx.Demonstrativo.Add(terceriro);

                            movimentacao.DescricaoDoTitulo = movimentacao.DescricaoDoTitulo + " GESTÃO ";
                            //}

                            if (Convert.ToBoolean(sinistro))
                            {
                                //var dba = new cfcadminEntities();

                                var stringkeysSinistros = sinistros; //.Split(',');
                                var keysSinistros = stringkeysSinistros.Select(int.Parse).ToArray();

                                var dadosDoSinsitro = db.DadosDoSinistro
                                    .Where(x => keysSinistros.Any(key => x.CodigoDoSinistro.Equals(key)) &&
                                                x.CodigoDoCliente == i.Cliente)//.GroupBy(item => item.CodigoDoCliente)
                                    .ToList();
                                    //.Select(group => new
                                    //{
                                    //    Cliente = group.Key,
                                        //Itens = group.ToList(),                                        
                                     //   Sinistro = group.FirstOrDefault()
                                    //}).ToList();

                                foreach (var sin in dadosDoSinsitro)
                                {                                    
                                    var detalhesSinistro = db.Sinistros.Find(sin.CodigoDoSinistro);                                    
                                    if (sin.CodigoDoItem != s.CodigoDoItem) continue;
                                    var rateio = new Demonstrativo
                                    {
                                        CodigoDaMovimentacao = movimentacao.CodigoDoTitulo,
                                        TituloDoGrupo = "RATEIO | SINISTRO nº " + detalhesSinistro.CodigoDoSinistro + " | TOTAL DE COTAS NA DATA " + detalhesSinistro.CotasNaData + " | VALOR POR COTA " + $"{detalhesSinistro.ValorPorCota:N2}" + (parcelas == 1 ? "" : " | PARCELA " + p + "/" + parcelas),
                                        ItemDescricao = "PLACA NA DATA " + s.Placa1DoVeiculo,
                                        ItemReferencia = "COTAS NA DATA " + $"{s.Valor / 10000:N4}",
                                        ItemValor = parcelas == 1 ? s.Valor / 10000 * detalhesSinistro.ValorPorCota : (s.Valor / 10000 * detalhesSinistro.ValorPorCota) / parcelas
                                    };
                                    dbx.Demonstrativo.Add(rateio);                                                                  
                                }
                                movimentacao.DescricaoDoTitulo = movimentacao.DescricaoDoTitulo + "RATEIO";
                            }

                            dbx.SaveChanges();

                            //altera movimentacao para somar o valor
                            //remove movimentacao se não existir altera o valor se existir
                            var demonstrativo = db.Demonstrativo.Where(x => x.CodigoDaMovimentacao == movimentacao.CodigoDoTitulo);
                            if (demonstrativo.Any())
                            {
                                //var mov = db.Movimentacao.Find(movimentacao.CodigoDoTitulo);
                                movimentacao.ValorDoTitulo = db.Demonstrativo.Where(x => x.CodigoDaMovimentacao == movimentacao.CodigoDoTitulo).Sum(x => x.ItemValor) + valores.TaxaDoBoleto ?? 0;
                                dbx.Entry(movimentacao).State = EntityState.Modified;
                                dbx.SaveChanges();

                                //imprime lista de retornos
                                var clientesRetorno = demonstrativo.GroupBy(item => item.CodigoDaMovimentacao)
                                    .Select(group => new { Cliente = group.Key, Itens = group.ToList() }).ToList();

                                foreach (var demo in clientesRetorno)
                                {
                                    var clienteDescricao = db.Clientes.Find(i.Cliente);
                                    retorno = retorno + "<br><strong>" + i.Cliente + " " + clienteDescricao.NomeCFC +
                                              " - " + clienteDescricao.Cidades.NomeDaCidade + "/" +
                                              clienteDescricao.Estados.NomeDoEstado + "</strong> Total R$ " +
                                              $"{demo.Itens.Sum(x => x.ItemValor):N2}" + " | Vencimento " +
                                              movimentacao.DataDoVencimento.ToShortDateString() + "<br>";

                                    foreach (var item in demo.Itens)
                                    {
                                        retorno = retorno + item.TituloDoGrupo + " > " + item.ItemDescricao + " - " +
                                                  item.ItemReferencia + " | " + $"{item.ItemValor:N2}" + "<br>";
                                    }
                                }
                            }
                            else
                            {
                                dbx.Movimentacao.Remove(movimentacao);
                                dbx.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    //modo normal cliente não separa
                    for (var p = 1; p <= parcelas; p++)
                    {
                        var movimentacao = new Movimentacao
                        {
                            Boleto = true,
                            CodigoDoCliente = i.Cliente,
                            TipoDoTitulo = "Boleto",
                            //ValorDoTitulo = parcelas > 1 ? i.Total / parcelas : i.Total,
                            ValorDoTitulo = 0,
                            DataDoTitulo = DateTime.Today,
                            DataDoVencimento = parcelas == 1 ? vencimento : vencimento.AddMonths(p - 1),
                            TipoDaBaixa = "Não Baixado",
                            Credito = true,
                            //DescricaoDoTitulo = i.TextoGestao.ToUpper() + " R$" + $"{i.Gestao:N2}" + " | " + i.TextoTerceiro.ToUpper() + " R$" + $"{i.Terceiro:N2}" + " | VEICULO(S) " + i.Veiculos + (parcelas > 1 ? " (PARCELA " + p + "/" + parcelas + ")" : ""),
                            DescricaoDoTitulo = "BOLETO ",
                            TotalParcelas = parcelas,
                            //NumeroParcela = p,
                            NumeroParcela = 1,
                            //Caixa = false,
                            //BoletoAdmin = true,
                            CodigoDoHistorico = 1
                        };
                        var dbx = new cfcadminEntities();
                        dbx.Movimentacao.Add(movimentacao);
                        dbx.SaveChanges();

                        var itensLista = i.Itens.Where(x =>
                            x.CodigoDocliente == i.Cliente && x.DataDaDesativacao == null);

                        if (itensLista.Any())
                        {
                            var gestao = new Demonstrativo
                            {
                                CodigoDaMovimentacao = movimentacao.CodigoDoTitulo,
                                TituloDoGrupo = "GESTÃO ON-LINE ",
                                ItemDescricao =
                                    "PLACAS " + string.Join(", ", itensLista.Select(x => x.Placa1DoVeiculo)),
                                ItemReferencia = "VEICULOS " + itensLista.Count(),
                                ItemValor = itensLista.Count() * valores.ValorDaMensalidade
                            };
                            dbx.Demonstrativo.Add(gestao);
                            dbx.SaveChanges();

                            var terceriro = new Demonstrativo
                            {
                                CodigoDaMovimentacao = movimentacao.CodigoDoTitulo,
                                TituloDoGrupo = "COBERTURA  DE TERCEIRO",
                                ItemDescricao = "PLACAS " + string.Join(", ", itensLista.Select(x => x.Placa1DoVeiculo)),
                                ItemReferencia = "VEICULOS " + itensLista.Count(),
                                ItemValor = itensLista.Count() * valores.ValorTerceiro
                            };
                            dbx.Demonstrativo.Add(terceriro);
                            dbx.SaveChanges();

                            movimentacao.DescricaoDoTitulo = movimentacao.DescricaoDoTitulo + " GESTÃO ";
                        }

                        if (Convert.ToBoolean(sinistro))
                        {
                            var dba = new cfcadminEntities();

                            var stringkeysSinistros = sinistros; //.Split(',');
                            var keysSinistros = stringkeysSinistros.Select(int.Parse).ToArray();

                            //var dadosDoSinsitro = db.DadosDoSinistro
                            //    .Where(x => keysSinistros.Any(key => x.CodigoDoSinistro.Equals(key)) &&
                            //                x.CodigoDoCliente == i.Cliente).GroupBy(item => item.CodigoDoCliente)
                            //    .Select(group => new
                            //    {
                            //        Cliente = group.Key,
                            //        Itens = group.ToList(),
                            //        Sinistro = group.FirstOrDefault()
                            //    }).ToList();

                            //var dadosDoSinsitro = db.DadosDoSinistro.Where(x => keysSinistros.Any(key => x.CodigoDoSinistro.Equals(key)) && x.CodigoDoCliente == i.Cliente).Distinct().ToList();//.GroupBy(item => item.CodigoDoCliente)

                            foreach (var sin in keysSinistros)
                            {
                                var dadosDoSinsitro = db.DadosDoSinistro.Where(x => x.CodigoDoSinistro == sin && x.CodigoDoCliente == i.Cliente);
                                if (dadosDoSinsitro.Any())
                                { 
                                    var detalhesSinistro = dba.Sinistros.Find(sin); //dba.Sinistros.Find(sin.Sinistro.CodigoDoSinistro);

                                    var placas = dadosDoSinsitro.Join(db.Itens, x => x.CodigoDoItem, d => d.CodigoDoItem,
                                        (x, d) => new
                                        {
                                            d.Placa1DoVeiculo,
                                        });

                                    try { 
                                        var rateio = new Demonstrativo
                                        {
                                            CodigoDaMovimentacao = movimentacao.CodigoDoTitulo,
                                            TituloDoGrupo = "RATEIO | SINISTRO nº " + sin + " | TOTAL DE COTAS NA DATA " + detalhesSinistro.CotasNaData + " | VALOR POR COTA " + $"{detalhesSinistro.ValorPorCota:N2}" + (parcelas == 1 ? "" : " | PARCELA " + p + "/" + parcelas),
                                            ItemDescricao = "PLACAS NA DATA " + string.Join(", ", placas.Select(x => x.Placa1DoVeiculo)), //"PLACAS NA DATA " + string.Join(", ", placas.Select(x => x.Placa1DoVeiculo)),
                                            ItemReferencia = "COTAS NA DATA " + $"{dadosDoSinsitro.Sum(x => x.ValorDoItem) / 10000:N4}",
                                            ItemValor = parcelas == 1 ? dadosDoSinsitro.Sum(x => x.ValorDoItem) / 10000 * detalhesSinistro.ValorPorCota : (dadosDoSinsitro.Sum(x => x.ValorDoItem) / 10000 * detalhesSinistro.ValorPorCota) / parcelas
                                        };
                                        dbx.Demonstrativo.Add(rateio);
                                        dbx.SaveChanges();
                                    }
                                    catch (DbEntityValidationException e)
                                    {
                                        var outputLines = new List<string>();
                                        foreach (var eve in e.EntityValidationErrors)
                                        {
                                            outputLines.Add($"{DateTime.Now}: Tipo da Entidade \"{eve.Entry.Entity.GetType().Name}\" no estado \"{eve.Entry.State}\" apresentou os seguintes erros de validação:");
                                            outputLines.AddRange(eve.ValidationErrors.Select(ve => $"- Propriedades: \"{ve.PropertyName}\", Erros: \"{ve.ErrorMessage}\""));
                                        }
                                        return retorno = outputLines.ToString();
                                        //throw;
                                    }
                                }
                                //detalhesSinistro.DataDoFaturamento = DateTime.Today;
                                //dba.Entry(detalhesSinistro).State = EntityState.Modified;
                                //dba.SaveChanges();
                            }
                            movimentacao.DescricaoDoTitulo = movimentacao.DescricaoDoTitulo + "RATEIO";
                        }
                        //try
                        //{
                        //    dbx.SaveChanges();
                        //}
                        //catch (DbEntityValidationException e)
                        //{
                        //    var outputLines = new List<string>();
                        //    foreach (var eve in e.EntityValidationErrors)
                        //    {
                        //        outputLines.Add($"{DateTime.Now}: Tipo da Entidade \"{eve.Entry.Entity.GetType().Name}\" no estado \"{eve.Entry.State}\" apresentou os seguintes erros de validação:");
                        //        outputLines.AddRange(eve.ValidationErrors.Select(ve => $"- Propriedades: \"{ve.PropertyName}\", Erros: \"{ve.ErrorMessage}\""));
                        //    }
                        //    var erro = outputLines.ToString();
                        //    throw;
                        //}

                        //altera movimentacao para somar o valor
                        //remove movimentacao se não existir altera o valor se existir
                        var demonstrativo =
                            db.Demonstrativo.Where(x => x.CodigoDaMovimentacao == movimentacao.CodigoDoTitulo);
                        if (demonstrativo.Any())
                        {
                            //var mov = db.Movimentacao.Find(movimentacao.CodigoDoTitulo);
                            movimentacao.ValorDoTitulo =
                                db.Demonstrativo.Where(x => x.CodigoDaMovimentacao == movimentacao.CodigoDoTitulo)
                                    .Sum(x => x.ItemValor) + valores.TaxaDoBoleto ?? 0;
                            dbx.Entry(movimentacao).State = EntityState.Modified;
                            dbx.SaveChanges();

                            //imprime lista de retornos
                            var clientesRetorno = demonstrativo.GroupBy(item => item.CodigoDaMovimentacao)
                                .Select(group => new {Cliente = group.Key, Itens = group.ToList()}).ToList();

                            foreach (var demo in clientesRetorno)
                            {
                                var clienteDescricao = db.Clientes.Find(i.Cliente);
                                retorno = retorno + "<br><strong>" + i.Cliente + " " + clienteDescricao.NomeCFC +
                                          " - " + clienteDescricao.Cidades.NomeDaCidade + "/" +
                                          clienteDescricao.Estados.NomeDoEstado + "</strong> Total R$ " +
                                          $"{demo.Itens.Sum(x => x.ItemValor):N2}" + " | Vencimento " +
                                          movimentacao.DataDoVencimento.ToShortDateString() + "<br>";

                                foreach (var item in demo.Itens)
                                {
                                    retorno = retorno + item.TituloDoGrupo + " > " + item.ItemDescricao + " - " +
                                              item.ItemReferencia + " | " + $"{item.ItemValor:N2}" + "<br>";
                                }
                            }
                        }
                        else
                        {
                            dbx.Movimentacao.Remove(movimentacao);
                            dbx.SaveChanges();
                        }
                    }
                }
            }

            if (!Convert.ToBoolean(sinistro)) return retorno;
            {
                var stringkeysSinistros = sinistros; //.Split(',');
                var keysSinistros = stringkeysSinistros.Select(int.Parse).ToArray();

                foreach (var sin in keysSinistros)
                {
                    var sini = db.Sinistros.Find(sin);
                    sini.DataDoFaturamento = DateTime.Today;
                    db.Entry(sini).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            return retorno;
        }

        public JsonResult ListaGestao(jQueryDataTableParamModel param)
        {
            var data = (from x in db.Clientes.Where(x => x.ClienteAtivo).ToList()
                let i = db.Itens.Where(a => a.CodigoDocliente == x.CodigoDoCliente && a.DataDaDesativacao == null)
                let c = db.Configuracoes.FirstOrDefault()
                select new
                {
                    Codigo = x.CodigoDoCliente,
                    NomeCFC = x.NomeCFC + " | " + x.Cidades.NomeDaCidade + "/" + x.Estados.NomeDoEstado,
                    Itens = i.Count(),
                    Cotas = i.Sum(b => b.Valor) / 10000,
                    ValorGestao = i.Count() * (c.ValorDaMensalidade + c.ValorTerceiro)
                });

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                data = data.Where(c => c.NomeCFC.ToUpper().Contains(param.sSearch));
            }

            var result = data;
            if (param.iDisplayLength != -1)
            {
                result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            }

            return Json(new
                {
                    param.sEcho,
                    iTotalRecords = db.Clientes.Count(x => x.ClienteAtivo),
                    iTotalDisplayRecords = data.Count(),
                    aaData = from x in result
                    select new[]
                    {
                        string.Empty,
                        x.Codigo.ToString(),
                        x.NomeCFC,
                        x.Itens.ToString(),
                        x.Cotas.ToString(),
                        x.ValorGestao.ToString(CultureInfo.CurrentCulture)
                    }
                },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult GerarRateio()
        {
            var lista = (from x in db.Sinistros.Where(x => x.DataDeConclusaoDoSinistro != null && x.DataDoFaturamento == null).OrderByDescending(x => x.CodigoDoSinistro)
                         join i in db.Itens on x.CodigoDoItem equals i.CodigoDoItem
                select new
                {
                    x.CodigoDoSinistro,
                    descricao = x.CodigoDoSinistro + " " + i.Placa1DoVeiculo + " - " + i.DescricaoDoModelo + " | " + i.Clientes.NomeCFC.ToUpper() + " - " + i.Clientes.Cidades.NomeDaCidade.ToUpper() + "/" + i.Clientes.Estados.NomeDoEstado
                }).ToList();

            ViewBag.Sinistros = new SelectList(lista, "CodigoDoSinistro", "descricao");
            return View();
        }

        [HttpPost]
        public string GerarRateio(string[] sinistros, string vencimento, string gerado)
        {
            //int codigoDoSinistro = dados["Sinistros"] == "" ? 0 : Convert.ToInt32(dados["Sinistros"]);
            if (sinistros[0] == "") return "Selecione pelo menos um Sinistro";                
            
            if (vencimento == "") return "Data de Vencimento é Obrigatória";
            if (gerado == "") return "Data de Geração é Obrigatória";

            var dataGeracao = Convert.ToDateTime(gerado);
            var dataVencimento = Convert.ToDateTime(vencimento);

            var stringkeys = sinistros; //.Split(',');
            var keys = stringkeys.Select(int.Parse).ToArray();

            if (dataGeracao < dataVencimento) return "Data de geração maior que a data de vencimento!";

            cfcadminEntities dbx = new cfcadminEntities();

            var dadosDoSinistro = db.DadosDoSinistro.Where(x => keys.Any(key => x.CodigoDoSinistro.Equals(key)));

            var lista = (from s in dadosDoSinistro.DistinctBy(x => x.CodigoDoCliente)
                join c in db.Clientes on s.CodigoDoCliente equals c.CodigoDoCliente
                let dsa = dadosDoSinistro.Where(x => x.CodigoDoCliente == s.CodigoDoCliente)
                let mv = db.Movimentacao
                let dsb = db.Sinistros.Where(z => dadosDoSinistro.Where(x => x.CodigoDoCliente == s.CodigoDoCliente).Select(x => x.CodigoDoSinistro).Any(x => x.Equals(z.CodigoDoSinistro))).Select(x => new
                {
                    Soma = db.DadosDoSinistro.Where(a => a.CodigoDoCliente == s.CodigoDoCliente && a.CodigoDoSinistro == x.CodigoDoSinistro).Sum(a => a.ValorDoItem) / 10000 * (x.ValorDoSinistro / x.CotasNaData) //mv.Where(b => b.CodigoDoSinistro == x.CodigoDoSinistro).Sum(b => b.ValorDoTitulo)) / x.CotasNaData
                }).Sum(d => d.Soma)
                select new
                {
                    s.CodigoDoCliente,
                    NomeCFC = s.CodigoDoCliente + " " + c.NomeCFC + " - " + c.Cidades.NomeDaCidade + "/" + c.Estados.NomeDoEstado,
                    DescricaoDoTitulo = "COB." + dataGeracao.Month + "/" + dataGeracao.Year + " - REF.SINISTRO(S) ",
                    ValorDoTitulo = dsb,
                    Sinistro = dsa.DistinctBy(x => x.CodigoDoSinistro)
                });

            var emp = db.Configuracoes.FirstOrDefault();
            var titulosGerados = "<strong>Títulos de Cobrança gerados</strong><br>Taxa Boleto R$ " + $"{emp.TaxaDoBoleto:N2}" +"<br><br>";

            foreach (var itens in lista)
            {
                var codSin = string.Join(",", itens.Sinistro.Select(x => x.CodigoDoSinistro.ToString()));

                var tit = new Movimentacao
                {
                    DescricaoDoTitulo = itens.DescricaoDoTitulo + codSin,
                    CodigoDoCliente = itens.CodigoDoCliente,
                    TipoDoTitulo = "Boleto",
                    ValorDoTitulo = (itens.ValorDoTitulo ?? 0) + emp.TaxaDoBoleto,
                    DataDoTitulo = dataGeracao,
                    DataDoVencimento = dataVencimento,
                    TipoDaBaixa = "Não Baixado",
                    NumeroParcela = 1,
                    TotalParcelas = 1,
                    CodigoDoHistorico = 1,
                    Boleto = true,
                    Rateio = true,
                    Credito = true
                };
                dbx.Movimentacao.Add(tit);
                //dbx.SaveChanges();

                titulosGerados = titulosGerados + "<strong>> " + itens.NomeCFC.ToUpper() + "</strong> - Sinitro(s) " + codSin + " - Boleto Gerado R$ " + $"{((itens.ValorDoTitulo) + emp.TaxaDoBoleto):N2}" + "<br>";
            }

            //var dba = new gpcgEntities();
            foreach (var itens in keys)
            {
                Sinistros sinistro = dbx.Sinistros.Find(itens);
                //sinistro.Processado = true;
                sinistro.DataDoFaturamento = dataGeracao;
                dbx.Entry(sinistro).State = EntityState.Modified;
                //db.SaveChanges();
            }
            dbx.SaveChanges();
            return titulosGerados;
        }

        public string Ocorrencia(int codigo)
        {
            switch (codigo)
            {
                case 2:
                    return "02-Entrada Confirmada";
                case 3:
                    return "03-Entrada Rejeitada";
                case 6:
                    return "06-Liquidação normal";
                case 9:
                    return "09-Baixado Automaticamente via Arquivo";
                case 10:
                    return "10-Baixado conforme instruções da Agência";
                case 11:
                    return "11-Em Ser - Arquivo de Títulos pendentes";
                case 12:
                    return "12-Abatimento Concedido";
                case 13:
                    return "13-Abatimento Cancelado";
                case 14:
                    return "14-Vencimento Alterado";
                case 15:
                    return "15-Liquidação em Cartório";
                case 17:
                    return "17-Liquidação após baixa ou Título não registrado";
                case 18:
                    return "18-Acerto de Depositária";
                case 19:
                    return "19-Confirmação Recebimento Instrução de Protesto";
                case 20:
                    return "20-Confirmação Recebimento Instrução Sustação de Protesto";
                case 21:
                    return "21-Acerto do Controle do Participante";
                case 23:
                    return "23-Entrada do Título em Cartório";
                case 24:
                    return "24-Entrada rejeitada por CEP Irregular";
                case 27:
                    return "27-Baixa Rejeitada";
                case 28:
                    return "28-Débito de tarifas/custas";
                case 30:
                    return "30-Alteração de Outros Dados Rejeitados";
                case 32:
                    return "32-Instrução Rejeitada";
                case 33:
                    return "33-Confirmação Pedido Alteração Outros Dados";
                case 34:
                    return "34-Retirado de Cartório e Manutenção Carteira";
                case 35:
                    return "35-Aceite do Pagador";
                case 68:
                    return "68-Acerto dos dados ) rateio de Crédito";
                case 69:
                    return "69-Cancelamento dos dados ) rateio";
                default:
                    return "";
            }
        }

        public string Tarifa(string codigo)
        {
            switch (codigo)
            {
                case "03":
                    return "03-Tarifa de sustação";
                case "04":
                    return "04-Tarifa de protesto";
                case "08":
                    return "08-Tarifa de custas de protesto";
                case "A9":
                    return "A9-Tarifa de manutenção de título vencido";
                case "B1":
                    return "B1-Tarifa de baixa da carteira";
                case "B3":
                    return "B3-Tarifa de registro de entrada do título";
                case "F5":
                    return "F5-Tarifa de entrada na rede Sicredi";
                default:
                    return "";
            }
        }

        public string MotivoRejeicao(string codigo)
        {
            switch (codigo)
            {
                case "00":
                    return "00-Processado";
                case "01":
                    return "01-Código do banco inválido";
                case "02":
                    return "02-Código do registro detalhe inválido";
                case "03":
                    return "03-Código da ocorrência inválido";
                case "04":
                    return "04-Código de ocorrência não permitida para a carteira";
                case "05":
                    return "05-Código de ocorrência não numérico";
                case "07":
                    return "07-Cooperativa/agência/conta/dígito inválidos";
                case "08":
                    return "08-Nosso número inválido";
                case "09":
                    return "09-Nosso número duplicado";
                case "10":
                    return "10-Carteira inválida 14 Título protestado";
                case "15":
                    return "15-Cooperativa/carteira/agência/conta/nosso número inválidos";
                case "16":
                    return "16-Data de vencimento inválida";
                case "17":
                    return "17-Data de vencimento anterior à data de emissão";
                case "18":
                    return "18-Vencimento fora do prazo de operação";
                case "20":
                    return "20-Valor do título inválido";
                case "21":
                    return "21-Espécie do título inválida";
                case "22":
                    return "22-Espécie não permitida para a carteira";
                case "24":
                    return "24-Data de emissão inválida";
                case "29":
                    return "29-Valor do desconto maior/igual ao valor do título";
                case "31":
                    return "31-Concessão de desconto - existe desconto anterior";
                case "33":
                    return "33-Valor do abatimento inválido";
                case "34":
                    return "34-Valor do abatimento maior/igual ao valor do título";
                case "36":
                    return "36-Concessão de abatimento - existe abatimento anterior";
                case "38":
                    return "38-Prazo para protesto inválido";
                case "39":
                    return "39-Pedido para protesto não permitido para o título";
                case "40":
                    return "40-Título com ordem de protesto emitida";
                case "41":
                    return "41-Pedido cancelamento/sustação sem instrução de protesto";
                case "44":
                    return "44-Cooperativa de crédito/agência beneficiária não prevista";
                case "45":
                    return "45-Nome do pagador inválido 46 Tipo/número de inscrição do pagador inválidos";
                case "47":
                    return "47-Endereço do pagador não informado";
                case "48":
                    return "48-CEP irregular";
                case "49":
                    return "49-Número de Inscrição do pagador/avalista inválido";
                case "50":
                    return "50-Pagador/avalista não informado";
                case "60":
                    return "60-Movimento para título não cadastrado";
                case "63":
                    return "63-Entrada para título já cadastrado A Aceito D Desprezado";
                case "A1":
                    return "A1-Praça do pagador não cadastrada.";
                case "A2":
                    return "A2-Tipo de cobrança do título divergente com a praça do pagador.";
                case "A3":
                    return "A3-Cooperativa/agência depositária divergente: atualiza o cadastro de praças da Coop./agência beneficiária";
                case "A4":
                    return "A4-Beneficiário não cadastrado ou possui CGC/CIC inválido";
                case "A5":
                    return "A5-Pagador não cadastrado";
                case "A6":
                    return "A6-Data da instrução/ocorrência inválida";
                case "A7":
                    return "A7-Ocorrência não pode ser comandada";
                case "A8":
                    return "A8-Recebimento da liquidação fora da rede Sicredi - via compensação eletrônica";
                case "B4":
                    return "B4-Tipo de moeda inválido";
                case "B5":
                    return "B5-Tipo de desconto/juros inválido";
                case "B6":
                    return "B6-Mensagem padrão não cadastrada";
                case "B7":
                    return "B7-Seu número inválido";
                case "B8":
                    return "B8-Percentual de multa inválido";
                case "B9":
                    return "B9-Valor ou percentual de juros inválido";
                case "C1":
                    return "C1-Data limite para concessão de desconto inválida";
                case "C2":
                    return "C2-Aceite do título inválido";
                case "C3":
                    return "C3-Campo alterado na instrução “31 – alteração de outros dados” inválido";
                case "C4":
                    return "C4-Título ainda não foi confirmado pela centralizadora";
                case "C5":
                    return "C5-Título rejeitado pela centralizadora";
                case "C6":
                    return "C6-Título já liquidado";
                case "C7":
                    return "C7-Título já baixado";
                case "C8":
                    return "C8-Existe mesma instrução pendente de confirmação para este título";
                case "C9":
                    return "C9-Instrução prévia de concessão de abatimento não existe ou não confirmada";
                case "D1":
                    return "D1-Título dentro do prazo de vencimento (em dia)";
                case "D2":
                    return "D2-Espécie de documento não permite protesto de título";
                case "D3":
                    return "D3-Título possui instrução de baixa pendente de confirmação";
                case "D4":
                    return "D4-Quantidade de mensagens padrão excede o limite permitido";
                case "D5":
                    return "D5-Quantidade inválida no pedido de boletos pré-impressos da cobrança sem registro";
                case "D6":
                    return "D6-Tipo de impressão inválida para cobrança sem registro";
                case "D7":
                    return "D7-Cidade ou Estado do pagador não informado";
                case "D8":
                    return "D8-Seqüência para composição do nosso número do ano atual esgotada";
                case "D9":
                    return "D9-Registro mensagem para título não cadastrado";
                case "E2":
                    return "E2-Registro complementar ao cadastro do título da cobrança com e sem registro não cadastrado";
                case "E3":
                    return "E3-Tipo de postagem inválido, diferente de S, N e branco";
                case "E4":
                    return "E4-Pedido de boletos pré-impressos";
                case "E5":
                    return "E5-Confirmação/rejeição para pedidos de boletos não cadastrado";
                case "E6":
                    return "E6-Pagador/avalista não cadastrado";
                case "E7":
                    return "E7-Informação para atualização do valor do título para protesto inválido";
                case "E8":
                    return "E8-Tipo de impressão inválido, diferente de A, B e branco";
                case "E9":
                    return "E9-Código do pagador do título divergente com o código da cooperativa de crédito";
                case "F1":
                    return "F1-Liquidado no sistema do cliente";
                case "F2":
                    return "F2-Baixado no sistema do cliente";
                case "F3":
                    return "F3-Instrução inválida, este título está caucionado/descontado";
                case "F4":
                    return "F4-Instrução fixa com caracteres inválido";
                case "F6":
                    return "F6-Nosso número / número da parcela fora de seqüência – total de parcelas inválido";
                case "F7":
                    return "F7-Falta de comprovante de prestação de serviço";
                case "F8":
                    return "F8-Nome do beneficiário incompleto / incorreto.";
                case "F9":
                    return "F9-CNPJ / CPF incompatível com o nome do pagador / Sacador Avalista";
                case "G1":
                    return "G1-CNPJ / CPF do pagador Incompatível com a espécie";
                case "G2":
                    return "G2-Título aceito: sem a assinatura do pagador";
                case "G3":
                    return "G3-Título aceito: rasurado ou rasgado";
                case "G4":
                    return "G4-Título aceito: falta título (cooperativa/ag. beneficiária deverá enviá-lo)";
                case "G5":
                    return "G5-Praça de pagamento incompatível com o endereço";
                case "G6":
                    return "G6-Título aceito: sem endosso ou beneficiário irregular";
                case "G7":
                    return "G7-Título aceito: valor por extenso diferente do valor numérico";
                case "G8":
                    return "G8-Saldo maior que o valor do título";
                case "G9":
                    return "G9-Tipo de endosso inválido";
                case "H1":
                    return "H1-Nome do pagador incompleto / Incorreto";
                case "H2":
                    return "H2-Sustação judicial H3 Pagador não encontrado";
                case "H4":
                    return "H4-Alteração de carteira";
                case "H5":
                    return "H5-Recebimento de liquidação fora da rede Sicredi – VLB Inferior – Via Compensação";
                case "H6":
                    return "H6-Recebimento de liquidação fora da rede Sicredi – VLB Superior – Via Compensação";
                case "H7":
                    return "H7-Espécie de documento necessita beneficiário ou avalista PJ";
                case "H8":
                    return "H8-Recebimento de liquidação fora da rede Sicredi – Contingência Via Compe";
                case "H9":
                    return "H9-Dados do título não conferem com disquete";
                case "I1":
                    return "I1-Pagador e Sacador Avalista são a mesma pessoa";
                case "I2":
                    return "I2-Aguardar um dia útil após o vencimento para protestar";
                case "I3":
                    return "I3-Data do vencimento rasurada I4 Vencimento – extenso não confere com número";
                case "I5":
                    return "I5-Falta data de vencimento no título";
                case "I6":
                    return "I6-DM/DMI sem comprovante autenticado ou declaração";
                case "I7":
                    return "I7-Comprovante ilegível para conferência e microfilmagem";
                case "I8":
                    return "I8-Nome solicitado não confere com emitente ou pagador";
                case "I9":
                    return "I9-Confirmar se são 2 emitentes. Se sim, indicar os dados dos 2";
                case "J1":
                    return "J1-Endereço do pagador igual ao do pagador ou do portador";
                case "J2":
                    return "J2-Endereço do apresentante incompleto ou não informado";
                case "J3":
                    return "J3-Rua/número inexistente no endereço";
                case "J4":
                    return "J4-Falta endosso do favorecido para o apresentante";
                case "J5":
                    return "J5-Data da emissão rasurada J6 Falta assinatura do pagador no título";
                case "J7":
                    return "J7-Nome do apresentante não informado/incompleto/incorreto";
                case "J8":
                    return "J8-Erro de preenchimento do titulo";
                case "J9":
                    return "J9-Titulo com direito de regresso vencido";
                case "K1":
                    return "K1-Titulo apresentado em duplicidade";
                case "K2":
                    return "K2-Titulo já protestado";
                case "K3":
                    return "K3-Letra de cambio vencida – falta aceite do pagador";
                case "K4":
                    return "K4-Falta declaração de saldo assinada no título";
                case "K5":
                    return "K5-Contrato de cambio – Falta conta gráfica";
                case "K6":
                    return "K6-Ausência do documento físico";
                case "K7":
                    return "K7-Pagador falecido";
                case "K8":
                    return "K8-Pagador apresentou quitação do título";
                case "K9":
                    return "K9-Título de outra jurisdição territorial";
                case "L1":
                    return "L1-Título com emissão anterior a concordata do pagador ";
                case "L2":
                    return "L2-Pagador consta na lista de falência";
                case "L3":
                    return "L3-Apresentante não aceita publicação de edital";
                case "L4":
                    return "L4-Dados do Pagador em Branco ou inválido";
                case "L5":
                    return "L5-Código do Pagador na agência beneficiária está duplicado";
                case "M1":
                    return "M1-Reconhecimento da dívida pelo pagador";
                case "M2":
                    return "M2-Não reconhecimento da dívida pelo pagador";
                case "X1":
                    return "X1-Regularização centralizadora – Rede Sicredi";
                case "X2":
                    return "X2-Regularização centralizadora – Compensação";
                case "X3":
                    return "X3-Regularização centralizadora – Banco correspondente";
                case "X4":
                    return "X4-Regularização centralizadora - VLB Inferior - via compensação";
                case "X5":
                    return "X5-Regularização centralizadora - VLB Superior - via compensação";
                case "X0":
                    return "X0-Pago com cheque X6 Pago com cheque – bloqueado 24 horas";
                case "X7":
                    return "X7-Pago com cheque – bloqueado 48 horas";
                case "X8":
                    return "X8-Pago com cheque – bloqueado 72 horas";
                case "X9":
                    return "X9-Pago com cheque – bloqueado 96 horas";
                case "XA":
                    return "XA-Pago com cheque – bloqueado 120 horas";
                case "XB":
                    return "XB-Pago com cheque – bloqueado 144 horas";
                default:
                    return "";
            }
        }

        public string DeletarFoto(int id, int foto)
        {

            var movimentacao = db.Movimentacao.Find(id);

            switch (foto)
            {
                case 1:
                    movimentacao.Foto1 = UploadImagens.UploadRename(null, null, movimentacao.Foto1);
                    break;
                case 2:
                    movimentacao.Foto2 = UploadImagens.UploadRename(null, null, movimentacao.Foto2);
                    break;
                case 3:
                    movimentacao.Foto3 = UploadImagens.UploadRename(null, null, movimentacao.Foto3);
                    break;
                case 4:
                    movimentacao.Foto4 = UploadImagens.UploadRename(null, null, movimentacao.Foto4);
                    break;                
            }
            db.Entry(movimentacao).State = EntityState.Modified;
            db.SaveChanges();
            return "ok";
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}