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
            Console.WriteLine();
            Console.WriteLine("Text used to seach subtitle is: {0}", video.GetSearchString());
            Console.WriteLine("Release Group to match: {0}", video.ReleaseGroup);

            var candidateSubs = new List<Sub>();
            var subdivxClient = new WebClient();
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
                        var sub = new Sub(subHtmlNode);

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

        private string GetUrl(string searchString, int page)
        {
            return string.Format(
                "http://www.subdivx.com/index.php?buscar={0}&accion=5&masdesc=&subtitulos=1&realiza_b=1&pg={1}",
                HttpUtility.UrlEncode(searchString), 
                page);
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