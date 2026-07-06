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
    public partial class Form13 : BaseForm
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public Form13(Form previous)
        {
            InitializeComponent();
            prev = previous;

            ds = Program.GetInfo();

            AddGamesToComboboxes();
            AddTestsToCombobox();
        }

        private void AddGamesToComboboxes()
        {
            foreach (string game in ds.GetGamesClass().GetAllGames())
            {
                comboBox1.Items.Add(game);
                comboBox3.Items.Add(game);
            }
        }

        private void AddTestsToCombobox()
        {
            foreach (string test in ds.GetTestsClass().GetAllTests())
                comboBox2.Items.Add(test);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                return;

            if (comboBox1.Text == "")
                return;

            string test = textBox1.Text;

            Tests tests = ds.GetTestsClass();

            if (tests.TestAlreadyExists(test))
                return;

            tests.AddTest(test);
            tests.SetGameForTest(test, comboBox1.Text);

            textBox1.Text = "";
            textBox1.Focus();

            comboBox2.Items.Add(test);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            string test = comboBox2.Text;
            string game = comboBox3.Text;

            if (test == "" || game == "")
                return;

            ds.GetTestsClass().SetGameForTest(test, game);
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
