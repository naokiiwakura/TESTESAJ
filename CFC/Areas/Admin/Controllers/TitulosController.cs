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
    public class TitulosController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();

        // GET: Admin/Titulos
        public ActionResult Index()
        {
            return View();
        }


        //public JsonResult Lista(jQueryDataTableParamModel param)
        //{


        //    var data = (from x in db.Titulos.ToList()                        
        //                select new
        //                {
        //                    x.CodigoDoTitulo,
        //                    x.Clientes.NomeCFC,
        //                    x.DescricaoDoTitulo,
        //                    x.TipoDoTitulo,
        //                    Ativa = x.ClienteAtivo ? "Sim" : "Não"
        //                });

        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        param.sSearch = param.sSearch.ToUpper();
        //        data = data.Where(c => c.NomeCFC.Contains(param.sSearch) || c.NomeDaCidade.Contains(param.sSearch) || c.NomeDoEstado.Contains(param.sSearch));
        //    }

        //    var result = data;
        //    if (param.iDisplayLength != -1)
        //    {
        //        result = result.Skip(param.iDisplayStart).Take(param.iDisplayLength);
        //    }

        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = db.Clientes.Count(),
        //        iTotalDisplayRecords = data.Count(),
        //        aaData = from x in result
        //                 select new[]
        //                {
        //                     x.CodigoDoCliente.ToString(),
        //                     x.NomeCFC,
        //                     x.NomeDaCidade,
        //                     x.NomeDoEstado,
        //                     x.Ativa
        //                }
        //    },
        //            JsonRequestBehavior.AllowGet);
        //}


        // GET: Admin/Titulos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Titulos titulos = db.Titulos.Find(id);
            if (titulos == null)
            {
                return HttpNotFound();
            }
            return View(titulos);
        }

        // GET: Admin/Titulos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Titulos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CodigoDoTitulo,DescricaoDoTitulo,CodigoDoCliente,TipoDoTitulo,ValorDoTitulo,DataDoTitulo,DataDoVencimento,DataDoRecebimento,NossoNumero,TipoDaBaixa,GeradoBoleto,GeradoRemessa,Observacao1,Observacao2,Observacao3,Cancelado,CodigoDaConta,EmailEnviado,NumeroParcela,TotalParcelas")] Titulos titulos)
        {
            if (ModelState.IsValid)
            {
                db.Titulos.Add(titulos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(titulos);
        }

        // GET: Admin/Titulos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Titulos titulos = db.Titulos.Find(id);
            if (titulos == null)
            {
                return HttpNotFound();
            }
            return View(titulos);
        }

        // POST: Admin/Titulos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CodigoDoTitulo,DescricaoDoTitulo,CodigoDoCliente,TipoDoTitulo,ValorDoTitulo,DataDoTitulo,DataDoVencimento,DataDoRecebimento,NossoNumero,TipoDaBaixa,GeradoBoleto,GeradoRemessa,Observacao1,Observacao2,Observacao3,Cancelado,CodigoDaConta,EmailEnviado,NumeroParcela,TotalParcelas")] Titulos titulos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(titulos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(titulos);
        }

        // GET: Admin/Titulos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Titulos titulos = db.Titulos.Find(id);
            if (titulos == null)
            {
                return HttpNotFound();
            }
            return View(titulos);
        }

        // POST: Admin/Titulos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Titulos titulos = db.Titulos.Find(id);
            db.Titulos.Remove(titulos);
            db.SaveChanges();
            return RedirectToAction("Index");
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
