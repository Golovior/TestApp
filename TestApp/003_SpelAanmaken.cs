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
    public partial class SpelAanmakenForm : BaseForm
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public SpelAanmakenForm(Form previous)
        {
            this.ds = Program.GetInfo();
            InitializeComponent();
            prev = previous;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Games gameClass = ds.GetGamesClass();

            string gameName = textBox1.Text;

            if (gameClass.GameAlreadyExists(gameName))
                return;

            gameClass.AddGame(gameName);

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
            this.Dispose();
            prev.Show();
        }
    }
}
