using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CFC.Models;
using CFC.Uteis;
using Microsoft.Ajax.Utilities;

namespace CFC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Operador")]
    public class SinistrosController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();

        // GET: Admin/Sinistros
        public ActionResult Index(string id)
        {
            ViewBag.Sinistro = id;
            return View();
        }
        
        public JsonResult Lista(jQueryDataTableParamModel param, int id = 0)
        {


            var data = (from x in db.Sinistros.ToList()
                select new
                {
                    x.CodigoDoSinistro,
                    Placa1DoVeiculo = x.CodigoDoItem + " " + x.Itens.DescricaoDoModelo + " - " + x.Itens.Placa1DoVeiculo,
                    NomeCFC = x.CodigoDoCliente + " " + x.Itens.Clientes.NomeCFC + " - " + x.Itens.Clientes.Cidades.NomeDaCidade + "/" + x.Itens.Clientes.Estados.NomeDoEstado,
                    Valor = x.DataDeConclusaoDoSinistro == null ? "R$" + $"{x.ValorDeReferencia:N2}" + " (R)"  : "R$" + $"{x.ValorDoSinistro:N2}",
                    Data = x.DataDoSinistro.Value.ToShortDateString(),
                    Conclusao = x.DataDeConclusaoDoSinistro == null ? "" : x.DataDoSinistro.Value.ToShortDateString()
                });

            if (id != 0)
            {
                data = data.Where(x => x.CodigoDoSinistro == id);
            }

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                param.sSearch = param.sSearch.ToUpper();
                data = data.Where(c => c.NomeCFC.Contains(param.sSearch) || c.Placa1DoVeiculo.Contains(param.sSearch) || c.NomeCFC.Contains(param.sSearch));
            }

            var result = data;
            if (param.iDisplayLength != -1)
            {
                result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            }

            return Json(new
                {
                    param.sEcho,
                    iTotalRecords = db.Estados.Count(),
                    iTotalDisplayRecords = data.Count(),
                    aaData = from x in result
                    select new[]
                    {
                        x.CodigoDoSinistro.ToString(),
                        x.Placa1DoVeiculo,
                        x.NomeCFC,
                        x.Valor,
                        x.Data,
                        x.Conclusao
                    }
                },
                JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Operador")]
        public ActionResult Simulacao()
        {
            var lista = (from x in db.Clientes
                select new
                {
                    x.CodigoDoCliente,
                    descricao = x.CodigoDoCliente + " " + x.NomeCFC.ToUpper() + " | " + x.Cidades.NomeDaCidade.ToUpper() + "/" + x.Estados.NomeDoEstado
                }).ToList();
            ViewBag.Cliente = new SelectList(lista, "CodigoDoCliente", "descricao");
            //ViewBag.CodigoDoSinistro = id;

            return View();
        }

        //[Authorize(Roles = "Admin,Operador")]
        //public JsonResult ListaSimulacao(jQueryDataTableParamModel param, string dataSimulacao,  int Cliente = 0)
        //{
        //    DateTime? datasinistro0 = DateTime.Today.AddDays(1);
        //    DateTime? datasinistro1 = DateTime.Today.AddDays(-1);
        //    DateTime? dataDoSinistro = DateTime.Today;

        //    if (dataSimulacao != "")
        //    {
        //        dataDoSinistro = Convert.ToDateTime(dataSimulacao);
        //        datasinistro0 = dataDoSinistro.Value.AddDays(1);
        //        datasinistro1 = dataDoSinistro.Value.AddDays(-1);
        //    }

        //    var data = (from x in db.Clientes.ToList()
        //        join i in db.Itens.Where(x => x.DataDaAtivacao < datasinistro0 && (x.DataDaDesativacao == null || x.DataDaDesativacao > datasinistro1)) on x.CodigoDoCliente equals i.CodigoDocliente 
        //        into i1                
        //        select new
        //        {
        //            x.CodigoDoCliente,
        //            x.NomeCFC,
        //            x.Cidades.NomeDaCidade,
        //            x.Estados.NomeDoEstado,
        //            x.CPFCNPJDoCliente,                    
        //            Ativa = x.ClienteAtivo ? "Sim" : "Não",
        //            NItens = i1.Count(),
        //            Itens = i1.Count().ToString(),
        //            Cotas = i1.Sum(z => z.Valor) / 10000
        //        }).Where(x => x.NItens > 0);

        //    //busca
        //    if (data.Any())
        //    {
        //        if (Cliente != 0)
        //        {
        //            data = data.Where(x => x.CodigoDoCliente == Cliente);
        //        } 
        //    }            

        //    var result = data;
        //    if (param.iDisplayLength != -1)
        //    {
        //        result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    }

        //    return Json(new
        //        {
        //            param.sEcho,
        //            iTotalRecords = db.Clientes.Count(),
        //            iTotalDisplayRecords = data.Count(),
        //            aaData = from x in result
        //            select new[]
        //            {
        //                string.Empty,
        //                x.CodigoDoCliente.ToString(),
        //                x.NomeCFC,
        //                x.NomeDaCidade,
        //                x.NomeDoEstado,
        //                x.Ativa,
        //                x.Itens,
        //                x.Cotas.ToString()

        //            }
        //        },
        //        JsonRequestBehavior.AllowGet);
        //}

        [Authorize(Roles = "Admin,Operador")]
        public JsonResult ListaSimulacao(jQueryDataTableParamModel param, string dataSimulacao, string valorSinistro, int Cliente = 0)
        {
            DateTime dataSinistro = dataSimulacao == "" ? DateTime.Today : Convert.ToDateTime(dataSimulacao);
            Decimal valorDoSinistro = valorSinistro == "" ? (dynamic)0 : Convert.ToDecimal(valorSinistro);

            var data = (from x in db.Itens.Where(x => x.DataDaAtivacao < dataSinistro && (x.DataDaDesativacao == null || x.DataDaDesativacao > dataSinistro)).OrderBy(x => x.CodigoDocliente).ToList()
                //join i in db.Itens on x.CodigoDoItem equals i.CodigoDoItem
                let soma = db.Itens.Where(z => z.DataDaAtivacao < dataSinistro && (z.DataDaDesativacao == null || z.DataDaDesativacao > dataSinistro)).ToList()
                let cotasNaData = valorDoSinistro / (soma.Sum(s => s.Valor) / 10000)
                        //let valor = db.Sinistros.Find(id)
                        select new
                {
                    //x.CodigoDadosDoSinistro,
                    x.Clientes.CodigoDoCliente,
                    x.CodigoDoItem,
                    CFC = " <strong>[ " + x.Clientes.CodigoDoCliente + " ] " + x.Clientes.NomeCFC + " - " + x.Clientes.Cidades.NomeDaCidade + " / " + x.Clientes.Estados.NomeDoEstado + (!x.Clientes.ClienteAtivo ? " (D)" : " (C " + x.Clientes.DataDoCadastro.Value.ToShortDateString() + ")") + " Cotas " + soma.Where(c => c.Clientes.CodigoDoCliente == x.CodigoDocliente).Sum(c => c.Valor) / 10000 + " | Rateio R$ " + $"{ cotasNaData * (soma.Where(c => c.Clientes.CodigoDoCliente == x.CodigoDocliente).Sum(c => c.Valor) / 10000):N2}",
                    Item = x.DescricaoDoModelo + "/" + x.DescricaoDaMarca + " - " + x.AnoDeFabricacao,
                    ItemData = (x.DataDaDesativacao == null ? "A (" + x.DataDaAtivacao.Value.ToShortDateString() : "D (" + x.DataDaDesativacao.Value.ToShortDateString()) + ")",
                    Valor = "R$ " + $"{x.Valor:N2}",
                    Cotas = x.Valor / 10000,
                    //Rateio = "R$ " + $"{cotasNaData * (x.Valor / 10000):N2}"
                    Rateio = "R$ " + $"{cotasNaData * (x.Valor / 10000):N2}"
                });

            if (data.Any())
            {
                if (Cliente != 0)
                {
                    data = data.Where(x => x.CodigoDoCliente == Cliente);
                }
            }

            var result = data;
            if (param.iDisplayLength != -1)
            {
                result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            }

            return Json(new
                {
                    param.sEcho,
                    iTotalRecords = db.Clientes.Count(),
                    iTotalDisplayRecords = data.Count(),
                    aaData = from x in result
                    select new[]
                    {
                        //x.CodigoDadosDoSinistro.ToString(),
                        x.CodigoDoItem.ToString(),
                        x.CFC,
                        x.Item,
                        x.ItemData,
                        x.Valor,
                        x.Cotas.ToString(),
                        x.Rateio.ToString()
                    }
                },
                JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Operador")]
        public PartialViewResult GetRowDetail(int id, string dataSimulacao)
        {
            //DateTime? datasinistro0 = DateTime.Today.AddDays(1);
            //DateTime? datasinistro1 = DateTime.Today.AddDays(-1);
            DateTime dataDoSinistro = DateTime.Today;

            if (dataSimulacao != "")
            {
                dataDoSinistro = Convert.ToDateTime(dataSimulacao);
                //datasinistro0 = dataDoSinistro.Value.AddDays(1);
                //datasinistro1 = dataDoSinistro.Value.AddDays(-1);
            }
            return PartialView("RowDetail", db.Itens.Where(x => x.CodigoDocliente.Equals(id) && (x.DataDaAtivacao < dataDoSinistro && (x.DataDaDesativacao == null || x.DataDaDesativacao > dataDoSinistro))));
        }

        // GET: Admin/Sinistros/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sinistros sinistros = db.Sinistros.Find(id);
            if (sinistros == null)
            {
                return HttpNotFound();
            }
            return View(sinistros);
        }

        // GET: Admin/Sinistros/Create
        public ActionResult Create()
        {
            var lista = (from x in db.Clientes.Where(x => x.ClienteAtivo) select new { x.CodigoDoCliente, descricao = x.NomeCFC.ToUpper() + " - " + x.NomeCompletoDoCliente.ToUpper() }).ToList();
            ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao");
            ViewBag.CodigoDoItem = new SelectList(db.Itens, "CodigoDoItem", "Placa1DoVeiculo", 0);
            return View();
        }

        // POST: Admin/Sinistros/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sinistros sinistros)
        {
            if (ModelState.IsValid)
            {
                sinistros.Foto1 = UploadImagens.UploadRename(Request.Files["f1"], "Foto1", null);
                sinistros.Foto2 = UploadImagens.UploadRename(Request.Files["f2"], "Foto2", null);
                sinistros.Foto3 = UploadImagens.UploadRename(Request.Files["f3"], "Foto3", null);
                sinistros.Foto4 = UploadImagens.UploadRename(Request.Files["f4"], "Foto4", null);
                sinistros.Foto5 = UploadImagens.UploadRename(Request.Files["f5"], "Foto5", null);
                sinistros.Foto6 = UploadImagens.UploadRename(Request.Files["f6"], "Foto6", null);
                sinistros.Foto7 = UploadImagens.UploadRename(Request.Files["f7"], "Foto7", null);
                sinistros.Foto8 = UploadImagens.UploadRename(Request.Files["f8"], "Foto8", null);

                db.Sinistros.Add(sinistros);
                db.SaveChanges();
                
                //salvar dados do sinistro
                SalvarSinistroCadastro(sinistros.CodigoDoSinistro);

                return RedirectToAction("Index");
            }

            var lista = (from x in db.Clientes select new { x.CodigoDoCliente, descricao = x.NomeCFC.ToUpper() + " - " + x.NomeCompletoDoCliente.ToUpper() }).ToList();
            ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao", sinistros.CodigoDoCliente);
            ViewBag.CodigoDoItem = new SelectList(db.Itens.Where(x => x.CodigoDocliente == sinistros.CodigoDoCliente), "CodigoDoItem", "Placa1DoVeiculo", sinistros.CodigoDoItem);
            return View(sinistros);
        }

        // GET: Admin/Sinistros/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sinistros sinistros = db.Sinistros.Find(id);            
            if (sinistros == null)
            {
                return HttpNotFound();
            }

            sinistros.ValorDoSinistro = db.Movimentacao.Any(x => x.CodigoDoSinistro == sinistros.CodigoDoSinistro) ? db.Movimentacao.Where(x => x.CodigoDoSinistro == sinistros.CodigoDoSinistro).Sum(x => x.ValorDoTitulo) : 0;
            sinistros.ValorPorCota = sinistros.ValorDoSinistro / sinistros.CotasNaData;
            db.Entry(sinistros).State = EntityState.Modified;
            db.SaveChanges();

            var lista = (from x in db.Clientes select new { x.CodigoDoCliente, descricao = x.NomeCFC.ToUpper() + " - " + x.NomeCompletoDoCliente.ToUpper() }).ToList();
            ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao", sinistros.CodigoDoCliente);
            ViewBag.CodigoDoItem = new SelectList(db.Itens.Where(x => x.CodigoDocliente == sinistros.CodigoDoCliente), "CodigoDoItem", "Placa1DoVeiculo", sinistros.CodigoDoItem);
            return View(sinistros);
        }

        // POST: Admin/Sinistros/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Sinistros sinistros)
        {
            if (ModelState.IsValid)
            {
                sinistros.Foto1 = UploadImagens.UploadRename(Request.Files["f1"], "Foto1", sinistros.Foto1);
                sinistros.Foto2 = UploadImagens.UploadRename(Request.Files["f2"], "Foto2", sinistros.Foto2);
                sinistros.Foto3 = UploadImagens.UploadRename(Request.Files["f3"], "Foto3", sinistros.Foto3);
                sinistros.Foto4 = UploadImagens.UploadRename(Request.Files["f4"], "Foto4", sinistros.Foto4);
                sinistros.Foto5 = UploadImagens.UploadRename(Request.Files["f5"], "Foto5", sinistros.Foto5);
                sinistros.Foto6 = UploadImagens.UploadRename(Request.Files["f6"], "Foto6", sinistros.Foto6);
                sinistros.Foto7 = UploadImagens.UploadRename(Request.Files["f7"], "Foto7", sinistros.Foto7);
                sinistros.Foto8 = UploadImagens.UploadRename(Request.Files["f8"], "Foto8", sinistros.Foto8);

                db.Entry(sinistros).State = EntityState.Modified;
                db.SaveChanges();

                //salvar dados do sinistro REMOVER
                //SalvarSinistroCadastro(sinistros.CodigoDoSinistro);

                return RedirectToAction("Index");
            }
            var lista = (from x in db.Clientes select new { x.CodigoDoCliente, descricao = x.NomeCFC.ToUpper() + " - " + x.NomeCompletoDoCliente.ToUpper() }).ToList();
            ViewBag.CodigoDoCliente = new SelectList(lista, "CodigoDoCliente", "descricao", sinistros.CodigoDoCliente);
            ViewBag.CodigoDoItem = new SelectList(db.Itens.Where(x => x.CodigoDocliente == sinistros.CodigoDoCliente), "CodigoDoItem", "Placa1DoVeiculo", sinistros.CodigoDoItem);
            return View(sinistros);
        }

        [Authorize(Roles = "Admin,Operador")]
        public JsonResult DeleteConfirmed(int id)
        {
            var response = new
            {
                success = true,
                messages = "Registro excluido!"

            };

            if (DadosSeguros.Nivel != "Admin")
            {
                response = new
                {
                    success = false,
                    messages = "Somente Administradores podem Exluir Registros!"
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            try
            {
                Sinistros sinistros = db.Sinistros.Find(id);

                //remove sinistro se houver
                if (db.DadosDoSinistro.Any(x => x.CodigoDoSinistro == id))
                {
                    foreach (var sin in db.DadosDoSinistro.Where(x => x.CodigoDoSinistro == id))
                    {
                        cfcadminEntities dbx = new cfcadminEntities();
                        DadosDoSinistro dadosDoSinistro = dbx.DadosDoSinistro.Find(sin.CodigoDadosDoSinistro);
                        if (dadosDoSinistro != null) dbx.DadosDoSinistro.Remove(dadosDoSinistro);
                        dbx.SaveChanges();
                    }
                }

                sinistros.Foto1 = UploadImagens.UploadRename(null, null, sinistros.Foto1);
                sinistros.Foto2 = UploadImagens.UploadRename(null, null, sinistros.Foto2);
                sinistros.Foto3 = UploadImagens.UploadRename(null, null, sinistros.Foto3);
                sinistros.Foto4 = UploadImagens.UploadRename(null, null, sinistros.Foto4);
                sinistros.Foto5 = UploadImagens.UploadRename(null, null, sinistros.Foto5);
                sinistros.Foto6 = UploadImagens.UploadRename(null, null, sinistros.Foto6);
                sinistros.Foto7 = UploadImagens.UploadRename(null, null, sinistros.Foto7);
                sinistros.Foto8 = UploadImagens.UploadRename(null, null, sinistros.Foto8);

                db.Sinistros.Remove(sinistros);
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
 
        [Authorize(Roles = "Admin,Operador")]
        public ActionResult Painel(string dataSimulacao, string valorSinistro, int cliente = 0, int codigoDoSinistro = 0)
        {
            ViewBag.Data = dataSimulacao;
            ViewBag.Cliente = cliente;
            ViewBag.CodigoDoSinistro = codigoDoSinistro;
            ViewBag.valorSinistro = valorSinistro == "" ? (dynamic) 0 : valorSinistro;
            return View();
        }

        public string DeletarFoto(int id, int foto)
        {

            var sinistros = db.Sinistros.Find(id);

            switch (foto)
            {
                case 1:
                    sinistros.Foto1 = UploadImagens.UploadRename(null, null, sinistros.Foto1); 
                    break;
                case 2:
                    sinistros.Foto2 = UploadImagens.UploadRename(null, null, sinistros.Foto2);
                    break;
                case 3:
                    sinistros.Foto3 = UploadImagens.UploadRename(null, null, sinistros.Foto3);
                    break;
                case 4:
                    sinistros.Foto4 = UploadImagens.UploadRename(null, null, sinistros.Foto4);
                    break;
                case 5:
                    sinistros.Foto5 = UploadImagens.UploadRename(null, null, sinistros.Foto5);
                    break;
                case 6:
                    sinistros.Foto6 = UploadImagens.UploadRename(null, null, sinistros.Foto6);
                    break;
                case 7:
                    sinistros.Foto7 = UploadImagens.UploadRename(null, null, sinistros.Foto7);
                    break;
                case 8:
                    sinistros.Foto8 = UploadImagens.UploadRename(null, null, sinistros.Foto8);
                    break;
            }
            db.Entry(sinistros).State = EntityState.Modified;
            db.SaveChanges();
            return "ok";
        }

        //Clientes no Sinistro
        public void SalvarSinistroCadastro(int id)
        {
            //remove sinistro se houver
            if (db.DadosDoSinistro.Any(x => x.CodigoDoSinistro == id))
            {
                foreach (var sin in db.DadosDoSinistro.Where(x => x.CodigoDoSinistro == id))
                {
                    cfcadminEntities dbx = new cfcadminEntities();
                    DadosDoSinistro dadosDoSinistro = dbx.DadosDoSinistro.Find(sin.CodigoDadosDoSinistro);
                    if (dadosDoSinistro != null) dbx.DadosDoSinistro.Remove(dadosDoSinistro);
                    dbx.SaveChanges();
                }
            }
            //salvar cotas na data, clientes no rateio
            var sinistro = db.Sinistros.Find(id);

            //DateTime? datasinistro0 = sinistro.DataDoSinistro?.AddDays(1) ?? sinistro.DataDoSinistro;
            //DateTime? datasinistro1 = sinistro.DataDoSinistro?.AddDays(-1) ?? sinistro.DataDoSinistro;

            {

                var selecionaItens = db.Itens.Where(x => x.DataDaAtivacao < sinistro.DataDoSinistro && (x.DataDaDesativacao == null || x.DataDaDesativacao > sinistro.DataDoSinistro));

                //altera sinistro selecionado
                sinistro.CotasNaData = selecionaItens.Sum(x => x.Valor) / 10000 ?? 0;                
                sinistro.ClientesNoRateio = selecionaItens.GroupBy(x => x.CodigoDocliente).Count();
                db.Entry(sinistro).State = EntityState.Modified;
                db.SaveChanges();

                //salva clientes e itens selecionados no sinistro
                var clientes = (from x in selecionaItens
                    select new
                    {
                        x.CodigoDocliente,
                        x.CodigoDoItem,
                        x.Valor,
                        ClienteCadastrado = x.Clientes.DataDoCadastro,
                        ClienteInativo = x.Clientes.ClienteAtivo,
                        ItemCadastrado = x.DataDoCadastro,
                        ItemInativo = x.DataDaDesativacao
                    }).ToList();

                var dadosSoSinistro = new DadosDoSinistro();
                foreach (var item in clientes)
                {
                    dadosSoSinistro.CodigoDoSinistro = id;
                    dadosSoSinistro.DataDoSinistro = sinistro.DataDoSinistro ?? DateTime.Today;
                    dadosSoSinistro.CodigoDoCliente = item.CodigoDocliente;
                    dadosSoSinistro.CodigoDoItem = item.CodigoDoItem;
                    dadosSoSinistro.ValorDoItem = item.Valor ?? 0;
                    dadosSoSinistro.ClienteCadastrado = item.ClienteCadastrado ?? DateTime.Today;
                    dadosSoSinistro.ClienteInativo = item.ClienteInativo;
                    dadosSoSinistro.ItemCadastrado = item.ItemCadastrado ?? DateTime.Now;
                    dadosSoSinistro.ItemInativo = item.ItemInativo;
                    db.DadosDoSinistro.Add(dadosSoSinistro);
                    db.SaveChanges();
                }
            }
        }

        public ActionResult ClientesNoSinistro(string id)
        {
            var lista = (from x in db.Clientes
                select new
                {
                    x.CodigoDoCliente,
                    descricao = x.CodigoDoCliente + " " + x.NomeCFC.ToUpper() + " | " + x.Cidades.NomeDaCidade.ToUpper() + "/" + x.Estados.NomeDoEstado
                }).ToList();
            ViewBag.Cliente = new SelectList(lista, "CodigoDoCliente", "descricao");
            ViewBag.CodigoDoSinistro = id;

            return View();
        }

        [Authorize(Roles = "Admin,Operador")]
        public JsonResult ListaClientesNoSinistro(jQueryDataTableParamModel param, int id, int Cliente = 0)
        {
            
            var data = (from x in db.DadosDoSinistro.Where(x => x.CodigoDoSinistro == id).OrderBy(x => x.CodigoDoCliente).ToList()
                join i in db.Itens on x.CodigoDoItem equals i.CodigoDoItem
                let soma = db.DadosDoSinistro.Where(z => z.CodigoDoSinistro == id).ToList()
                let valor = db.Sinistros.Find(id)
                        select new
                {
                    x.CodigoDadosDoSinistro,
                    x.CodigoDoCliente,
                    x.CodigoDoItem,
                    CFC = " <strong>[ " + x.CodigoDoCliente +" ] " + i.Clientes.NomeCFC + " - " + i.Clientes.Cidades.NomeDaCidade + " / " + i.Clientes.Estados.NomeDoEstado + (!x.ClienteInativo ? " (D)" : " (C " + x.ClienteCadastrado.ToShortDateString() + ")") + " Cotas " + soma.Where(c => c.CodigoDoCliente == x.CodigoDoCliente).Sum(c => c.ValorDoItem)/10000 + " | Rateio R$ " + $"{(valor.ValorDeReferencia/valor.CotasNaData) * (soma.Where(c => c.CodigoDoCliente == x.CodigoDoCliente).Sum(c => c.ValorDoItem) / 10000):N2}",                    
                    Item = i.DescricaoDoModelo + " - " + i.Placa1DoVeiculo,
                    ItemData = (i.DataDaDesativacao == null ? "A (" + i.DataDaAtivacao.Value.ToShortDateString() : "D (" + i.DataDaDesativacao.Value.ToShortDateString()) + ")",
                    Valor = "R$ " + $"{x.ValorDoItem:N2}",
                    Cotas = i.Valor / 10000,
                    Rateio ="R$ " + $"{(valor.ValorDeReferencia/valor.CotasNaData) * (i.Valor / 10000):N2}"
                });

            if (data.Any())
            {
                if (Cliente != 0)
                {
                    data = data.Where(x => x.CodigoDoCliente == Cliente);
                }
            }

            var result = data;
            if (param.iDisplayLength != -1)
            {
                result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            }

            return Json(new
            {
                param.sEcho,
                iTotalRecords = db.Clientes.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = from x in result
                         select new[]
                         {
                        x.CodigoDadosDoSinistro.ToString(),
                        x.CodigoDoItem.ToString(),
                        x.CFC,
                        x.Item,
                        x.ItemData,
                        x.Valor,
                        x.Cotas.ToString(),
                        x.Rateio
                    }
                },
                JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Operador")]
        public JsonResult RemoverClienteDoSinistro(int id)
        {
            var response = new
            {
                success = true,
                messages = "Registro excluido!"

            };

            if (DadosSeguros.Nivel != "Admin")
            {
                response = new
                {
                    success = false,
                    messages = "Somente Administradores podem Exluir Registros!"
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            try
            {
                DadosDoSinistro dadosDoSinistro = db.DadosDoSinistro.Find(id);

                db.DadosDoSinistro.Remove(dadosDoSinistro);
                db.SaveChanges();

                //altera sinistro selecionado
                var sinistro = db.Sinistros.Find(dadosDoSinistro.CodigoDoSinistro);
                var selecionaItens = db.DadosDoSinistro.Where(x => x.CodigoDoSinistro == dadosDoSinistro.CodigoDoSinistro);

                sinistro.CotasNaData = selecionaItens.Sum(x => x.ValorDoItem) / 10000;
                sinistro.ClientesNoRateio = selecionaItens.GroupBy(x => x.CodigoDoCliente).Count();
                db.Entry(sinistro).State = EntityState.Modified;
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

        [Authorize(Roles = "Admin,Operador")]
        public JsonResult InserirClienteDoSinistro( int codigoDoSinistro, int id = 0)
        {
            var response = new
            {
                success = true,
                messages = "Registro Inserido!"

            };

            if (DadosSeguros.Nivel != "Admin")
            {
                response = new
                {
                    success = false,
                    messages = "Somente Administradores podem Inserir Registros!"
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            if (id == 0)
            {
                response = new
                {
                    success = false,
                    messages = "Selecione um Item para inserir no Sinistro!"
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if(db.DadosDoSinistro.Any(x => x.CodigoDoItem == id && x.CodigoDoSinistro == codigoDoSinistro))
                {
                    response = new
                    {
                        success = false,
                        messages = "Item já existe no Sinistro verifique!"
                    };
                    return Json(response, JsonRequestBehavior.AllowGet);
                }

                DadosDoSinistro dadosDoSinistro = new DadosDoSinistro();
                var item = db.Itens.Find(id);
                var sinistro = db.Sinistros.Find(codigoDoSinistro);
                
                //insere item nos dados do sinistro
                dadosDoSinistro.CodigoDoItem = id;
                dadosDoSinistro.ClienteCadastrado = item.Clientes.DataDoCadastro.Value;
                dadosDoSinistro.ClienteInativo = item.Clientes.ClienteAtivo;
                dadosDoSinistro.CodigoDoCliente = item.Clientes.CodigoDoCliente;
                dadosDoSinistro.CodigoDoSinistro = codigoDoSinistro;
                dadosDoSinistro.DataDoSinistro = sinistro.DataDoSinistro.Value;
                dadosDoSinistro.ItemCadastrado = item.DataDaAtivacao.Value;
                dadosDoSinistro.ItemInativo = item.DataDaDesativacao;
                dadosDoSinistro.ValorDoItem = item.Valor.Value;

                db.DadosDoSinistro.Add(dadosDoSinistro);
                db.SaveChanges();

                //altera sinistro selecionado
                var selecionaItens = db.DadosDoSinistro.Where(x => x.CodigoDoSinistro == codigoDoSinistro);

                sinistro.CotasNaData = selecionaItens.Sum(x => x.ValorDoItem) / 10000;
                sinistro.ClientesNoRateio = selecionaItens.GroupBy(x => x.CodigoDoCliente).Count();
                db.Entry(sinistro).State = EntityState.Modified;
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
