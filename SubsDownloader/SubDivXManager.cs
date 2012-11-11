namespace SubsDownloader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;

    using HtmlAgilityPack;

    public class SubDivXManager
    {
        public IList<Sub> GetCandidateSubs(Video video)
        {
            Console.WriteLine("Text used to seach subtitle is: {0}", video.GetSearchString());

            var subdivxClient = new WebClient();
            subdivxClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; MSIE 9.0; WIndows NT 9.0; en-US))";
            var output = subdivxClient.DownloadString(
                string.Format(
                    "http://www.subdivx.com/index.php?buscar={0}&accion=5&masdesc=&subtitulos=1&realiza_b=1",
                    HttpUtility.UrlEncode(video.GetSearchString())));

            var doc = new HtmlDocument();
            doc.LoadHtml(output);
            var subsHtmlNodes = doc.DocumentNode.Descendants("div").Where(div => div.GetAttributeValue("id", string.Empty).Equals("buscador_detalle"));

            var candidateSubs = new List<Sub>();

            foreach (var subHtmlNode in subsHtmlNodes)
            {
                var sub = new Sub(subHtmlNode);

                if (sub.Matches(video))
                {
                    candidateSubs.Add(sub);
                }
            }

            candidateSubs.Sort(new Sub.SubComparer());

            return candidateSubs;
        }

        public void DownloadSub(string path, string downloadUrl)
        {
            var subdivxClient = new WebClient();
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