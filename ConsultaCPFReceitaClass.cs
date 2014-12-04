using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace RetaguardaClassLibrary
{
    public class ConsultaCPFReceitaClass
    {
        private string _erro;
        private string viewState;
        private CookieContainer cookieContainer = new CookieContainer();
        private String UrlDominio = "http://www.receita.fazenda.gov.br";
        private String UrlBase = "http://www.receita.fazenda.gov.br/aplicacoes/atcta/cpf/ConsultaPublica.asp";
        private string UrlCaptcha;
        public String ErroDetectado { get { return _erro; } }

        /// <summary>
        /// Recupera a imagem de verificação e retorna um Bitmap para ser exibido na tela.
        /// </summary>
        /// <returns></returns>
        public Image RecuperaCaptcha()
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://www.receita.fazenda.gov.br/aplicacoes/atcta/cpf/captcha/gerarCaptcha.asp");
                httpWebRequest.CookieContainer = cookieContainer;
                using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream = httpWebReponse.GetResponseStream())
                    {
                        return Image.FromStream(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                _erro = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Consulta o CPF na Receita Federal e retorna um dicionário com os campos Nome e Situacao.
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="captcha"></param>
        /// <returns></returns>
        public Dictionary<String, String> Consulta(string cpf, string captcha)
        {
            try
            {
                HttpWebRequest req1 = (HttpWebRequest)WebRequest.Create("http://www.receita.fazenda.gov.br/aplicacoes/atcta/cpf/ConsultaPublica.asp");
                req1.CookieContainer = cookieContainer;
                req1.GetResponse();

                Dictionary<String, String> resultado = new Dictionary<string, string>();
                var parametros = "txtCPF=" + System.Uri.EscapeDataString(cpf) + "&" +
                                 "txtTexto_captcha_serpro_gov_br=" + System.Uri.EscapeDataString(captcha) + "&" +
                                 //"txtToken_captcha_serpro_gov_br=" + System.Uri.EscapeDataString(viewState) + "&" +
                                 "Enviar=Consultar";
                                 
                
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.receita.fazenda.gov.br/aplicacoes/atcta/cpf/ConsultaPublicaExibir.asp");                
                
                req.CookieContainer = cookieContainer;

                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                req.Timeout = 20000;
                req.AllowAutoRedirect = true;
                req.ContentLength = parametros.Length;
                StreamWriter stParametros = new StreamWriter(req.GetRequestStream(), Encoding.GetEncoding("ISO-8859-1"));
                stParametros.Write(parametros);
                stParametros.Close();
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader stHtml = new StreamReader(req.GetResponse().GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"));

                    string retorno = stHtml.ReadToEnd();

                    if (retorno.Contains("<div id=\"idMensagemErro\">"))
                    {
                        _erro = "CPF não está cadastrado na Receita Federal ou o Captcha está incorreto!";
                        return null;
                    }

                    string nome = Regex.Match(retorno, "<span class=\"clConteudoDados\">Nome da Pessoa Física: (.*)</span>").Groups[1].Value.Trim();
                    string situacao = Regex.Match(retorno, "<span class=\"clConteudoDados\">Situação Cadastral: (.*)</span>").Groups[1].Value;

                    resultado.Add("Nome", nome);
                    resultado.Add("Situacao", situacao);

                    stHtml.Close();
                }
                response.Close();

                return resultado;
            }
            catch (Exception ex)
            {
                _erro = ex.Message;
                return null;
            }
        }
    }
}


