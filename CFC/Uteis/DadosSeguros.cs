using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CFC.Models;
using System.Web.Security;
using System.Web.Mvc;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Data.Entity.Validation;
using System.Security.Principal;
using System.Text;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;

namespace CFC.Uteis
{
    
    public static class DadosSeguros
    {
        //private static cfcadminEntities db = new cfcadminEntities();        

        public static string Versao => "1.3.8";

        public static string Info
        {
            get
            {
                const string info =
                    "<li><p><a href='#'>3.8</a> Modificado <a href='#'Impressão de boletos de 3 em 3 (carnê)</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>3.8</a> Adicionado <a href='#'>EDITAR Clientes nova opção Separar Veiculos na geração da Gestão e Rateio</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>3.7</a> Adicionado <a href='#'>Adicionado parcelamento do Sinistro em Gestão</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>3.6</a> Adicionado <a href='#'>Agrupamento do Rateio a Geração da Gestão</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>3.5</a> Corrigido <a href='#'>Geração de Caixa e Gestão</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>3.4</a> Novo <a href='#'>Adicionado Sinistro e Simulação</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>3.3</a> Alterado <a href='#'>Adequação de Retorno e Remessa</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>3.2</a> Novo <a href='#'>Liberado Processamento de Retorno</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>3.1</a> Novo <a href='#'>Liberado Gerar Gestão</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>2.9</a> Novo <a href='#'>Liberado Financeiro para Clientes</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>";
                    //"<li><p><a href='#'>2.8</a> Inclusão <a href='#'>Boletos agrupados 3 por folha impressa</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>2.7</a> Inclusão <a href='#'>Incluido valor na lista de Remessa</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>2.6</a> Alterado <a href='#'>Lista Cliente Gerar Caixa exibe referencia de inativos</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>254</a> Corrigido <a href='#'>Corrigida data no Caixa Gerado</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>2.4</a> Corrigido <a href='#'>Corrigida data na Movimentação</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>2.3</a> Alterado <a href='#'>Alterado caixa para boleto por Item</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>2.2</a> Novo <a href='#'>Adicionado Financeiro</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>2.1</a> Novo <a href='#'>Adicionada Configurações</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>1.2</a> Novo <a href='#'>Correções Diversas</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>";
                    //"<li><p><a href='#'>1.1</a> Novo <a href='#'>Inclusão controle de Usuários</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>1.0</a> Novo <a href='#'>Inclusão Área de MKT</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>0.9</a> Novo <a href='#'>Inclusão Área do Cliente</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>0.8</a> Novo <a href='#'>Inclusão do Site</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>0.7</a> Novo <a href='#'>Adicionado bloqueio de CNPJ repetido</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>";
                    //"<li><p><a href='#'>0.6</a> Novo <a href='#'>Contador de fotos na visão geral</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>0.5</a> Estética <a href='#'>Adicionado nomes dos filtros</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>0.4</a> Novo <a href='#'>Editar Item e Clientes pelo Detalhe</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>0.3</a> Estética <a href='#'>Detalhes clique no Código</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>0.2</a> Nova <a href='#'>Home</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>0.1</a> Correção <a href='#'>na páginação</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>";

                return info;
            }
        }

        public static string InfoCli
        {
            get
            {
                const string info =
                    "<li><p><a href='#'>2.9</a> Novo <a href='#'>Liberado Financeiro</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>1.2</a> Novo <a href='#'>Correções Diversas</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +                    
                    "<li><p><a href='#'>0.9</a> Novo <a href='#'>Inclusão Área do Cliente</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>0.8</a> Novo <a href='#'>Inclusão do Site</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>";                                   

                return info;
            }
        }

        public static string InfoMkt
        {
            get
            {
                const string info =
                    "<li><p><a href='#'>1.2</a> Novo <a href='#'>Correções Diversas</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>1.1</a> Novo <a href='#'>Inclusão controle de Usuários</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    "<li><p><a href='#'>1.0</a> Novo <a href='#'>Inclusão Área de MKT</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>";
                    //"<li><p><a href='#'>0.9</a> Novo <a href='#'>Inclusão Área do Cliente</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>0.8</a> Novo <a href='#'>Inclusão do Site</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                    //"<li><p><a href='#'>0.7</a> Novo <a href='#'>Adicionado bloqueio de CNPJ repetido</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>";
                //"<li><p><a href='#'>0.6</a> Novo <a href='#'>Contador de fotos na visão geral</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                //"<li><p><a href='#'>0.5</a> Estética <a href='#'>Adicionado nomes dos filtros</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                //"<li><p><a href='#'>0.4</a> Novo <a href='#'>Editar Item e Clientes pelo Detalhe</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                //"<li><p><a href='#'>0.3</a> Estética <a href='#'>Detalhes clique no Código</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                //"<li><p><a href='#'>0.2</a> Nova <a href='#'>Home</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>" +
                //"<li><p><a href='#'>0.1</a> Correção <a href='#'>na páginação</a><span class='timeline-icon'><i class='fa fa-file-text-o'></i></span></p></li>";

                return info;
            }
        }

