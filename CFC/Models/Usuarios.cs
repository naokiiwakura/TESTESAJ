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
    
    public partial class Usuarios
    {
        public int CodigoDoUsuario { get; set; }
        public string NomeDoUsuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool UsuarioAtivo { get; set; }
        public string FotoDoUsuario { get; set; }
        public System.DateTime DataDoCadastro { get; set; }
        public bool Admin { get; set; }
        public bool Site { get; set; }
        public bool Operador { get; set; }
    }
}
