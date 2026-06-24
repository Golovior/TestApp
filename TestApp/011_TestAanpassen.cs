using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form11 : Form
    {
        readonly Form prev;

        public Form11(Form previous)
        {
            prev = previous;
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Form13 form = new(this);

            this.Hide();
            form.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            Form14 form = new(this);

            this.Hide();
            form.Show();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            Form16 form = new(this);

            this.Hide();
            form.Show();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Form17 form = new(this);

            this.Hide();
            form.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Form19 form = new(this, false);

            this.Hide();
            form.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
