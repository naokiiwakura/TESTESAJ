using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CFC.Models
{
    [MetadataType(typeof(ConfiguracoesMetadata))]
    public partial class Configuracoes
    {
        class ConfiguracoesMetadata
        {
            [HiddenInput]
            public Int32 Codigo { get; set; }

            [DisplayName("CNPJ")]
            [Required]
            [StringLength(20)]
            public String CnpjDaEmpresa { get; set; }

            [DisplayName("Razao Social")]
            [Required]
            [StringLength(100)]
            public String RazaoSocialDaEmpresa { get; set; }

            [DisplayName("Agência")]
            [Required]
            [StringLength(10)]
            public String NumeroDaAgenciaBancaria { get; set; }

            [DisplayName("Digito")]
            [Required]
            [StringLength(2)]
            public String DigitoDaAgenciaBancaria { get; set; }

            [DisplayName("Conta")]
            [Required]
            [StringLength(10)]
            public String NumeroDaContaBancaria { get; set; }

            [DisplayName("Digito")]
            [Required]
            [StringLength(2)]
            public String DigitoDaContaBancaria { get; set; }

            [DisplayName("Ultima Remessa")]
            [Required]
            public Int32 UltimaRemessa { get; set; }

            [DisplayName("Mensalidade")]
            [Required]
            public Decimal ValorDaMensalidade { get; set; }

            [DisplayName("Taxa Do Boleto")]
            [Required]
            public Decimal TaxaDoBoleto { get; set; }

            [DisplayName("Texto Multa")]
            [StringLength(150)]
            public String TextoMulta { get; set; }

            [DisplayName("Texto Mora")]
            [StringLength(150)]
            public String TextoMora { get; set; }

            [DisplayName("Email")]
            [EmailAddress]
            [StringLength(250)]
            public String Email { get; set; }

            [DisplayName("Senha")]
            [StringLength(50)]
            public String Senha { get; set; }

            [DisplayName("SMTP")]
            [StringLength(50)]
            public String SMTP { get; set; }

            [DisplayName("Disparar Email")]
            [Required]
            public Boolean DisparaEmail { get; set; }

            [DisplayName("Terceiro")]
            [Required]
            public Decimal ValorTerceiro { get; set; }

            [DisplayName("Texto Gestão")]
            [StringLength(150)]
            public String TextoGestao { get; set; }

            [DisplayName("Texto Terceiro")]
            [StringLength(150)]
            public String TextoTerceiro { get; set; }
        }
    }
}
