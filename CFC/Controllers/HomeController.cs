using CFC.Uteis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CFC.Models;

namespace CFC.Controllers
{
    public class HomeController : Controller
    {
        private cfcadminEntities db = new cfcadminEntities();
        public ActionResult Index()
        {
            Site site = db.Site.FirstOrDefault();
            if (!site.AtivaSite)
            {
                RedirectToAction("Off");
            }
            return View(site);
        }

        public ActionResult Off()
        {
            return View();
        }

       public ActionResult Contato()
        {            

            return View();
        }

        public ActionResult Pagina(string id)
        {
            ViewBag.Pagina = id;     
            return View(db.Site.FirstOrDefault());
        }

        public ActionResult Cabecalho()
        {

            return PartialView(db.Site.FirstOrDefault());
        }

        public ActionResult Rodape()
        {

            return PartialView(db.Site.FirstOrDefault());
        }
    }
}