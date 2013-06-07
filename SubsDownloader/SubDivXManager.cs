namespace SubsDownloader
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;

    using HtmlAgilityPack;

    public class SubDivXManager
    {
        private CookieContainer loginCookies = new CookieContainer();

        public IList<Sub> GetCandidateSubs(Video video, bool searchInComments)
        {
            Console.WriteLine();
            Console.WriteLine("Text used to seach subtitle is: {0}", video.GetSearchString());
            Console.WriteLine("Release Group to match: {0}", video.ReleaseGroup);

            this.login();

            var candidateSubs = new List<Sub>();
            var subdivxClient = new CookieAwareWebClient(this.loginCookies);
            subdivxClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; MSIE 9.0; WIndows NT 9.0; en-US))";

            var page = 1;

            while (page > 0)
            {
                var url = this.GetUrl(video.GetSearchString(), page);

                var output = subdivxClient.DownloadString(url);
                
                var doc = new HtmlDocument();
                doc.LoadHtml(output);
                var subsHtmlNodes = doc.DocumentNode.Descendants("div").Where(div => div.GetAttributeValue("id", string.Empty).Equals("buscador_detalle"));

                if (subsHtmlNodes.Count() == 0)
                {
                    page = -1;
                }
                else
                {
                    foreach (var subHtmlNode in subsHtmlNodes)
                    {
                        var subComments = string.Empty;
                        if (searchInComments)
                        {
                            subComments = this.GetSubComments(subdivxClient, subHtmlNode);
                        }

                        var sub = new Sub(subHtmlNode, subComments);

                        if (sub.Matches(video))
                        {
                            candidateSubs.Add(sub);
                        }
                    }
                }

                page++;
            }

            candidateSubs.Sort(new Sub.SubComparer());

            return candidateSubs;
        }

        private void login()
        {
            var encoding = new ASCIIEncoding();

            var values = new NameValueCollection();
            values.Add("usuario", ConfigurationManager.AppSettings["subdivxUserName"]);
            values.Add("clave", ConfigurationManager.AppSettings["subdivxPassword"]);
            values.Add("Enviar", "Entrar");
            values.Add("accion", "14");
            values.Add("enviau", "1");

            var array = (from key in values.AllKeys
                         from value in values.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();

            byte[] data = encoding.GetBytes(string.Join("&", array));

            var myRequest = (HttpWebRequest)WebRequest.Create("http://www.subdivx.com/index.php");
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded";
            myRequest.ContentLength = data.Length;
            var newStream = myRequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            var response = myRequest.GetResponse();

            var cookies = response.Headers["Set-Cookie"];

            this.loginCookies.SetCookies(new Uri("http://www.subdivx.com/index.php"), cookies);
        }

        private string GetSubComments(WebClient webClient, HtmlNode subHtmlNode)
        {
            var aux = subHtmlNode.InnerHtml;
            var i = aux.IndexOf("href=\"popcoment.php?idsub=", System.StringComparison.OrdinalIgnoreCase);
            var j = aux.IndexOf("\"", i + "href=\"".Length, System.StringComparison.OrdinalIgnoreCase);

            if (i > -1 && i < j)
            {
                var start = i + "href=\"".Length;
                var url = "http://subdivx.com/" + aux.Substring(start, j - start);
                var comments = webClient.DownloadString(url);
                return comments;
            }

            return string.Empty;
        }

        private string GetUrl(string searchString, int page)
        {
            return string.Format(
                "http://www.subdivx.com/index.php?buscar={0}&accion=5&masdesc=&subtitulos=1&realiza_b=1&pg={1}",
                HttpUtility.UrlEncode(searchString), 
                page);
        }

        public void DownloadSub(string path, string downloadUrl)
        {
            var subdivxClient = new CookieAwareWebClient(this.loginCookies);
            subdivxClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; MSIE 9.0; WIndows NT 9.0; en-US))";

            var directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            subdivxClient.DownloadFile(downloadUrl, path);
        }
    }
}