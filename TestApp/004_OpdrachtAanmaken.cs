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
    public partial class OpdrachtAanmakenForm : Form
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public OpdrachtAanmakenForm(Form previous)
        {
            InitializeComponent();
            ds = Program.ds;
            prev = previous;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Opdrachten opdrachtenClass = ds.GetOpdrachtenClass();

            string opdrachtenName = textBox1.Text;

            if (opdrachtenClass.OpdrachtAlreadyExists(opdrachtenName))
                return;

            opdrachtenClass.AddOpdracht(opdrachtenName);

            textBox1.Text = "";
            textBox1.Focus();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
