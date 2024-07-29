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
    public partial class Form6 : Form
    {
        readonly Form prev;

        public Form6(Form previous)
        {
            InitializeComponent();
            prev = previous;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Form7 form = new(this);

            this.Hide();
            form.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Form8 form = new(this);

            this.Hide();
            form.Show();

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Form9 form = new(this);

            this.Hide();
            form.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            Form12 form = new(this);

            this.Hide();
            form.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
