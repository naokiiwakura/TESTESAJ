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

namespace CFC.Areas.Admin.Controllers
{
    public class ClientesController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();


        // GET: Admin/Clientes
        [Authorize(Roles = "Admin,Operador")]
        public ActionResult Index()
        {
            //var clientes = db.Clientes.Include(c => c.Cidades).Include(c => c.Estados);
            return View();
        }

        [Authorize(Roles = "Admin,Operador")]
        public JsonResult Lista(jQueryDataTableParamModel param)
        {


            var data = (from x in db.Clientes.ToList()
                        select new
                        {
                            x.CodigoDoCliente,
                            NomeCFC = x.NomeCFC + "<br>" + x.NomeCompletoDoCliente,
                            x.Cidades.NomeDaCidade,
                            x.Estados.NomeDoEstado, 
                            Ativa = x.ClienteAtivo ? "Sim" : "Não",
                            x.CPFCNPJDoCliente,
                            Separa = x.SepararVeiculos ? "Sim" : "Não"
                        });

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                param.sSearch = param.sSearch.ToUpper();
                data = data.Where(c => c.NomeCFC.Contains(param.sSearch) || c.NomeDaCidade.Contains(param.sSearch) || c.NomeDoEstado.Contains(param.sSearch) || c.CPFCNPJDoCliente.Contains(param.sSearch));
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
                             x.CodigoDoCliente.ToString(),
                             x.NomeCFC,
                             x.CPFCNPJDoCliente,
                             x.NomeDaCidade,
                             x.NomeDoEstado,                             
                             x.Separa,
                             x.Ativa
                        }
            },
                    JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Clientes/Details/5
        [Authorize(Roles = "Admin,Operador")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // GET: Admin/Clientes/Create
        [Authorize(Roles = "Admin,Operador")]
        public ActionResult Create()
        {
            ViewBag.CodigoDaCidade = new SelectList(db.Cidades, "CodigoDaCidade", "NomeDaCidade");
            ViewBag.CodigoDoEstado = new SelectList(db.Estados, "CodigoDoEstado", "NomeDoEstado");
            return View();
        }

        // POST: Admin/Clientes/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Clientes clientes, string cidade, string estado)
        {
            if (cidade == null)
            {
                TempData["erro"] = "Utilize o CEP para Localizar a Cidade!";                
                return View(clientes);
            }

            if (estado == null)
            {
                TempData["erro"] = "Utilize o CEP para Localizar o Estado!";
                return View(clientes);
            }

            //verifica cnpj
            if (db.Clientes.Any(x => x.CPFCNPJDoCliente.Contains(clientes.CPFCNPJDoCliente)))
            {
                TempData["erro"] = "CNPJ já CADASTRADO por favor verifique!!";
                return View(clientes);
            }

            //verifica estado
            var e = db.Estados.Where(x => x.NomeDoEstado.Contains(estado.ToUpper()));
            if (e.Any())
            {
                clientes.CodigoDoEstado = e.FirstOrDefault().CodigoDoEstado;
            }
            else
            {
                var ne = new Estados()
                {
                    NomeDoEstado = estado.ToUpper()
                };
                db.Estados.Add(ne);
                db.SaveChanges();
                clientes.CodigoDoEstado = ne.CodigoDoEstado;
            }

            //verifica estado
            var c = db.Cidades.Where(x => x.NomeDaCidade.Contains(cidade.ToUpper()));
            if (c.Any())
            {
                clientes.CodigoDaCidade = c.FirstOrDefault().CodigoDaCidade;
            }
            else
            {
                var nc = new Cidades()
                {
                    NomeDaCidade = cidade.ToUpper(),
                    CodigoDoEstado = clientes.CodigoDoEstado
                };
                db.Cidades.Add(nc);
                db.SaveChanges();
                clientes.CodigoDaCidade = nc.CodigoDaCidade;

            }

            clientes.DataDoCadastro = DateTime.Today;
            if (ModelState.IsValid)
            {
                clientes.NomeCFC = clientes.NomeCFC.ToUpper();
                clientes.NomeCompletoDoCliente = clientes.NomeCompletoDoCliente.ToUpper();
                clientes.EnderecoDoCliente = clientes.EnderecoDoCliente == null ? "" : clientes.EnderecoDoCliente.ToUpper();
                clientes.ComplementoDoCliente = clientes.ComplementoDoCliente == null ? "" : clientes.ComplementoDoCliente.ToUpper();
                clientes.BairroDoCliente = clientes.BairroDoCliente == null ? "" : clientes.BairroDoCliente.ToUpper();
                clientes.EmailDoCliente = clientes.EmailDoCliente == null ? "" : clientes.EmailDoCliente.ToLower();                
                db.Clientes.Add(clientes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CodigoDaCidade = new SelectList(db.Cidades, "CodigoDaCidade", "NomeDaCidade", clientes.CodigoDaCidade);
            ViewBag.CodigoDoEstado = new SelectList(db.Estados, "CodigoDoEstado", "NomeDoEstado", clientes.CodigoDoEstado);
            return View(clientes);
        }

        // GET: Admin/Clientes/Edit/5
        [Authorize(Roles = "Admin,Operador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            ViewBag.CodigoDaCidade = new SelectList(db.Cidades, "CodigoDaCidade", "NomeDaCidade", clientes.CodigoDaCidade);
            ViewBag.CodigoDoEstado = new SelectList(db.Estados, "CodigoDoEstado", "NomeDoEstado", clientes.CodigoDoEstado);
            return View(clientes);
        }

        // POST: Admin/Clientes/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Operador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Clientes clientes)
        {
            if (db.Itens.Any(x => x.CodigoDocliente.Equals(clientes.CodigoDoCliente) && x.DataDaDesativacao == null) && !clientes.ClienteAtivo)
            {
                TempData["erro"] = "Este cliente possui Itens ativos, Por favor verifique!";
                ViewBag.CodigoDaCidade = new SelectList(db.Cidades, "CodigoDaCidade", "NomeDaCidade", clientes.CodigoDaCidade);
                ViewBag.CodigoDoEstado = new SelectList(db.Estados, "CodigoDoEstado", "NomeDoEstado", clientes.CodigoDoEstado);
                return View(clientes);
            }

            if (ModelState.IsValid)
            {                                
                clientes.NomeCFC = clientes.NomeCFC.ToUpper();
                clientes.NomeCompletoDoCliente = clientes.NomeCompletoDoCliente.ToUpper();
                clientes.EnderecoDoCliente = clientes.EnderecoDoCliente == null ? "" : clientes.EnderecoDoCliente.ToUpper();
                clientes.ComplementoDoCliente = clientes.ComplementoDoCliente == null ? "" : clientes.ComplementoDoCliente.ToUpper();
                clientes.BairroDoCliente = clientes.BairroDoCliente == null ? "" : clientes.BairroDoCliente.ToUpper();
                clientes.EmailDoCliente = clientes.EmailDoCliente == null ? "" : clientes.EmailDoCliente.ToLower();

                db.Entry(clientes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CodigoDaCidade = new SelectList(db.Cidades, "CodigoDaCidade", "NomeDaCidade", clientes.CodigoDaCidade);
            ViewBag.CodigoDoEstado = new SelectList(db.Estados, "CodigoDoEstado", "NomeDoEstado", clientes.CodigoDoEstado);
            return View(clientes);
        }

        // GET: Admin/Clientes/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Clientes clientes = db.Clientes.Find(id);
        //    if (clientes == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(clientes);
        //}

        // POST: Admin/Clientes/Delete/5
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
                    messages = "Somente Administradores podem Excluir Registros!"
                };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            try
            {
                Clientes clientes = db.Clientes.Find(id);
                db.Clientes.Remove(clientes);
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
