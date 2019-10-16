using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CFC.Models
{
    [MetadataType(typeof(CidadesMetadata))]
    public partial class Cidades
    {
        class CidadesMetadata
        {
            [HiddenInput]
            public Int32 CodigoDaCidade { get; set; }

            [DisplayName("Cidade")]
            [Required]
            [StringLength(100)]
            public String NomeDaCidade { get; set; }

            [DisplayName("Estado")]
            [Required]
            public Int32 CodigoDoEstado { get; set; }
        }
    }
}
