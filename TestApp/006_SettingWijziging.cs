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
    public partial class Form9 : BaseForm
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public Form9(Form previous)
        {
            this.ds = Program.GetInfo();

            InitializeComponent();

            prev = previous;

            AddSettingsToCombobox();
        }

        private void AddSettingsToCombobox()
        {
            List<string> keys = this.ds.GetSettingsClass().GetKeys();

            foreach (string key in keys)
                comboBox1.Items.Add(key);

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || textBox1.Text == "")
                return;

            this.ds.GetSettingsClass().UpdateSetting(comboBox1.Text, textBox1.Text);

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
