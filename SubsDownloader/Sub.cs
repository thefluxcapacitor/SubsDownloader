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

        public IList<string> Comments { get; set; }

        public string DownloadUrl { get; set; }

        public int Cds { get; set; }

        public int Downloads { get; set; }

        public string SubsTeam { get; set; }

        public string Title { get; set; }

        public string SubUrl { get; set; }

        public Guid Id { get; set; }

        public Sub()
        {
            this.Id = Guid.NewGuid();
        }

        public Sub(HtmlNode html, string subComments) : this()
        {
            if (Sub.PreferredSubsTeams.Count == 0)
            {
                Sub.PreferredSubsTeams.AddRange(ConfigurationManager.AppSettings["preferredSubsTeams"].Split(new char[] { ',' }));
                Sub.PreferredSubsTeams.Add("other");
            }

            if (html.PreviousSibling.GetAttributeValue("id", string.Empty).Equals("menu_detalle_buscador"))
            {
                var aux = html.PreviousSibling.Descendants("a").FirstOrDefault(child => child.GetAttributeValue("class", string.Empty).Equals("titulo_menu_izq"));
                if (aux != null)
                {
                    this.Title = aux.InnerText;
                    this.SubUrl = aux.GetAttributeValue("href", string.Empty);
                }
            }

            this.Description = html.Descendants("div")
                .First(div => div.GetAttributeValue("id", string.Empty).Equals("buscador_detalle_sub"))
                .InnerHtml;

            this.DownloadUrl = html.Descendants("a")
                .First(a => a.GetAttributeValue("href", string.Empty).IndexOf("http://www.subdivx.com/bajar.php", StringComparison.OrdinalIgnoreCase) >= 0)
                .GetAttributeValue("href", string.Empty);

            this.Cds = int.Parse(this.ExtractBetween(html.InnerHtml, "<b>Cds:</b>", "<b>Comentarios:</b>"));
            this.Downloads = int.Parse(this.ExtractBetween(html.InnerHtml, "<b>Downloads:</b>", "<b>Cds:</b>").Replace(",", string.Empty));

            this.Comments = ParseComments(subComments);

            this.SetSubsTeam();
        }

        public static IList<string> ParseComments(string comments)
        {

            if (string.IsNullOrEmpty(comments))
            {
                return new List<string>();
            }

            const string StartOfCommentDelimiter = "<div id=\"pop_upcoment\">";
            const string EndOfCommentDelimiter = "<div id=\"pop_upcoment_der\">";

            var splitted = comments.Split(new string[] { StartOfCommentDelimiter }, StringSplitOptions.RemoveEmptyEntries).ToList();
            splitted.RemoveAll(item => item.IndexOf(EndOfCommentDelimiter, StringComparison.OrdinalIgnoreCase) <= 0);

            return splitted.Select(item => item.Substring(0,
                    item.IndexOf(EndOfCommentDelimiter, StringComparison.OrdinalIgnoreCase)))
                .ToList();
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
            return this.Matches(video, this.Description) || this.Matches(video, this.GetCommentsCsv());
        }

        private string GetCommentsCsv()
        {
            if (this.Comments.Any())
            {
                return this.Comments.Aggregate((c, n) => c + "," + n);
            }

            return string.Empty;
        }

        private bool Matches(Video video, string content)
        {
            if (string.IsNullOrEmpty(video.ReleaseGroup))
            {
                return true;
            }

            var pos = content.IndexOf(video.ReleaseGroup, StringComparison.OrdinalIgnoreCase);

            if (pos < 0)
            {
                return false;
            }

            if (pos > 0 && char.IsLetterOrDigit(content[pos - 1]))
            {
                return false;
            }

            if (pos + video.ReleaseGroup.Length < content.Length)
            {
                if (char.IsLetterOrDigit(content[pos + video.ReleaseGroup.Length]))
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