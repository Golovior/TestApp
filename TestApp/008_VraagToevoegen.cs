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
    public partial class Form3 : Form
    {
        readonly Form previous;
        readonly DataSetClass ds;

        public Form3(Form prev)
        {
            InitializeComponent();
            previous = prev;

            ds = Program.GetInfo();

            AddOpdrachtenToCombobox();
        }

        private void AddOpdrachtenToCombobox()
        {
            List<string> opdrachten = this.ds.GetOpdrachtenClass().GetOpdrachten();

            foreach (string opdracht in opdrachten)
                comboBox1.Items.Add(opdracht);

        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            previous.Show();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "" || textBox1.Text == "")
                return;

            string opdracht = comboBox1.Text;
            string question = textBox1.Text;

            string alfabetisch = "0";

            if (checkBox1.Checked)
                alfabetisch = "1";

            Questions questions = this.ds.GetQuestionsClass();

            if (questions.QuestionAlreadyExists(opdracht, question, alfabetisch))
                return;

            questions.AddQuestion(opdracht, question, alfabetisch);

            textBox1.Text = "";
            checkBox1.Checked = false;
            textBox1.Focus();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            previous.Show();
        }
    }
}
