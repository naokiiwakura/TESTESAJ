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
    public class UsuariosController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();

        // GET: Admin/Usuarios
        public ActionResult Index()
        {
            return View(db.Usuarios.ToList());
        }

        //Lista
        //[HttpPost]
        public JsonResult Lista(jQueryDataTableParamModel param)
        {
            var data = (from x in db.Usuarios.ToList()
                        select new
                        {
                            x.CodigoDoUsuario,
                            x.NomeDoUsuario,
                            x.Email,
                            Admin = x.Admin ? "Sim" : "Não",
                            Operador = x.Operador ? "Sim" : "Não",
                            Site = x.Site ? "Sim" : "Não",
                            Ativo = x.UsuarioAtivo ? "Sim" : "Não"
                        });

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                data = data.Where(c => c.NomeDoUsuario.Contains(param.sSearch) || c.Email.Contains(param.sSearch));
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
                             x.CodigoDoUsuario.ToString(),
                             x.NomeDoUsuario,
                             x.Email,
                             x.Admin,
                             x.Operador,
                             x.Site,
                             x.Ativo                             
                         }
            },
                    JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Usuarios/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Usuarios usuarios = db.Usuarios.Find(id);
        //    if (usuarios == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(usuarios);
        //}

        // GET: Admin/Usuarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Usuarios/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CodigoDoUsuario,NomeDoUsuario,Email,Senha,UsuarioAtivo,FotoDoUsuario,DataDoCadastro,Admin,Site,Operador,f1")] Usuarios usuarios)
        {
            usuarios.FotoDoUsuario = UploadImagens.UploadRename(Request.Files["f1"], "FotoDoUsuario", null);

            if (ModelState.IsValid)
            {
                usuarios.NomeDoUsuario = usuarios.NomeDoUsuario.ToUpper();
                db.Usuarios.Add(usuarios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuarios);
        }

        // GET: Admin/Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = db.Usuarios.Find(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        // POST: Admin/Usuarios/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CodigoDoUsuario,NomeDoUsuario,Email,Senha,UsuarioAtivo,FotoDoUsuario,DataDoCadastro,Admin,Site,Operador,f1")] Usuarios usuarios)
        {
            usuarios.FotoDoUsuario = UploadImagens.UploadRename(Request.Files["f1"], "FotoDoUsuario", usuarios.FotoDoUsuario);

            if (ModelState.IsValid)
            {
                usuarios.NomeDoUsuario = usuarios.NomeDoUsuario.ToUpper();
                db.Entry(usuarios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(usuarios);
        }

        // GET: Admin/Usuarios/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Usuarios usuarios = db.Usuarios.Find(id);
        //    if (usuarios == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(usuarios);
        //}

        //// POST: Admin/Usuarios/Delete/5
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
                Usuarios usuarios = db.Usuarios.Find(id);
                db.Usuarios.Remove(usuarios);
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
