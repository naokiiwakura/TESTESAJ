//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CFC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Configuracoes
    {
        public int Codigo { get; set; }
        public string CnpjDaEmpresa { get; set; }
        public string RazaoSocialDaEmpresa { get; set; }
        public string NumeroDaAgenciaBancaria { get; set; }
        public string DigitoDaAgenciaBancaria { get; set; }
        public string NumeroDaContaBancaria { get; set; }
        public string DigitoDaContaBancaria { get; set; }
        public int UltimaRemessa { get; set; }
        public decimal ValorDaMensalidade { get; set; }
        public decimal TaxaDoBoleto { get; set; }
        public string TextoMulta { get; set; }
        public string TextoMora { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string SMTP { get; set; }
        public bool DisparaEmail { get; set; }
        public string TextoGestao { get; set; }
        public string TextoTerceiro { get; set; }
        public decimal ValorTerceiro { get; set; }
    }
}
