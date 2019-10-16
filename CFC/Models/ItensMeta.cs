using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CFC.Models
{
    [MetadataType(typeof(ItensMetadata))]
    public partial class Itens
    {
        class ItensMetadata
        {
            [HiddenInput]
            public Int32 CodigoDoItem { get; set; }

            [DisplayName("Cliente")]
            [Required]
            public Int32 CodigoDocliente { get; set; }

            [DisplayName("Marca")]
            [Required]
            [StringLength(100)]
            public String DescricaoDaMarca { get; set; }

            [DisplayName("Modelo")]
            [StringLength(200)]
            public String DescricaoDoModelo { get; set; }

            [DisplayName("Placa 1")]
            [Required]
            [StringLength(9)]
            public String Placa1DoVeiculo { get; set; }

            [DisplayName("Placa 2")]
            //[Required]
            [StringLength(9)]
            public String Placa2DoVeiculo { get; set; }

            [DisplayName("Renavam")]
            [StringLength(40)]
            public String Renavam { get; set; }

            [DisplayName("Chassi")]
            [StringLength(40)]
            public String NumeroDoChassi { get; set; }

            [DisplayName("Fabricação")]
            [Required]
            [StringLength(10)]
            public String AnoDeFabricacao { get; set; }

            [DisplayName("Modelo")]
            [Required]
            [StringLength(4)]
            public String AnoDoModelo { get; set; }

            [DisplayName("Cor")]
            [Required]
            [StringLength(50)]
            public String CorPredominante { get; set; }

            [DisplayName("Particular")]
            [Required]
            public Boolean Particular { get; set; }

            [DisplayName("Observaçao")]
            [StringLength(8000)]
            public String ObservacaoDoItem { get; set; }

            [DisplayName("Cadastrado")]
            [Required]
            public DateTime? DataDoCadastro { get; set; }

            [DisplayName("Desativado")]
            public DateTime? DataDaDesativacao { get; set; }

            [DisplayName("Fipe")]
            [Required]
            [StringLength(50)]
            public String CodigoFipe { get; set; }

            [DisplayName("Tipo")]
            [Required]
            public Int32 CodigoDoTipo { get; set; }

            [DisplayName("Foto 1")]
            [StringLength(100)]
            public String Foto1 { get; set; }

            [DisplayName("Foto 2")]
            [StringLength(100)]
            public String Foto2 { get; set; }

            [DisplayName("Foto 3")]
            [StringLength(100)]
            public String Foto3 { get; set; }

            [DisplayName("Foto 4")]
            [StringLength(100)]
            public String Foto4 { get; set; }

            [DisplayName("Ativado")]
            [Required]
            public DateTime? DataDaAtivacao { get; set; }

            [DisplayName("Valor")]
            //[DataType(DataType.Currency)]
            [Required]
            [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Valor inválido")]
            public Decimal? Valor { get; set; }
        }
    }
}
