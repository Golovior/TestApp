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
    public partial class Form13 : Form
    {
        readonly Form prev;
        readonly DataSetInfo dsi;

        public Form13(Form previous)
        {
            InitializeComponent();
            prev = previous;
            dsi = Program.GetInfo();
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

            List<Tests>? testsMade = dsi.GetTests();

            int id = 0;
            if(testsMade != null)
                id = testsMade.Count;

            Tests test = new();
            test.SetName(textBox1.Text);
            test.SetId(id);

            test.WriteToFile();
            dsi.AddToTestList(test);

            textBox1.Text = "";
            textBox1.Focus();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
