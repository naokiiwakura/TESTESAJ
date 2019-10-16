using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CFC.Models
{
    [MetadataType(typeof(SiteMetadata))]
    public partial class Site
    {
        class SiteMetadata
        {
            [DisplayName("Ativa Site")]
            [Required]
            public Boolean AtivaSite { get; set; }

            [DisplayName("Ativa")]
            [Required]
            public Boolean AtivaBanner { get; set; }

            [DisplayName("Ativa")]
            [Required]
            public Boolean AtivaCobertura { get; set; }

            [DisplayName("Ativa")]
            [Required]
            public Boolean AtivaServicos { get; set; }

            [DisplayName("Ativa")]
            [Required]
            public Boolean AtivaVeiculos { get; set; }

            [DisplayName("Ativa Login")]
            [Required]
            public Boolean AtivaLogin { get; set; }

            [DisplayName("Ativa")]
            [Required]
            public Boolean AtivaSobre { get; set; }

            [DisplayName("Ativa Contato")]
            [Required]
            public Boolean AtivaContato { get; set; }

            [DisplayName("Titulo")]
            [StringLength(150)]
            public String SobreTitulo { get; set; }

            [DisplayName("Resumo")]
            [StringLength(250)]
            public String SobreResumo { get; set; }

            [DataType(DataType.MultilineText)]
            [DisplayName("Texto")]
            public String VeiculosTexto { get; set; }

            [DataType(DataType.MultilineText)]
            [DisplayName("Texto")]
            public String CoberturaTexto { get; set; }

            [DataType(DataType.MultilineText)]
            [DisplayName("Texto")]
            public String PrecoTexto { get; set; }

            [DataType(DataType.MultilineText)]
            [DisplayName("Texto")]
            public String ServicosTexto { get; set; }

            [DisplayName("Titulo")]
            [StringLength(50)]
            public String Area1Titulo { get; set; }

            [DisplayName("Texto")]
            [StringLength(250)]
            public String Area1Texto { get; set; }

            [DisplayName("Titulo")]
            [StringLength(50)]
            public String Area2Titulo { get; set; }

            [DisplayName("Texto")]
            [StringLength(250)]
            public String Area2Texto { get; set; }

            [DisplayName("Cliente")]
            [StringLength(50)]
            public String Depoimento1Cliente { get; set; }

            [DisplayName("Texto")]
            [StringLength(250)]
            public String Depoimento1Texto { get; set; }

            [DisplayName("Cliente")]
            [StringLength(50)]
            public String Depoimento2Cliente { get; set; }

            [DisplayName("Texto")]
            [StringLength(250)]
            public String Depoimento2Texto { get; set; }

            [DisplayName("Cliente")]
            [StringLength(50)]
            public String Depoimento3Cliente { get; set; }

            [DisplayName("Texto")]
            [StringLength(250)]
            public String Depoimento3Texto { get; set; }

            [DisplayName("Cliente")]
            [StringLength(50)]
            public String Depoimento4Cliente { get; set; }

            [DisplayName("Texto")]
            [StringLength(250)]
            public String Depoimento4Texto { get; set; }

            [DisplayName("Ativa")]
            [Required]
            public Boolean AtivaDepoimento { get; set; }

            [DisplayName("Titulo")]
            [StringLength(20)]
            public String Contador1Titulo { get; set; }

            [DisplayName("Numero")]
            public Int32? Contador1Numero { get; set; }

            [DisplayName("Titulo")]
            [StringLength(20)]
            public String Contador2Titulo { get; set; }

            [DisplayName(" Numero")]
            public Int32? Contador2Numero { get; set; }

            [DisplayName("Titulo")]
            [StringLength(20)]
            public String Contador3Titulo { get; set; }

            [DisplayName("Numero")]
            public Int32? Contador3Numero { get; set; }

            [DisplayName("Titulo")]
            [StringLength(20)]
            public String Contador4Titulo { get; set; }

            [DisplayName("Numero")]
            public Int32? Contador4Numero { get; set; }

            [DisplayName("Ativa Parceiros")]
            [Required]
            public Boolean AtivaParceiros { get; set; }

            [DisplayName("Telefone")]
            [StringLength(50)]
            public String Telefone { get; set; }

            [DisplayName("Endereço")]
            [StringLength(150)]
            public String Endereco { get; set; }

            [DisplayName("Facebook")]
            [StringLength(500)]
            public String Facebook { get; set; }

            [DisplayName("Youtube")]
            [StringLength(500)]
            public String Youtube { get; set; }

            [DisplayName("Twitter")]
            [StringLength(500)]
            public String Twitter { get; set; }

            [DisplayName("Instagram")]
            [StringLength(500)]
            public String Instagram { get; set; }

            [DisplayName("Email")]
            [EmailAddress]
            [StringLength(50)]
            public String Email { get; set; }

            [DisplayName("Banner 1 Texto")]
            [StringLength(150)]
            public String Banner1Texto1 { get; set; }

            [DisplayName("Banner 1 Texto 2")]
            [StringLength(150)]
            public String Banner1Texto2 { get; set; }

            [DisplayName("Banner 1 Imagem")]
            [StringLength(150)]
            public String Banner1Imagem { get; set; }

            [DisplayName("Banner 1 Padrão")]
            [Required]
            public Boolean AtivaBanner1Padrao { get; set; }

            [DisplayName("Banner 2 Padrão")]
            [Required]
            public Boolean AtivaBanner2Padrao { get; set; }

            [DisplayName("Banner 2 Texto 1")]
            [StringLength(150)]
            public String Banner2Texto1 { get; set; }

            [DisplayName("Banner 2 Texto")]
            [StringLength(250)]
            public String Banner2Texto2 { get; set; }

            [DisplayName("Banner 2 Texto 3")]
            [StringLength(150)]
            public String Banner2Texto3 { get; set; }

            [DisplayName("Banner 2 Imagem")]
            [StringLength(150)]
            public String Banner2Imagem { get; set; }

            [HiddenInput]
            public Int32 CodigoDoSite { get; set; }

            [DataType(DataType.MultilineText)]
            [DisplayName("Texto")]
            public String SobreTexto { get; set; }

            [DisplayName("Sobre Imagem")]
            [StringLength(150)]
            public String SobreImagem { get; set; }

            [DisplayName("Ativa")]
            [Required]
            public Boolean AtivaPreco { get; set; }

            [DisplayName("Ativa")]
            [Required]
            public Boolean AtivaContador { get; set; }

            [DisplayName("Sobre Imagem Equipe")]
            [StringLength(150)]
            public String SobreImagemEquipe { get; set; }

            [DisplayName("Ativa")]
            [Required]
            public Boolean AtivaArea { get; set; }

            [DisplayName("Ativa Barra de Links")]
            [Required]
            public Boolean AtivaLinks { get; set; }
        }
    }
}
