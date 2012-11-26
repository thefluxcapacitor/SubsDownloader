namespace SubsDownloader
{
    partial class FrmVideoInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textYear = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textReleaseGroup = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textSeason = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textEpisode = new System.Windows.Forms.TextBox();
            this.checkTVShow = new System.Windows.Forms.CheckBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textTitle
            // 
            this.textTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textTitle.Location = new System.Drawing.Point(104, 13);
            this.textTitle.Name = "textTitle";
            this.textTitle.Size = new System.Drawing.Size(252, 20);
            this.textTitle.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Title";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Release Group";
            // 
            // textYear
            // 
            this.textYear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textYear.Location = new System.Drawing.Point(104, 39);
            this.textYear.Name = "textYear";
            this.textYear.Size = new System.Drawing.Size(252, 20);
            this.textYear.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Year";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // textReleaseGroup
            // 
            this.textReleaseGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textReleaseGroup.Location = new System.Drawing.Point(104, 65);
            this.textReleaseGroup.Name = "textReleaseGroup";
            this.textReleaseGroup.Size = new System.Drawing.Size(252, 20);
            this.textReleaseGroup.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Season";
            // 
            // textSeason
            // 
            this.textSeason.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textSeason.Location = new System.Drawing.Point(104, 122);
            this.textSeason.Name = "textSeason";
            this.textSeason.Size = new System.Drawing.Size(252, 20);
            this.textSeason.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Episode";
            // 
            // textEpisode
            // 
            this.textEpisode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textEpisode.Location = new System.Drawing.Point(104, 148);
            this.textEpisode.Name = "textEpisode";
            this.textEpisode.Size = new System.Drawing.Size(252, 20);
            this.textEpisode.TabIndex = 8;
            // 
            // checkTVShow
            // 
            this.checkTVShow.AutoSize = true;
            this.checkTVShow.Checked = true;
            this.checkTVShow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkTVShow.Location = new System.Drawing.Point(12, 99);
            this.checkTVShow.Name = "checkTVShow";
            this.checkTVShow.Size = new System.Drawing.Size(68, 17);
            this.checkTVShow.TabIndex = 10;
            this.checkTVShow.Text = "TV show";
            this.checkTVShow.UseVisualStyleBackColor = true;
            this.checkTVShow.CheckedChanged += new System.EventHandler(this.checkTVShow_CheckedChanged);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(182, 186);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(84, 24);
            this.buttonOk.TabIndex = 11;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(272, 186);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(84, 24);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FrmVideoInfo
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(368, 222);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.checkTVShow);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textEpisode);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textSeason);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textReleaseGroup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textYear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textTitle);
            this.Name = "FrmVideoInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmVideoInfo";
            this.Load += new System.EventHandler(this.FrmVideoInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textYear;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textReleaseGroup;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textSeason;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textEpisode;
        private System.Windows.Forms.CheckBox checkTVShow;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
    }
}