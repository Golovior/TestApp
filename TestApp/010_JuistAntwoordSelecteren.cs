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
    public partial class JuistAntwoordSelecteren : Form
    {

        readonly Form prev;
        readonly DataSetClass ds;

        public JuistAntwoordSelecteren(Form previous)
        {
            prev = previous;
            InitializeComponent();

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
            prev.Show();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string opdracht = comboBox1.Text;
            string question = comboBox2.Text;

            foreach(CheckBox cb in checkboxes)
            {
                if (cb.Checked)
                {
                    string choice = cb.Name[8..];
                    string name = "";
                    foreach(Label lbl in labels)
                    {
                        if (lbl.Name == "label~" + choice)
                            name = lbl.Text;
                    }

                    this.ds.GetAntwoordenClass().SetAsCorrectAntwoord(opdracht, question, name);
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void SelectedOpdracht(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            updateElements();

            string selectedOpdracht = comboBox1.Text;

            if (selectedOpdracht == "")
                return;

            List<List<string>> allQuestions = ds.GetQuestionsClass().GetAllQuestions();

            foreach (List<string> question in allQuestions)
            {
                if (selectedOpdracht == question[0])
                {
                    comboBox2.Items.Add(question[1]);
                }
            }
        }

        private void ChangeQuestion(object sender, EventArgs e)
        {
            updateElements();

            string selectedOpdracht = comboBox1.Text;
            string selectedQuestion = comboBox2.Text;

            if (selectedOpdracht == "" || selectedQuestion == "")
                return;

            List<List<string>> allAnswers = ds.GetAntwoordenClass().GetAntwoorden();

            int order = 0;

            label1.Text = allAnswers.Count.ToString();

            foreach(List<string> answer in allAnswers)
            {
                if (answer[0] != selectedOpdracht || answer[1] != selectedQuestion)
                    continue;

                bool correct = false;

                if (answer[3] == "1")
                    correct = true;

                MakeAnswerRow(answer, order, correct);
                order++;
            }
        }
    }
}
