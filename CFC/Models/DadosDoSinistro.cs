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
    
    public partial class DadosDoSinistro
    {
        public int CodigoDadosDoSinistro { get; set; }
        public int CodigoDoSinistro { get; set; }
        public int CodigoDoCliente { get; set; }
        public int CodigoDoItem { get; set; }
        public decimal ValorDoItem { get; set; }
        public System.DateTime ClienteCadastrado { get; set; }
        public bool ClienteInativo { get; set; }
        public System.DateTime ItemCadastrado { get; set; }
        public Nullable<System.DateTime> ItemInativo { get; set; }
        public System.DateTime DataDoSinistro { get; set; }
    }
}
