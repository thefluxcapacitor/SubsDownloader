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
    using SubsDownloader;

    public partial class FrmShowSubs : Form
    {
        public FrmShowSubs()
        {
            InitializeComponent();
        }

        public static Sub SelectSub(IEnumerable<Sub> subs)
        {
            var frm = new FrmShowSubs();
            frm.dataGridView1.DataSource = subs;
            frm.dataGridView1.Columns[0].Width = 800;
            //frm.dataGridView1.Refresh();
            frm.ShowDialog();
            return frm.SelectedSub;
        }

        public Sub SelectedSub { get; set; }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.SelectedSub = (Sub)dataGridView1.Rows[e.RowIndex].DataBoundItem;
            this.Close();
        }
    }
}
