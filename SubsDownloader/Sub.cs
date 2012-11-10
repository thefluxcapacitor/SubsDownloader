namespace SubsDownloader
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using HtmlAgilityPack;

    public class Sub
    {
        private static readonly List<string> PreferredSubsTeams = new List<string>();

        public string Description { get; set; }

        public string DownloadUrl { get; set; }

        public int Cds { get; set; }

        public int Downloads { get; set; }

        public string SubsTeam { get; set; }

        public Sub(HtmlNode html)
        {
            if (Sub.PreferredSubsTeams.Count == 0)
            {
                Sub.PreferredSubsTeams.AddRange(ConfigurationManager.AppSettings["preferredSubsTeams"].Split(new char[] { ',' }));
                Sub.PreferredSubsTeams.Add("other");
            }

            this.Description = html.Descendants("div")
                .Where(div => div.GetAttributeValue("id", string.Empty).Equals("buscador_detalle_sub"))
                .First().InnerHtml;

            this.DownloadUrl = html.Descendants("a")
                .Where(a => a.GetAttributeValue("href", string.Empty)
                                .IndexOf("http://www.subdivx.com/bajar.php", StringComparison.OrdinalIgnoreCase) >= 0)
                .First().GetAttributeValue("href", string.Empty);

            this.Cds = int.Parse(this.ExtractBetween(html.InnerHtml, "<b>Cds:</b>", "<b>Comentarios:</b>"));
            this.Downloads = int.Parse(this.ExtractBetween(html.InnerHtml, "<b>Downloads:</b>", "<b>Cds:</b>"));
            
            this.SetSubsTeam();
        }

        private string ExtractBetween(string text, string leftDelimiter, string rightDelimiter)
        {
            var startOfLeftDelimiter = text.IndexOf(leftDelimiter);
            var endOfLeftDelimiter = startOfLeftDelimiter + leftDelimiter.Length;
            var startOfRightDelimiter = text.IndexOf(rightDelimiter);
            return text.Substring(endOfLeftDelimiter, startOfRightDelimiter - endOfLeftDelimiter);
        }

        private void SetSubsTeam()
        {
            foreach (var subsTeam in Sub.PreferredSubsTeams)
            {
                if (this.Description.IndexOf(subsTeam, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    this.SubsTeam = subsTeam;
                }
            }

            if (string.IsNullOrEmpty(this.SubsTeam))
            {
                this.SubsTeam = "other";
            }
        }

        public bool Matches(Video video)
        {
            var pos = this.Description.IndexOf(video.ReleaseGroup, StringComparison.OrdinalIgnoreCase);

            if (pos < 0)
            {
                return false;
            }

            if (pos > 0 && char.IsLetterOrDigit(this.Description[pos - 1]))
            {
                return false;
            }

            if (pos + video.ReleaseGroup.Length < this.Description.Length)
            {
                if (char.IsLetterOrDigit(this.Description[pos + video.ReleaseGroup.Length]))
                {
                    return false;
                }
            }

            return true;
        }

        public class SubComparer : IComparer<Sub>
        {
            public int Compare(Sub x, Sub y)
            {
                if (Sub.PreferredSubsTeams.IndexOf(x.SubsTeam) > Sub.PreferredSubsTeams.IndexOf(y.SubsTeam))
                {
                    return 1;
                }

                if (Sub.PreferredSubsTeams.IndexOf(x.SubsTeam) < Sub.PreferredSubsTeams.IndexOf(y.SubsTeam))
                {
                    return -1;
                }

                return 0;
            }
        }
    }
}