using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CFC.Models
{
    [MetadataType(typeof(UsuariosMetadata))]
    public partial class Usuarios
    {
        class UsuariosMetadata
        {
            [HiddenInput]
            public Int32 CodigoDoUsuario { get; set; }

            [DisplayName("Nome")]
            [Required]
            [StringLength(100)]
            public String NomeDoUsuario { get; set; }

            [DisplayName("Email (Login)")]
            [Required]
            [EmailAddress]
            [StringLength(100)]
            public String Email { get; set; }

            [DisplayName("Senha")]
            [Required]
            [StringLength(10)]
            public String Senha { get; set; }

            [DisplayName("Ativo")]
            [Required]
            public Boolean UsuarioAtivo { get; set; }

            [DisplayName("Foto")]
            [StringLength(100)]
            public String FotoDoUsuario { get; set; }

            [DisplayName("Cadastrado")]
            [Required]
            public DateTime DataDoCadastro { get; set; }

            [DisplayName("Admin")]
            [Required]
            public Boolean Admin { get; set; }

            [DisplayName("Site")]
            [Required]
            public Boolean Site { get; set; }

            [DisplayName("Operador")]
            [Required]
            public Boolean Operador { get; set; }
        }
    }
}