        public static int Codigo
        {
            get
            {
                cfcadminEntities db = new cfcadminEntities();
                var codigo = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
                var cod = db.Clientes.Find(codigo);
                return cod?.CodigoDoCliente ?? 0; 
            }
        }
        //sistema
        public static bool Login(this HttpResponseBase response, string usuario, string senha, bool isPersistent)
        {
            cfcadminEntities ctx = new cfcadminEntities();
            Usuarios usu = ctx.Usuarios.FirstOrDefault(x => x.Email == usuario && x.Senha == senha);

            var roles = new List<string>();

            if (usu == null) return false;

            if (!usu.UsuarioAtivo) return false;


            if (usu.Admin)
                roles.Add("Admin");
            if (usu.Operador)
                roles.Add("Operador");
            var codigo = usu.CodigoDoUsuario;


            string userData = string.Join(",", roles.ToArray());

            var now = DateTime.Now;

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                codigo.ToString(),
                now,
                now.Add(FormsAuthentication.Timeout),
                isPersistent,
                userData,
              FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);

            response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            return true;
        }

        //clientes
        public static bool LoginCliente(this HttpResponseBase response, string usuario, string senha, bool isPersistent)
        {
            cfcadminEntities ctx = new cfcadminEntities();
            Clientes cli = ctx.Clientes.FirstOrDefault(x => x.CPFCNPJDoCliente.Replace(".", "").Replace("-", "").Replace("/", "").Replace(" ", "") == usuario && x.SenhaDoCliente == senha);

            var roles = new List<string>();

            if (cli == null) return false;

            if (!cli.ClienteAtivo) return false;


            roles.Add("Cliente");
            var codigoDoCliente = cli.CodigoDoCliente;


            string userData = string.Join(",", roles.ToArray());

            var now = DateTime.Now;

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                codigoDoCliente.ToString(),
                now,
                now.Add(FormsAuthentication.Timeout),
                isPersistent,
                userData,
              FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);

