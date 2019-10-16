using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CFC.Models
{
    [MetadataType(typeof(EstadosMetadata))]
    public partial class Estados
    {
        class EstadosMetadata
        {
            [HiddenInput]
            public Int32 CodigoDoEstado { get; set; }

            [DisplayName("Estado")]
            [Required]
            [StringLength(2)]
            public String NomeDoEstado { get; set; }
        }
    }
}
