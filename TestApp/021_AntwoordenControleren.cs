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
    public partial class Form22 : Form
    {
        readonly Form prev;
        readonly DataSetClass ds;
        readonly TestAntwoorden testAntwoorden;

        public Form22(Form previous)
        {
            this.prev = previous;
            ds = Program.GetInfo();
            
            testAntwoorden = ds.GetTestAntwoordenClass();

            InitializeComponent();

            FillComboboxWithTests();

        }

        public void FillComboboxWithTests()
        {
            List<string> tests = ds.GetTestsClass().GetAllTests();

            foreach (string test in tests)
            {
                comboBox1.Items.Add(test);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void ChangeTest(object sender, EventArgs e)
        {
            string testName = comboBox1.Text;

            comboBox2.Items.Clear();
            comboBox2.Text = "";

            List<string> spelers = new();

            foreach(List<string> a in testAntwoorden.GetAllTestAntwoorden())
            {
                if (a[0] != testName)
                    continue;

                if(!spelers.Contains(a[1]))
                    spelers.Add(a[1]);
            }

            spelers.Sort();

            foreach(string speler in spelers)
            {
                comboBox2.Items.Add(speler);
            }
        }

        private void ChangePlayer(object sender, EventArgs e)
        {
            string testName = comboBox1.Text;
            string name = comboBox2.Text;

            if (name == null)
                return;

            List<List<string>> antwoordenSet = new();

            foreach (List<string> a in testAntwoorden.GetAllTestAntwoorden())
            {
                if (a[0] != testName)
                    continue;

                if (a[1] != name)
                    continue;

                antwoordenSet.Add(a);
            }

            showAntwoordenFromPlayer(antwoordenSet);
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            prev.Show();
        }
    }
}
