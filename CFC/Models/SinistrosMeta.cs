using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CFC.Models
{
    [MetadataType(typeof(SinistrosMeta))]
    public partial class Sinistros
    {

    }


    public class SinistrosMeta
    {

        [Display(Name = "Item")]
        [Required(ErrorMessage = "O Item é Obrigatório!")]
        public int CodigoDoItem { get; set; }

        [Display(Name = "Cliente")]
        [Required(ErrorMessage = "O Cliente é Obrigatório!")]
        public int CodigoDoCliente { get; set; }

        [Display(Name = "Valor")]
        [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido")]
        [Required(ErrorMessage = "O Valor é Obrigatório!")]
        public decimal ValorDoSinistro { get; set; }

        [Display(Name = "Valor de Referência")]
        [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido")]
        [Required(ErrorMessage = "O Valor é Obrigatório!")]
        public decimal ValorDeReferencia { get; set; }

        [Display(Name = "Cotas na Data")]
        //[DisplayFormat(DataFormatString = "{0:#.####}")]
        //[RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido")]
        //[Required(ErrorMessage = "O Valor é Obrigatório!")]
        public decimal CotasNaData { get; set; }

        [Display(Name = "Valor por Cota")]
        //[RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido")]
        //[Required(ErrorMessage = "O Valor é Obrigatório!")]
        public decimal ValorPorCota { get; set; }

        [Display(Name = "Data do Sinistro")]
        [Required(ErrorMessage = "A Data do Sinistro é Obrigatória!")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataDoSinistro { get; set; }

        [Display(Name = "Data de Conclusão")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataDeConclusaoDoSinistro { get; set; }

        [Display(Name = "Observação")]
        [StringLength(8000, ErrorMessage = "Limite para o campo 'Descrição' é de '8000' caracteres")]
        public string ObservacaoDoSinistro { get; set; }

        [Display(Name = "Data Faturamento")]
        public DateTime? DataDoFaturamento { get; set; }

        [Display(Name = "Foto 1")]       
        public string Foto1 { get; set; }

        [Display(Name = "Foto 2")]       
        public string Foto2 { get; set; }

        [Display(Name = "Foto 3")]       
        public string Foto3 { get; set; }

        [Display(Name = "Foto 4")]       
        public string Foto4 { get; set; }

        [Display(Name = "Foto 5")]       
        public string Foto5 { get; set; }

        [Display(Name = "Foto 6")]       
        public string Foto6 { get; set; }

        [Display(Name = "Foto 7")]       
        public string Foto7 { get; set; }

        [Display(Name = "Foto 8")]       
        public string Foto8 { get; set; }
    }
}