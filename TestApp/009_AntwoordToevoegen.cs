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
    public partial class Form4 : Form
    {
        readonly Form previous;
        readonly DataSetClass ds;

        public Form4(Form prev)
        {
            InitializeComponent();

            previous = prev;
            ds = Program.GetInfo();

            AddOpdrachtenToCombobox();
        }

        public void AddOpdrachtenToCombobox()
        {
            List<string> opdrachten = ds.GetOpdrachtenClass().GetOpdrachten();

            foreach (string opdracht in opdrachten)
            {
                comboBox1.Items.Add(opdracht);
            }

        }

        public void AddQuestionsToCombobox()
        {
            comboBox2.Items.Clear();

            List<List<string>> questions = ds.GetQuestionsClass().GetAllQuestions();

            string opdracht = comboBox1.Text;

            foreach (List<string> question in questions)
            {
                if (question[0] == opdracht)
                    comboBox2.Items.Add(question[1]);
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string question = comboBox2.Text;
            string opdracht = comboBox1.Text;
            string antwoord = textBox1.Text;

            if (antwoord == "")
                return;

            if (opdracht == "")
                return;

            if (question == "")
                return;

            Antwoorden antwoordClass = ds.GetAntwoordenClass();

            if (antwoordClass.AntwoordAlreadyExists(opdracht, question, antwoord))
                return;

            antwoordClass.AddAntwoord(opdracht, question, antwoord);
            
            textBox1.Text = "";
            textBox1.Focus();

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            previous.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddQuestionsToCombobox();
        }
    }
}
