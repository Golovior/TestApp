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
        readonly bool saveResults;

        public Form19(Form previous, bool saveResults)
        {
            prev = previous;
            ds = Program.GetInfo();

            InitializeComponent();

            FillComboboxWithTests();
            this.saveResults = saveResults;
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

            if (ds.GetTestsClass().GetGameForTest(test) == null)
            {
                MessageBox.Show("Deze test is nog niet aan een spel gekoppeld. Koppel eerst een spel via Test aanmaken.");
                return;
            }

            Form20 form = new(this, test, this.saveResults);

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
