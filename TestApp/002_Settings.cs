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
    public partial class SettingsForm : BaseForm
    {
        readonly Form prev;

        public SettingsForm(Form previous)
        {
            InitializeComponent();
            prev = previous;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SpelAanmakenForm form = new(this);

            this.Hide();
            form.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Form9 form = new(this);

            this.Hide();
            form.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            OpdrachtAanmakenForm form = new(this);

            this.Hide();
            form.Show();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            AddPlayersForm form = new(this);

            this.Hide();
            form.Show();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            PlayerStatus form = new(this);

            this.Hide();
            form.Show();
        }

        private async void Button5_Click(object sender, EventArgs e)
        {
            DataSetClass ds = Program.GetInfo();

            Api api = ds.GetApiClass();
            await api.SaveData();
        }
    }
}
