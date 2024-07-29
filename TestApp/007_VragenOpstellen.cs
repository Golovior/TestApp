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
    public partial class Form2 : Form
    {
        readonly Form previous;

        public Form2(Form prev)
        {
            InitializeComponent();
            previous = prev;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Form3 f3 = new(this);

            this.Hide();
            f3.ShowDialog();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
            previous.Show();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Form4 f4 = new(this);

            this.Hide();
            f4.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Form10 f10 = new(this);

            this.Hide();
            f10.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
