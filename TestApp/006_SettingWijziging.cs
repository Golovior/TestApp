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
    public partial class Form9 : Form
    {
        readonly Form prev;
        readonly List<string>? settingsForCombobox;
        readonly List<string>? settings;
        readonly DataSetInfo dsi;

        public Form9(Form previous)
        {
            this.dsi = Program.GetInfo();

            this.settings = dsi.GetSettings();

            List<string> expectedSettings = new()
            {
                "gameid",
                "specialPlayer"
            };

            InitializeComponent();

            if (settings != null)
            {
                settingsForCombobox = new List<string>();
                foreach (string setting in settings)
                {
                    string? s = setting.Split('=')[0];
                    if (s == null)
                        continue;

                    settingsForCombobox.Add(s);
                }

                foreach(string s in expectedSettings)
                {
                    bool inSettings = false;

                    foreach (string setting in settingsForCombobox)
                    {
                        if (setting == s)
                        {
                            inSettings = true;
                            break;
                        }
                    }

                    if(!inSettings)
                    {
                        settingsForCombobox.Add(s);
                    }
                }

                comboBox1.Items.AddRange(settingsForCombobox.ToArray());
            }

            prev = previous;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || textBox1.Text == "")
                return;

            dsi.UpdateSetting(comboBox1.Text, textBox1.Text);

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
