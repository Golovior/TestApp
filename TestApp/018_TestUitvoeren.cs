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
    public partial class Form19 : Form
    {
        readonly Form prev;
        readonly List<Tests>? tests;
        readonly DataSetInfo dsi;

        readonly bool isRealTest;

        public Form19(Form previous, bool realTest)
        {
            this.dsi = Program.GetInfo();
            prev = previous;
            isRealTest = realTest;

            this.tests = dsi.GetTests();

            InitializeComponent();

            if(!realTest)
            {
                button1.Text = "Test Controleren";
            }

            if (tests != null)
            {
                List<string> testsForCombobox = new();
                foreach (Tests test in tests)
                {
                    string? t = test.GetName();
                    if (t == null)
                        continue;

                    testsForCombobox.Add(t);
                }

                comboBox1.Items.AddRange(testsForCombobox.ToArray());
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string testName = comboBox1.Text;

            if (testName == "")
                return;

            if (tests == null)
                return;

            foreach(Tests t in tests)
            {
                if(t.GetName() == testName)
                {
                    Form20 testForm = new(this, isRealTest);
                    testForm.SetTest(t);

                    this.Hide();
                    testForm.Show();

                    testForm.repositionElements();

                    return;
                }
            }
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
