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
using System.Globalization;

namespace CFC.Areas.Admin.Controllers
{
    public class ItensController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();

        // GET: Admin/Itens
        [Authorize(Roles = "Admin,Operador")]
        public ActionResult Index(string id)
        {
            //var itens = db.Itens.Include(i => i.Clientes).Include(i => i.Marcas).Include(i => i.Tipos);
            ViewBag.Cliente = id;
            return View();
        }

        [Authorize(Roles = "Admin,Operador")]
        public JsonResult Lista(jQueryDataTableParamModel param, int id = 0)
        {


            var data = (from x in db.Itens.ToList()
                        select new
                        {
                            x.CodigoDoItem,
                            x.Placa1DoVeiculo,
                            x.DescricaoDoModelo,
                            NomeCFC = x.Clientes.NomeCFC + " - " + x.Clientes.Cidades.NomeDaCidade + "/" + x.Clientes.Estados.NomeDoEstado,
                            x.CodigoDocliente,
                            Valor = String.Format("{0:N2}", x.Valor),
                            Ativo = x.DataDaDesativacao != null ? "D " + x.DataDaDesativacao.Value.ToShortDateString() : "A " + x.DataDaAtivacao.Value.ToShortDateString()
                        });

            if (id != 0)
            {
                data = data.Where(x => x.CodigoDocliente == id);
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
                iTotalRecords = db.Estados.Count(),
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

        // GET: Admin/Itens/Details/5
        [Authorize(Roles = "Admin,Operador")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Itens itens = db.Itens.Find(id);
            if (itens == null)
            {
                return HttpNotFound();
            }
            return View(itens);
        }

        // GET: Admin/Itens/Create
        [Authorize(Roles = "Admin,Operador")]
        public ActionResult Create()
        {
            ViewBag.CodigoDocliente = new SelectList(db.Clientes.Select(x => new { x.CodigoDoCliente, Cliente = x.NomeCFC + " (" + x.Cidades.NomeDaCidade + "/" + x.Estados.NomeDoEstado + ")" }), "CodigoDoCliente", "Cliente");
            //ViewBag.CodigoDaMarca = new SelectList(db.Marcas, "CodigoDaMarca", "NomeDaMarca");
            ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo");
            return View();
        }

        // POST: Admin/Itens/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Itens itens)
        {
            var cli = new SelectList(db.Clientes.Select(x => new { x.CodigoDoCliente, Cliente = x.NomeCFC + " (" + x.Cidades.NomeDaCidade + "/" + x.Estados.NomeDoEstado + ")" }), "CodigoDoCliente", "Cliente", itens.CodigoDocliente);
            if (itens.CodigoDocliente == 0)
            {
                TempData["erro"] = "Selecione o Cliente!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            if (db.Itens.Any(x => x.Placa1DoVeiculo.Contains(itens.Placa1DoVeiculo)))
            {
                TempData["erro"] = "Placa já Cadastrada!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            if (itens.DataDoCadastro > itens.DataDaAtivacao)
            {
                TempData["erro"] = "A Data de Ativação não pode ser MENOR que a Data do Cadastro!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            var datacli = db.Clientes.Find(itens.CodigoDocliente).DataDoCadastro;
            if (itens.DataDaAtivacao < datacli || itens.DataDoCadastro < datacli)
            {
                TempData["erro"] = "A Data de Ativação/Cadastro não pode ser MENOR que a Data de cadastro do CLIENTE!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            if (itens.AnoDeFabricacao == null)
            {
                TempData["erro"] = "Preencha o Ano de Fabricação!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            if (itens.DescricaoDaMarca == null)
            {
                TempData["erro"] = "Preencha a Marca!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            if (itens.DescricaoDoModelo == null)
            {
                TempData["erro"] = "Preencha o Modelo!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }            
            
            itens.AnoDeFabricacao = itens.AnoDeFabricacao.Substring(0, 4);
            itens.Foto1 = UploadImagens.UploadRename(Request.Files["f1"], "Foto1", null);
            itens.Foto2 = UploadImagens.UploadRename(Request.Files["f2"], "Foto2", null);
            itens.Foto3 = UploadImagens.UploadRename(Request.Files["f3"], "Foto3", null);
            itens.Foto4 = UploadImagens.UploadRename(Request.Files["f4"], "Foto4", null);

            if (ModelState.IsValid)
            {
                itens.Placa1DoVeiculo = itens.Placa1DoVeiculo.ToUpper();
                itens.Placa2DoVeiculo = itens.Placa2DoVeiculo?.ToUpper();
                itens.DescricaoDoModelo = itens.DescricaoDoModelo.ToUpper();
                itens.DescricaoDaMarca = itens.DescricaoDaMarca.ToUpper();
                db.Itens.Add(itens);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CodigoDocliente = cli;
            ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
            return View(itens);
        }

        // GET: Admin/Itens/Edit/5
        [Authorize(Roles = "Admin,Operador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Itens itens = db.Itens.Find(id);
            if (itens == null)
            {
                return HttpNotFound();
            }
            ViewBag.CodigoDocliente = new SelectList(db.Clientes, "CodigoDoCliente", "NomeCFC", itens.CodigoDocliente);
            ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
            return View(itens);
        }

        // POST: Admin/Itens/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Itens itens)
        {
            var cli = new SelectList(db.Clientes.Select(x => new { x.CodigoDoCliente, Cliente = x.NomeCFC + " (" + x.Cidades.NomeDaCidade + "/" + x.Estados.NomeDoEstado +")" }), "CodigoDoCliente", "Cliente", itens.CodigoDocliente);
            if (itens.CodigoDocliente == 0)
            {
                TempData["erro"] = "Selecione o Cliente!";
                ViewBag.CodigoDocliente = cli;            
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            if (itens.DataDoCadastro > itens.DataDaAtivacao)
            {
                TempData["erro"] = "A Data de Ativação não pode ser MENOR que a Data do Cadastro!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            var datacli = db.Clientes.Find(itens.CodigoDocliente).DataDoCadastro;
            if (itens.DataDaAtivacao < datacli || itens.DataDoCadastro < datacli)
            {
                TempData["erro"] = "A Data de Ativação/Cadastro não pode ser MENOR que a Data de cadastro do CLIENTE!";
                ViewBag.CodigoDocliente = cli;                
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            if (itens.AnoDeFabricacao == null)
            {
                TempData["erro"] = "Preencha o Ano de Fabricação!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            if (itens.DescricaoDaMarca == null)
            {
                TempData["erro"] = "Preencha a Marca!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            if (itens.DescricaoDoModelo == null)
            {
                TempData["erro"] = "Preencha o Modelo!";
                ViewBag.CodigoDocliente = cli;
                ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
                return View(itens);
            }

            itens.AnoDeFabricacao = itens.AnoDeFabricacao.Count() > 4 ? itens.AnoDeFabricacao.Substring(0, 4) : itens.AnoDeFabricacao;
            itens.Foto1 = UploadImagens.UploadRename(Request.Files["f1"], "Foto1", itens.Foto1);
            itens.Foto2 = UploadImagens.UploadRename(Request.Files["f2"], "Foto2", itens.Foto2);
            itens.Foto3 = UploadImagens.UploadRename(Request.Files["f3"], "Foto3", itens.Foto3);
            itens.Foto4 = UploadImagens.UploadRename(Request.Files["f4"], "Foto4", itens.Foto4);

            if (ModelState.IsValid)
            {
                itens.Placa1DoVeiculo = itens.Placa1DoVeiculo.ToUpper();
                itens.Placa2DoVeiculo = itens.Placa2DoVeiculo?.ToUpper();
                itens.DescricaoDoModelo = itens.DescricaoDoModelo.ToUpper();
                itens.DescricaoDaMarca = itens.DescricaoDaMarca.ToUpper();
                db.Entry(itens).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CodigoDocliente = cli;
            ViewBag.CodigoDoTipo = new SelectList(db.Tipos, "CodigoDoTipo", "NomeDoTipo", itens.CodigoDoTipo);
            return View(itens);
        }

        // GET: Admin/Itens/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Itens itens = db.Itens.Find(id);
        //    if (itens == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(itens);
        //}

        // POST: Admin/Itens/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
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
                Itens itens = db.Itens.Find(id);
            db.Itens.Remove(itens);
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
        public ActionResult Itens(int CodigoDoCliente)
        {
            var lista = (from x in db.Itens.Where(x => x.CodigoDocliente == CodigoDoCliente) select new { x.CodigoDoItem, descricao = x.CodigoDoItem + " " + x.Placa1DoVeiculo.ToUpper() + " - " + x.DescricaoDoModelo.ToUpper() }).ToList();
            ViewBag.CodigoDoItem = new SelectList(lista, "CodigoDoItem", "descricao");
            return View();
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
