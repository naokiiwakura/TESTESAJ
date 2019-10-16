using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CFC.Models;
using CFC.Uteis;

namespace CFC.Areas.Mkt.Controllers
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
            //var numeros = new Regex(@"[^\d]");
            //usuario = numeros.Replace(usuario, "");

            if (!Response.LoginMkt(usuario, password, false)) return RedirectToAction("Index", new { id = 1 });
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }            
            return RedirectToAction("Site", "Home");
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            DadosSeguros.LogOff();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Site")]
        // GET: Admin/Site/Edit/5
        public ActionResult Site()
        {
            Site site = db.Site.FirstOrDefault();
            return View(site);
        }

        // POST: Admin/Site/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Site")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Site(Site site)
        {
            site.Banner1Imagem = UploadImagens.UploadRename(Request.Files["f1"], "Banner1Imagem", site.Banner1Imagem);
            site.Banner2Imagem = UploadImagens.UploadRename(Request.Files["f2"], "Banner2Imagem", site.Banner2Imagem);
            site.SobreImagem = UploadImagens.UploadRename(Request.Files["f3"], "SobreImagem", site.SobreImagem);
            site.SobreImagemEquipe = UploadImagens.UploadRename(Request.Files["f4"], "SobreImagemEquipe", site.SobreImagemEquipe);

            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(site).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["ok"] = "Informações salvas com Sucesso!";
                    return RedirectToAction("Site");
                }
                return View(site);
            }
            catch
            {
                TempData["erro"] = "Houve um erro ao salvar as informações!";
            }
            return RedirectToAction("Site");
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