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
    [Authorize(Roles = "Admin")]
    public class CidadesController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();

        // GET: Admin/Cidades
        public ActionResult Index()
        {
            //var cidades = db.Cidades.Include(c => c.Estados);
            return View();
        }

        public JsonResult Lista(jQueryDataTableParamModel param)
        {


            var data = (from x in db.Cidades.ToList()
            select new
            {
                x.CodigoDaCidade,
                x.NomeDaCidade,
                x.Estados.NomeDoEstado
            });

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                param.sSearch = param.sSearch.ToUpper();
                data = data.Where(c => c.NomeDaCidade.Contains(param.sSearch) || c.NomeDoEstado.Contains(param.sSearch));
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
                             x.CodigoDaCidade.ToString(),
                             x.NomeDaCidade,
                             x.NomeDoEstado,
                        }
            },
                    JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Cidades/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Cidades cidades = db.Cidades.Find(id);
        //    if (cidades == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(cidades);
        //}

        // GET: Admin/Cidades/Create
        public ActionResult Create()
        {
            ViewBag.CodigoDoEstado = new SelectList(db.Estados, "CodigoDoEstado", "NomeDoEstado");
            return View();
        }

        // POST: Admin/Cidades/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CodigoDaCidade,NomeDaCidade,CodigoDoEstado")] Cidades cidades)
        {
            if (ModelState.IsValid)
            {
                cidades.NomeDaCidade = cidades.NomeDaCidade.ToUpper();
                db.Cidades.Add(cidades);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CodigoDoEstado = new SelectList(db.Estados, "CodigoDoEstado", "NomeDoEstado", cidades.CodigoDoEstado);
            return View(cidades);
        }

        // GET: Admin/Cidades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cidades cidades = db.Cidades.Find(id);
            if (cidades == null)
            {
                return HttpNotFound();
            }
            ViewBag.CodigoDoEstado = new SelectList(db.Estados, "CodigoDoEstado", "NomeDoEstado", cidades.CodigoDoEstado);
            return View(cidades);
        }

        // POST: Admin/Cidades/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CodigoDaCidade,NomeDaCidade,CodigoDoEstado")] Cidades cidades)
        {
            if (ModelState.IsValid)
            {
                cidades.NomeDaCidade = cidades.NomeDaCidade.ToUpper();
                db.Entry(cidades).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CodigoDoEstado = new SelectList(db.Estados, "CodigoDoEstado", "NomeDoEstado", cidades.CodigoDoEstado);
            return View(cidades);
        }

        // GET: Admin/Cidades/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Cidades cidades = db.Cidades.Find(id);
        //    if (cidades == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(cidades);
        //}

        // POST: Admin/Cidades/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public JsonResult DeleteConfirmed(int id)
        {
            var response = new
            {
                success = true,
                messages = "Registro excluido!"

            };

            try
            {
                Cidades cidades = db.Cidades.Find(id);
                db.Cidades.Remove(cidades);
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
