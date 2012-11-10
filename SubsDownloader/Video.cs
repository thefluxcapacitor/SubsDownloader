namespace SubsDownloader
{
    using System;
    using System.Configuration;
    using System.Text.RegularExpressions;

    public class Video
    {
        private string[] knownShows;

        public string Episode { get; set; }

        public string Season { get; set; }

        public bool? TvShow { get; set; }

        public string ReleaseGroup { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public string TorrentName { get; set; }

        public Video(string torrentName)
        {
            this.knownShows = ConfigurationManager.AppSettings["knownTvShows"].Split(new char[] { ',' });

            this.TorrentName = torrentName;

            if (this.ParseTvShow(torrentName))
            {
                this.ParseSeasonEpisode(torrentName);
            }

            this.ParseReleaseGroup(torrentName);

            this.ParseYear(torrentName);
        }

        private void ParseSeasonEpisode(string torrentName)
        {
            var regex = new Regex(@"[s,S][0-9]{2}[e,E][0-9]{2}");
            var result = regex.Match(torrentName);

            if (result.Success)
            {
                this.Season = torrentName.Substring(result.Index + 1, 2);
                this.Episode = torrentName.Substring(result.Index + 4, 2);
            }
            else
            {
                // This is the case of big bang theory and others (601 = season 6 episode 01)
                regex = new Regex(@"[\.,\s][0-9]{3}[\.,\s]");
                result = regex.Match(torrentName);

                if (result.Success)
                {
                    this.Season = string.Format("{0:D2}", int.Parse(torrentName.Substring(result.Index + 1, 1)));
                    this.Episode = torrentName.Substring(result.Index + 2, 2);
                }
            }
        }

        private bool ParseTvShow(string torrentName)
        {
            if (!this.TvShow.HasValue)
            {
                this.TvShow = false;

                foreach (var show in this.knownShows)
                {
                    if ((torrentName.IndexOf(show, StringComparison.OrdinalIgnoreCase) > -1)
                        || (torrentName.IndexOf(show.Replace(' ', '.'), StringComparison.OrdinalIgnoreCase) > -1))
                    {
                        this.Title = show.Replace('.', ' ');
                        this.TvShow = true;
                        break;
                    }
                }
            }

            return this.TvShow.Value;
        }

        private void ParseYear(string torrentName)
        {
            var consecutiveDigits = 0;
            var lastCharIsDigit = false;

            for (var i = 0; i < torrentName.Length; i++)
            {
                if (char.IsDigit(torrentName[i]))
                {
                    if (lastCharIsDigit)
                    {
                        consecutiveDigits++;
                    }
                    else
                    {
                        consecutiveDigits = 1;
                        lastCharIsDigit = true;
                    }
                }
                else
                {
                    lastCharIsDigit = false;

                    if (consecutiveDigits == 4)
                    {
                        this.Year = int.Parse(torrentName.Substring(i - 4, 4));

                        if (string.IsNullOrEmpty(this.Title))
                        {
                            this.Title = torrentName.Substring(0, i - 5).Replace('.', ' ');
                        }

                        consecutiveDigits = 0;
                    }
                }
            }
        }

        private void ParseReleaseGroup(string torrentName)
        {
            var releaseGroup = string.Empty;

            for (var j = torrentName.Length - 1; j >= 0; j--)
            {
                if (torrentName[j] == '-')
                {
                    break;
                }

                releaseGroup = torrentName[j] + releaseGroup;
            }

            this.ReleaseGroup = releaseGroup;
        }

        public string GetSearchString()
        {
            if (this.TvShow.Value)
            {
                return string.Format("{0} S{1}E{2}", this.Title, this.Season, this.Episode);
            }
            else
            {
                return string.Format("{0} {1}", this.Title, this.Year);
            }
        }
    }
}