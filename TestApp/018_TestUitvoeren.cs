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
        readonly DataSetClass ds;

        public Form19(Form previous)
        {
            prev = previous;
            ds = Program.GetInfo();

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

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
                return;

            string test = comboBox1.Text;

            Form20 form = new(this, test, true);

            this.Hide();
            form.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            prev.Show();
        }
    }
}
