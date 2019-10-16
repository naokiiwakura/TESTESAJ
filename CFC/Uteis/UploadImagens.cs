using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace CFC.Uteis
{
    public class UploadImagens : Controller
    {
        //Função para várias imagens em uma página
        //Variavel = arquivo fisico enviado, nomeArquivo nome do arquivo senco enviado, nomeAtual nome no banco gravado
        public static string UploadRename(HttpPostedFileBase variavel, string nomeArquivo, string nomeAtual)
        {
                //Remove arquivo no Host Para o delete deixar a variavel nula
                if (variavel == null && nomeArquivo == null && nomeAtual != "SemImagem.jpg")
                {
                    string fullPathh = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + nomeAtual);
                    if (System.IO.File.Exists(fullPathh))
                    {
                        System.IO.File.Delete(fullPathh);
                    }
                }

                //Edit sem trocar imagem
            if (variavel == null) return nomeAtual;
            if (variavel.ContentLength <= 0 && nomeAtual != "SemImagem.jpg")
            {
                return nomeAtual;
            }

            //Editar Salvar / Criar novo nome e Salvar imagem
            var arquivoRecebido = variavel;
            var ext = Path.GetExtension(arquivoRecebido.FileName).ToLower();

            if (ext != ".jpg" && ext != ".svg" && (ext != ".png" || arquivoRecebido.ContentLength <= 0)) return null;

            var caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var gerador = new string(
                Enumerable.Repeat(caracteres, 20)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            //Remover Arquivo no Host Para o edit preencher o nomeAtual
            string fullPath = System.Web.HttpContext.Current.Server.MapPath("~/Uploads/" + nomeAtual);
            if (System.IO.File.Exists(fullPath) && nomeAtual != "SemImagem.jpg")
            {
                System.IO.File.Delete(fullPath);
            }

            var RetornaNome = nomeArquivo + gerador + ext;
            var path = Path.Combine(HostingEnvironment.MapPath("~/Uploads"), RetornaNome);
            arquivoRecebido.SaveAs(path);
            return RetornaNome;
        }
    }
}