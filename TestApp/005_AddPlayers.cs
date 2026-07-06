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
    public partial class AddPlayersForm : BaseForm
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public AddPlayersForm(Form prev)
        {
            this.prev = prev;
            ds = Program.GetInfo();

            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string playerName = textBox1.Text;

            if (playerName == "")
                return;

            Spelers spelers = ds.GetSpelersClass();

            if (spelers.SpelerAlreadyExists(playerName))
                return;

            spelers.AddSpeler(playerName);

            textBox1.Text = "";
            textBox1.Focus();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }
    }
}
