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
    public partial class Form15 : Form
    {
        readonly Form prev;

        readonly Tests test;
        readonly DataSetInfo dsi;

        public Form15(Form previous, int testId)
        {
            dsi = Program.GetInfo();

            prev = previous;
            InitializeComponent();

            test = dsi.GetTestById(testId);

            List<Questions>? questions = test.GetQuestions();
            if (questions == null)
                return;

            foreach (Questions q in questions)
            {
                string? questionString = q.GetQuestion();
                if (questionString == null)
                    continue;

                listBox1.Items.Add(questionString);
            }
            
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
            prev.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
