using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SubsDownloader
{
    public partial class FrmVideoInfo : Form
    {
        private readonly Video video;

        public FrmVideoInfo(Video video)
        {
            this.video = video;

            InitializeComponent();
        }

        private void SetDefaultValues()
        {
            this.textTitle.Text = this.video.Title;
            this.textYear.Text = this.video.Year.ToString();
            this.textReleaseGroup.Text = this.video.ReleaseGroup;
            if (this.video.TvShow.HasValue && this.video.TvShow.Value)
            {
                this.checkTVShow.Checked = true;
                this.textSeason.Text = this.video.Season;
                this.textEpisode.Text = this.video.Episode;
            }
            else
            {
                this.checkTVShow.Checked = false;
                this.textSeason.Text = string.Empty;
                this.textEpisode.Text = string.Empty;
            }
        }

        public static Video GetVideoInfo(Video video)
        {
            var frm = new FrmVideoInfo(video);
            frm.SetDefaultValues();

            if (frm.ShowDialog() == DialogResult.OK)
            {
                return frm.GetVideo();
            }

            return null;
        }

        private Video GetVideo()
        {
            return this.video;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void FrmVideoInfo_Load(object sender, EventArgs e)
        {

        }

        private void checkTVShow_CheckedChanged(object sender, EventArgs e)
        {
            textEpisode.Enabled = checkTVShow.Checked;
            textSeason.Enabled = checkTVShow.Checked;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.video.Title = textTitle.Text;
            this.video.Year = string.IsNullOrEmpty(textYear.Text) ? 0 : int.Parse(textYear.Text);
            this.video.ReleaseGroup = textReleaseGroup.Text;
            if (checkTVShow.Checked)
            {
                this.video.TvShow = true;
                this.video.Season = textSeason.Text;
                this.video.Episode = textEpisode.Text;
            }
            else
            {
                this.video.TvShow = false;
                this.video.Season = string.Empty;
                this.video.Episode = string.Empty;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
