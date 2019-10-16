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
    public class SiteController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();

 
        // GET: Admin/Site/Edit/5
        public ActionResult Edit()
        {
            Site site = db.Site.FirstOrDefault();            
            return View(site);
        }

        // POST: Admin/Site/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Site site)
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
                    return RedirectToAction("Edit");
                }
                return View(site);
            }
            catch
            {
                TempData["erro"] = "Houve um erro ao salvar as informações!";
            }
            return RedirectToAction("Edit");
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
