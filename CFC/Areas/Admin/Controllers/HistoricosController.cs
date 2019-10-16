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
    public class HistoricosController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();

        // GET: Admin/Estados
        public ActionResult Index()
        {            
            return View();
        }

        //Lista
        //[HttpPost]
        public JsonResult Lista(jQueryDataTableParamModel param)
        {
            var data = (from x in db.Historicos.ToList()
            select new
            {
                x.CodigoDoHistorico,
                x.DescricaoDoHistorico,
                Ativo = x.HistoricoAtivo ? "Sim" : "Não"
            });

            if (!string.IsNullOrEmpty(param.sSearch))
            {
               data = data.Where(c => c.DescricaoDoHistorico.Contains(param.sSearch));
            }

            var result = data;
            if (param.iDisplayLength != -1)
            {
                result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            }

            return Json(new {
                param.sEcho,
                         iTotalRecords = db.Estados.Count(),
                         iTotalDisplayRecords = data.Count(),
                         aaData = from x in result select new[]
                         {
                             x.CodigoDoHistorico.ToString(),
                             x.DescricaoDoHistorico,
                             x.Ativo
                         }
            },
                    JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Estados/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Historicos historicos = db.Historicos.Find(id);
            if (historicos == null)
            {
                return HttpNotFound();
            }
            return View(historicos);
        }

        // GET: Admin/Estados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Estados/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Historicos historicos)
        {
            if (ModelState.IsValid)
            {
                historicos.DescricaoDoHistorico = historicos.DescricaoDoHistorico.ToUpper();
                db.Historicos.Add(historicos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(historicos);
        }

        // GET: Admin/Estados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Historicos historicos = db.Historicos.Find(id);
            if (historicos == null)
            {
                return HttpNotFound();
            }
            return View(historicos);
        }

        // POST: Admin/Estados/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Historicos historicos)
        {
            if (ModelState.IsValid)
            {
                historicos.DescricaoDoHistorico = historicos.DescricaoDoHistorico.ToUpper();
                db.Entry(historicos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(historicos);
        }

        // GET: Admin/Estados/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Estados estados = db.Estados.Find(id);
        //    if (estados == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(estados);
        //}

        // POST: Admin/Estados/Delete/5
        //[HttpPost, ActionName("Index")]
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
                Historicos historicos = db.Historicos.Find(id);
                db.Historicos.Remove(historicos);
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
