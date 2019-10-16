using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CFC.Models
{
    [MetadataType(typeof(HistoricosMeta))]
    public partial class Historicos
    {
    }

    public class HistoricosMeta
    {
        [Display(Name = "Código")]
        public int CodigoDoHistorico { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(100, ErrorMessage = "Limite para o campo 'Descrição' é de '100' caracteres")]
        [Required(ErrorMessage = "O Campo 'Descrição' é Obrigatório!")]
        public string DescricaoDoHistorico { get; set; }

        [Display(Name = "Ativo")]
        public bool HistoricoAtivo { get; set; }
    }
}