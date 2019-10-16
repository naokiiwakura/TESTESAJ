using CFC.Uteis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CFC.Models;
using static CFC.Uteis.DadosSeguros;

namespace CFC.Areas.Admin.Controllers
{    
    public class HomeController : Controller
    {
         private cfcadminEntities db = new cfcadminEntities();

        [Authorize(Roles = "Admin,Operador")]        
        public ActionResult Painel()
        {
            //Session["menu"] = "home";
            var itens = db.Itens;
            ViewBag.CotasAtivas = itens.Any(x => x.DataDaDesativacao == null) ? itens.Where(x => x.DataDaDesativacao == null).Sum(x => x.Valor)/10000 : 0;
            ViewBag.CotasInativas = itens.Any(x => x.DataDaDesativacao != null) ? itens.Where(x => x.DataDaDesativacao != null).Sum(x => x.Valor)/10000 : 0;

            ViewBag.ItensAtivos = itens.Count(x => x.DataDaDesativacao == null);
            ViewBag.ItensInativos = itens.Count(x => x.DataDaDesativacao != null);

            ViewBag.Cfc = itens.Count(x => !x.Particular && x.DataDaDesativacao == null);
            ViewBag.Particular = itens.Count(x => x.Particular && x.DataDaDesativacao == null);

            var clientes = db.Clientes;
            ViewBag.ClientesAtivos = clientes.Count(x => x.ClienteAtivo);
            ViewBag.ClientesInativos = clientes.Count(x => !x.ClienteAtivo);
            return View();
        }

        [Authorize(Roles = "Admin,Operador")]
        public JsonResult Lista(jQueryDataTableParamModel param, int itens = 0)
        {
            //var veiculos =  from x in db.Itens select x;

            //if (itens != 0)
            //{
            //    switch (itens)
            //    {
            //        case 1:
            //            veiculos = veiculos.Where(x => x.Particular);
            //            break;
            //        case 2:
            //            veiculos = veiculos.Where(x => !x.Particular);
            //            break;                   
            //    }
            //}

            var data = (from x in db.Clientes.ToList()
                        join i in db.Itens on x.CodigoDoCliente equals i.CodigoDocliente into i1
                        select new
                        {                    
                            x.CodigoDoCliente,
                            NomeCFC = x.NomeCFC + "<br>" + x.NomeCompletoDoCliente,
                            x.Cidades.NomeDaCidade,
                            x.Estados.NomeDoEstado, 
                            x.CPFCNPJDoCliente,
                            Ativa = x.ClienteAtivo ? "Sim" : "Não",   
                            Itens = i1.Count().ToString(),
                            Cotas = i1.Sum(z => z.Valor)/10000
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
                                string.Empty,
                                x.CodigoDoCliente.ToString(),
                                x.NomeCFC,
                                x.NomeDaCidade,
                                x.NomeDoEstado,                                
                                x.Ativa,
                                x.Itens,
                                x.Cotas.ToString()
                        
                        }
            },
                    JsonRequestBehavior.AllowGet);
    }

        [Authorize(Roles = "Admin,Operador")]
        public PartialViewResult GetRowDetail(int id)
        {
        return PartialView("RowDetail", db.Itens.Where(x => x.CodigoDocliente.Equals(id)));
        }

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
            if (!Response.Login(usuario, password, false)) return RedirectToAction("Index", new { id = 1 });
            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Painel", "Home");            
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            DadosSeguros.LogOff();
            return RedirectToAction("Index", "Home");
        }
    }
}