            response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            return true;
        }

        //mkt
        public static bool LoginMkt(this HttpResponseBase response, string usuario, string senha, bool isPersistent)
        {
            cfcadminEntities ctx = new cfcadminEntities();
            Usuarios usu = ctx.Usuarios.FirstOrDefault(x => x.Email == usuario && x.Senha == senha);

            var roles = new List<string>();

            if (usu == null) return false;

            if (!usu.UsuarioAtivo) return false;

            if(usu.Site)
            roles.Add("Site");

            var codigo = usu.CodigoDoUsuario;


            string userData = string.Join(",", roles.ToArray());

            var now = DateTime.Now;

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                codigo.ToString(),
                now,
                now.Add(FormsAuthentication.Timeout),
                isPersistent,
                userData,
              FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);

            response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            return true;
        }

        public static void LogOff()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
        }

        public static string Itens
        {
            get
            {
                var db = new cfcadminEntities();
                return db.Itens.Count(x => x.DataDaDesativacao == null).ToString();
            }
        }

        public static string Clientes
        {
            get
            {
                var db = new cfcadminEntities();
                return db.Clientes.Count(x => x.ClienteAtivo).ToString();
            }
        }

        public static string Nivel
        {
            get
            {
                var retorno = "";
                if (HttpContext.Current.User.IsInRole("Admin"))
                {
                    retorno = "Admin";
                }                
                if (HttpContext.Current.User.IsInRole("Site"))
                {
                    retorno = "Site";
                }
                if (HttpContext.Current.User.IsInRole("Operador"))
                {
                    retorno = "Operador";
                }

                return retorno;
            }
        }
    }

    //public static class Substitui
    //{
    //    public static string Caracteres(string texto, int codigoDaProposta)
    //    {
            //gpcgEntities db = new gpcgEntities();
            //var proposta = db.Propostas.Find(codigoDaProposta);
            //var cliente = db.Clientes.Find(proposta.CodigoDoCliente);
            //var veiculo = db.Itens.Find(proposta.CodigoDoItem);

            //var novoTexto = texto.Replace("{nomeRazaoSocial}", cliente.NomeCompletoDoCliente == null ? "" : cliente.NomeCompletoDoCliente.ToUpper())
            //        .Replace("{cpfCnpj}", cliente.CPFCNPJDoCliente)
            //        .Replace("{enderecoCompleto}", cliente.EnderecoDoCliente.ToUpper() + ", " + (cliente.NumeroDoCliente == null ? "" : cliente.NumeroDoCliente + ", ") + (cliente.ComplementoDoCliente == null ? "" : cliente.ComplementoDoCliente.Length < 1 ? "" : cliente.ComplementoDoCliente.ToUpper() + ", ") + (cliente.BairroDoCliente == null ? "" : cliente.BairroDoCliente.ToUpper()))
            //        .Replace("{cepCidadeEstado}", cliente.CEPDoCliente + " - " + (cliente.CidadeDoCliente == null ? "" : cliente.CidadeDoCliente.ToUpper()) + "/" + cliente.UFDoCliente)
            //        .Replace("{datahoje}", DateTime.Today.ToShortDateString())
            //        .Replace("{condutor}", proposta.Condutor == null ? "" : "1-  " + proposta.Condutor.ToUpper())
            //        .Replace("{condutor2}", proposta.Condutor2 == null ? "" : "2- " + proposta.Condutor2.ToUpper())
            //        .Replace("{veiculo}", veiculo.Marca.DescricaoDaMarca.ToUpper() + "/" + veiculo.DescricaoDoModelo.ToUpper() + "- ANO " + veiculo.AnoDeFabricacao + "/" + veiculo.AnoDoModelo)
            //        .Replace("{corPlaca}", veiculo.CorPredominante.ToUpper() + " / " + veiculo.Placa1DoVeiculo.ToUpper())
            //        .Replace("{chassi}", "CHASSI " + veiculo.NumeroDoChassi.ToUpper())
            //        .Replace("{terceiro}", proposta.EmpresaTerceiro == null ? "" : "COBERTURAS DE TERCEIROS (RFC) - " + proposta.EmpresaTerceiro);                              
            //return novoTexto;
    //    }
    //}

   [Flags]
    public enum EPermissoes
    {
        Logado = 0,
        Consulta = 1,
        Editar = 2,
        Excluir = 4,
        Incluir = 8,
        ImprimirRelatorios = 16,
        OperacoesEspeciais = 32
    }

    public class RequerPermissaoAttribute : AuthorizeAttribute
    {
        readonly EPermissoes _permissoesRequeridas;
        readonly string _formulario;

        public RequerPermissaoAttribute()
            : this(EPermissoes.Logado)
        {
        }

        public RequerPermissaoAttribute(EPermissoes permissoes)
            : this(permissoes, string.Empty)
        {
        }

        public RequerPermissaoAttribute(EPermissoes permissoesRequeridas, string formulario)
        {
            _permissoesRequeridas = permissoesRequeridas;
            _formulario = formulario ?? string.Empty;
        }

        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{
        //    var result = base.AuthorizeCore(httpContext); //verifica se está autenticado
        //    if (!result)
        //        return false;

        //    if (DadosSeguros.Nivel == "Admin") //se for master, permite tudo
        //        return true;

        //    EPermissoes permissoesEfetivas = EPermissoes.Logado;

        //    var usuario = DadosSeguros.Codigo;

        //    string currentController = _formulario;
        //    if (string.IsNullOrEmpty(currentController))
        //    {
        //        var rd = httpContext.Request.RequestContext.RouteData;
        //        //string currentAction = rd.GetRequiredString("action");
        //        currentController = rd.GetRequiredString("controller");
        //        //string currentArea = rd.Values["area"] as string;
        //    }

        //    //using (var ctx = new sbmsiteEntities())
        //    //{
        //    //    var dbPermissoes = ctx.Permissoes.FirstOrDefault(x => x.Formularios.NomeDoFormulario == currentController && x.CodigoDoUsuario == usuario && x.Formularios.FormularioAtivo == true);
        //    //    if (dbPermissoes != null)
        //    //    {
        //    //        if (dbPermissoes.PermiteConsulta)
        //    //            permissoesEfetivas = permissoesEfetivas | EPermissoes.Consulta;
        //    //        if (dbPermissoes.PermiteEditar)
        //    //            permissoesEfetivas = permissoesEfetivas | EPermissoes.Editar;
        //    //        if (dbPermissoes.PermiteExcluir)
        //    //            permissoesEfetivas = permissoesEfetivas | EPermissoes.Excluir;
        //    //        if (dbPermissoes.PermiteIncluir)
        //    //            permissoesEfetivas = permissoesEfetivas | EPermissoes.Incluir;
        //    //        if (dbPermissoes.PermiteImprimirRelatorios)
        //    //            permissoesEfetivas = permissoesEfetivas | EPermissoes.ImprimirRelatorios;
        //    //        if (dbPermissoes.PermiteOperacoesEspeciais)
        //    //            permissoesEfetivas = permissoesEfetivas | EPermissoes.OperacoesEspeciais;
        //    //    }
        //    //}

        //    var temAcesso = (permissoesEfetivas & _permissoesRequeridas) == _permissoesRequeridas;
        //    return temAcesso;

            
        //}

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var autorizado = AuthorizeCore(filterContext.HttpContext);
            if (autorizado)
                return;

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "PermissoesInsuficientes",
                    controller = "Home",
                }));

                filterContext.Result = result;
            }
            else HandleUnauthorizedRequest(filterContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //if ajax request set status code and end responcse
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 401;
                filterContext.HttpContext.Response.End();
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
    }

    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Home/PermissoesInsuficientes" }));
            }
        }
    }

    public class FormattedDbEntityValidationException : Exception
    {
        public FormattedDbEntityValidationException(DbEntityValidationException innerException) :
            base(null, innerException)
        {
        }

        public override string Message
        {
            get
            {
                var innerException = InnerException as DbEntityValidationException;
                if (innerException != null)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine();
                    sb.AppendLine();
                    foreach (var eve in innerException.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                ve.PropertyName,
                                eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                ve.ErrorMessage));
                        }
                    }
                    sb.AppendLine();

                    return sb.ToString();
                }

                return base.Message;
            }
        }
    }
}