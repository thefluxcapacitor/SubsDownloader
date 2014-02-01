namespace SubsDownloader
{
    using System;
    using System.Diagnostics;
    using System.Text.RegularExpressions;

    public class Video
    {
        public string Episode { get; set; }

        public string Season { get; set; }

        public bool? TvShow { get; set; }

        public string ReleaseGroup { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }

        public Video()
        {
        }

        public Video(string torrentName)
        {
            if (string.IsNullOrEmpty(torrentName) || string.IsNullOrEmpty(torrentName))
            {
                return;
            }

            torrentName = torrentName.Trim();

            if (string.IsNullOrEmpty(torrentName))
            {
                return;
            }

            if (torrentName.EndsWith("mp4", StringComparison.OrdinalIgnoreCase) || 
                torrentName.EndsWith("avi", StringComparison.OrdinalIgnoreCase))
            {
                torrentName = torrentName.Remove(torrentName.Length - 3).Trim();
            }

            int endingPosition;
            if (!this.ParseTvShow(torrentName, out endingPosition))
            {
                this.ParseYearAndTitle(torrentName, out endingPosition);
            }

            this.ParseReleaseGroup(torrentName, endingPosition);
        }

        private void ParseSeasonEpisode(string torrentName, out Match match)
        {
            var regex = new Regex(@"[s,S][0-9]{2}[e,E][0-9]{2}");
            match = regex.Match(torrentName);

            if (match.Success)
            {
                this.Season = torrentName.Substring(match.Index + 1, 2);
                this.Episode = torrentName.Substring(match.Index + 4, 2);
            }
            else
            {
                // This is the case of big bang theory and others (601 = season 6 episode 01)
                regex = new Regex(@"[\.,\-,\s][0-9]{3}($|[\.,\-,\s])");
                match = regex.Match(torrentName);

                if (match.Success)
                {
                    this.Season = string.Format("{0:D2}", int.Parse(torrentName.Substring(match.Index + 1, 1)));
                    this.Episode = torrentName.Substring(match.Index + 2, 2);
                }
                else
                {
                    // 6x9 or 6x10
                    regex = new Regex(@"[\.,\-,\s][0-9]{1}[x,X][0-9]{1,2}($|[\.,\-,\s])");
                    match = regex.Match(torrentName);

                    if (match.Success)
                    {
                        this.Season = string.Format("{0:D2}", int.Parse(torrentName.Substring(match.Index + 1, 1)));

                        int auxNumber;
                        var aux = torrentName.Substring(match.Index + 3, 2);
                        if (!int.TryParse(aux, out auxNumber))
                        {
                            auxNumber = int.Parse(torrentName.Substring(match.Index + 3, 1));
                        }

                        this.Episode = string.Format("{0:D2}", auxNumber);
                    }
                }
            }

            Debug.WriteLine("Season: {0}, Episode: {1}", this.Season, this.Episode); 
        }

        private bool ParseTvShow(string torrentName, out int endingPosition)
        {
            endingPosition = -1;

            if (!this.TvShow.HasValue)
            {
                this.TvShow = false;

                Match match;
                this.ParseSeasonEpisode(torrentName, out match);

                if (match.Success)
                {
                    var aux = torrentName.Substring(0, match.Index).Replace('.', ' ').Trim();
                    for (var i = aux.Length - 1; i >= 0; i--)
                    {
                        if (char.IsLetterOrDigit(aux[i]))
                        {
                            aux = aux.Substring(0, i + 1);
                            break;
                        }
                    }

                    this.Title = aux; 
                    this.TvShow = true;
                    
                    endingPosition = match.Index + match.Length;
                }

                if (this.TvShow.HasValue && this.TvShow.Value)
                {
                    Debug.WriteLine("Video evaluated as a TV show: {0}", this.Title);
                }
                else
                {
                    Debug.WriteLine("Video evaluated as a movie: {0}", this.Title);
                }
            }

            return this.TvShow.HasValue && this.TvShow.Value;
        }

        private void ParseYearAndTitle(string torrentName, out int endingPosition)
        {
            endingPosition = -1;

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
                            this.Title = torrentName.Substring(0, i - 5).Replace('.', ' ').Trim();
                        }

                        endingPosition = i;

                        break;
                    }
                }
            }

            if (this.Year == 0 && consecutiveDigits == 4)
            {
                this.Year = int.Parse(torrentName.Substring(torrentName.Length - 4, 4));

                if (string.IsNullOrEmpty(this.Title))
                {
                    this.Title = torrentName.Substring(0, torrentName.Length - 5).Replace('.', ' ');
                }

                endingPosition = torrentName.Length;
            }
            else if (this.Year == 0 && consecutiveDigits != 4)
            {
                var titleAssigned = false;
                for (var i = 0; i < torrentName.Length; i++)
                {
                    if (!char.IsLetterOrDigit(torrentName[i]) && torrentName[i] != ' ')
                    {
                        this.Title = torrentName.Substring(0, i).Trim();
                        endingPosition = i;
                        titleAssigned = true;
                        break;
                    }
                }

                if (!titleAssigned)
                {
                    this.Title = torrentName;
                    endingPosition = torrentName.Length;
                }
            }

            Debug.WriteLine("Title: {0}", this.Title);
            Debug.WriteLine("Year: {0}", this.Year);
        }

        private void ParseReleaseGroup(string torrentName, int startingPosition)
        {
            var releaseGroup = string.Empty;

            if (!string.IsNullOrEmpty(torrentName) && startingPosition > 0)
            {
                if (torrentName.Length > startingPosition)
                {
                    for (var j = torrentName.Length - 1; j >= startingPosition; j--)
                    {
                        if (!char.IsLetterOrDigit(torrentName[j]) && torrentName[j] != '[' && torrentName[j] != ']')
                        {
                            break;
                        }

                        if (torrentName[j] != '[' && torrentName[j] != ']')
                        {
                            releaseGroup = torrentName[j] + releaseGroup;
                        }
                    }
                }
            }

            this.ReleaseGroup = releaseGroup.Trim();

            Debug.WriteLine("ReleaseGroup: {0}", this.ReleaseGroup);
        }

        public string GetSearchString()
        {
            if (this.TvShow.HasValue && this.TvShow.Value)
            {
                return string.Format("{0} S{1}E{2}", this.Title, this.Season, this.Episode);
            }
            else
            {
                return string.Format("{0} {1}", this.Title, this.Year > 0 ? this.Year.ToString() : string.Empty);
            }
        }
    }
}