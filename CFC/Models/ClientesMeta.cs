using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CFC.Models
{
    [MetadataType(typeof(ClientesMetadata))]
    public partial class Clientes
    {
        class ClientesMetadata
        {
            [HiddenInput]
            public Int32 CodigoDoCliente { get; set; }

            [DisplayName("CFC")]
            [Required]
            [StringLength(100)]
            public String NomeCFC { get; set; }

            [DisplayName("Nome do Cliente")]
            [Required]
            [StringLength(100)]
            public String NomeCompletoDoCliente { get; set; }

            [DisplayName("Tipo")]
            [Required]
            [StringLength(4)]
            public String TipoDePessoaDoCliente { get; set; }

            [DisplayName("CNPJ/CPF")]
            [Required]
            [StringLength(18)]
            public String CPFCNPJDoCliente { get; set; }

            [DisplayName("Endereço")]
            [Required]
            [StringLength(100)]
            public String EnderecoDoCliente { get; set; }

            [DisplayName("Numero")]
            [StringLength(20)]
            public String NumeroDoCliente { get; set; }

            [DisplayName("Complemento")]
            [StringLength(50)]
            public String ComplementoDoCliente { get; set; }

            [DisplayName("Bairro")]
            [Required]
            [StringLength(100)]
            public String BairroDoCliente { get; set; }

            [DisplayName("Cidade")]
            [Required]
            public Int32 CodigoDaCidade { get; set; }

            [DisplayName("Estado")]
            [Required]
            public Int32 CodigoDoEstado { get; set; }

            [DisplayName("CEP")]
            [Required]
            [StringLength(9)]
            public String CEPDoCliente { get; set; }

            [DisplayName("Email")]
            [StringLength(100)]
            public String EmailDoCliente { get; set; }

            [DisplayName("Senha")]
            [StringLength(8)]
            public int SenhaDoCliente { get; set; }

            [DisplayName("Telefone")]
            [Required]
            [StringLength(15)]
            public String TelefoneResidencialDoCliente { get; set; }

            [DisplayName("Celular")]
            [StringLength(15)]
            public String TelefoneCelularDoCliente { get; set; }

            [DisplayName("Ativo")]
            [Required]
            public Boolean ClienteAtivo { get; set; }

            [DisplayName("Separar Veiculos")]
            [Required]
            public Boolean SepararVeiculos { get; set; }

            [DisplayName("Observações")]
            [StringLength(8000)]
            public String ObservacoesDoCliente { get; set; }

            [DisplayName("Cadastrado")]
            [Required]
            public DateTime? DataDoCadastro { get; set; }
        }
    }
}
