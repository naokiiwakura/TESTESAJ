using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CFC.Models
{
    [MetadataType(typeof(MovimentacaoMetadata))]
    public partial class Movimentacao
    {
        class MovimentacaoMetadata
        {
            [HiddenInput]
            public Int32 CodigoDoTitulo { get; set; }

            [DisplayName("Descrição")]
            [Required]
            [StringLength(300)]
            public String DescricaoDoTitulo { get; set; }

            [DisplayName("Cliente")]
            public Int32? CodigoDoCliente { get; set; }

            [DisplayName("Tipo")]
            [Required]
            [StringLength(50)]
            public String TipoDoTitulo { get; set; }

            [DisplayName("Valor")]
            [Required]
            public Decimal ValorDoTitulo { get; set; }

            [DisplayName("Criação")]
            [Required]
            public DateTime DataDoTitulo { get; set; }

            [DisplayName("Vencimento")]
            [Required]
            public DateTime DataDoVencimento { get; set; }

            [DisplayName("Recebimento")]
            public DateTime? DataDoRecebimento { get; set; }

            [DisplayName("Nosso Numero")]
            [StringLength(30)]
            public String NossoNumero { get; set; }

            [DisplayName("Tipo Da Baixa")]
            [StringLength(50)]
            public String TipoDaBaixa { get; set; }

            [DisplayName("Boleto Gerado")]
            [Required]
            public Boolean GeradoBoleto { get; set; }

            [DisplayName("Remessa Gerada")]
            [Required]
            public Boolean GeradoRemessa { get; set; }

            [DisplayName("Observação 1")]
            [StringLength(150)]
            public String Observacao1 { get; set; }

            [DisplayName("Observação 2")]
            [StringLength(150)]
            public String Observacao2 { get; set; }

            [DisplayName("Observação 3")]
            [StringLength(150)]
            public String Observacao3 { get; set; }

            [DisplayName("Cancelado")]
            [Required]
            public Boolean Cancelado { get; set; }

            [DisplayName("Numero Parcela")]
            public Int32? NumeroParcela { get; set; }

            [DisplayName("Total Parcelas")]
            public Int32? TotalParcelas { get; set; }

            [DisplayName("Taxa Admin")]
            [Required]
            public Boolean BoletoAdmin { get; set; }

            [DisplayName("Crédito")]
            [Required]
            public Boolean Credito { get; set; }

            [DisplayName("Débito")]
            [Required]
            public Boolean Debito { get; set; }

            [DisplayName("Sinistro")]
            //[Required]
            public Int32 CodigoDoSinistro { get; set; }

            [DisplayName("Fornecedor")]
            //[Required]
            public Int32 CodigoDoFornecedor { get; set; }

            [DisplayName("Boleto")]
            [Required]
            public Boolean Boleto { get; set; }

            [DisplayName("Caixa")]
            [Required]
            public Boolean Caixa { get; set; }

            [DisplayName("Rateio")]
            [Required]
            public Boolean Rateio { get; set; }

            [DisplayName("Detalhes/Informações/Linha Digitavel/Nº do Documento")]
            [StringLength(8000)]
            public String Detalhes { get; set; }

            [DisplayName("Historico")]
            [Required]
            public Int32 CodigoDoHistorico { get; set; }

            [Display(Name = "Foto 1")]
            public string Foto1 { get; set; }

            [Display(Name = "Foto 2")]
            public string Foto2 { get; set; }

            [Display(Name = "Foto 3")]
            public string Foto3 { get; set; }

            [Display(Name = "Foto 4")]
            public string Foto4 { get; set; }
        }
    }
}
