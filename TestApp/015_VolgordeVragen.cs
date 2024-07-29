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
    public partial class Form16 : Form
    {
        readonly Form prev;
        readonly List<Tests>? tests;
        readonly DataSetInfo? dsi;

        Tests? selectedTest;

        public Form16(Form previous)
        {
            this.dsi = Program.GetInfo();
            prev = previous;

            this.tests = dsi.GetTests();

            InitializeComponent();

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

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void ButtonUp_click(object sender, EventArgs e)
        {
            if (this.selectedTest == null)
                return;

            Button clicked = (Button)sender;
            
            string name = clicked.Name.Replace("Up~","");
            string prevQuestionId = Convert.ToString(Convert.ToInt32(name) - 1);

            if (prevQuestionId == "0")
                return;

            string questionValue = "";
            string questionReplaceValue = "";


            foreach (Label l in labels)
            {
                if(l.Name == "label~" + name)
                {
                    questionValue = l.Text;
                }

                if (l.Name == "label~" + prevQuestionId)
                {
                    questionReplaceValue = l.Text;
                }
            }

            if (questionValue == "" || questionReplaceValue == "")
                return;

            foreach (Label l in labels)
            {
                if (l.Name == "label~" + name)
                {
                    l.Text = questionReplaceValue;
                }
                if (l.Name == "label~" + prevQuestionId)
                {
                    l.Text = questionValue;
                }
            }
        }

        private void ButtonDown_click(object sender, EventArgs e)
        {

            if (this.selectedTest == null)
                return;

            Button clicked = (Button)sender;

            string name = clicked.Name.Replace("Down~", "");
            string prevQuestionId = Convert.ToString(Convert.ToInt32(name) + 1);

            List<Questions>? testQuestions = selectedTest.GetQuestions();

            if (testQuestions == null)
                return;

            if (prevQuestionId == Convert.ToString(testQuestions.Count + 1))
                return;

            string questionValue = "";
            string questionReplaceValue = "";

            foreach (Label l in labels)
            {
                if (l.Name == "label~" + name)
                {
                    questionValue = l.Text;
                }

                if (l.Name == "label~" + prevQuestionId)
                {
                    questionReplaceValue = l.Text;
                }
            }

            if (questionValue == "" || questionReplaceValue == "")
                return;

            foreach (Label l in labels)
            {
                if (l.Name == "label~" + name)
                {
                    l.Text = questionReplaceValue;
                }
                if (l.Name == "label~" + prevQuestionId)
                {
                    l.Text = questionValue;
                }
            }
        }

        private void ButtonRemove_click(object sender, EventArgs e)
        {
            if (selectedTest == null)
                return;

            Button clicked = (Button)sender;

            string name = clicked.Name.Replace("Remove~", "");
            string questionValue = "";

            foreach (Label l in labels)
            {
                if (l.Name == "label~" + name)
                {
                    questionValue = l.Text;
                    break;
                }
            }

            if (questionValue == "")
                return;

            List<Questions>? questions = selectedTest.GetQuestions();

            if (questions == null)
                return;

            Questions selectedQuestion = new();

            foreach(Questions q in questions)
            {
                if (q.GetQuestion() == questionValue)
                {
                    selectedQuestion = q;
                    break;
                }
            }

            if (selectedQuestion.GetQuestion() == null)
                return;

            selectedTest.RemoveQuestion(selectedQuestion);

            this.updateElements(selectedTest);
        }

        private void ChangeTest(object sender, EventArgs e)
        {
            if (tests == null)
                return;

            string testValue = comboBox1.Text;

            foreach (Tests t in this.tests)
            {
                if (t.GetName() == testValue)
                {
                    this.selectedTest = t;
                    break;
                }
            }

            if (this.selectedTest == null)
                return;

            this.updateElements(selectedTest);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (selectedTest == null)
                return;

            List<Questions>? questionsForTest = selectedTest.GetQuestions();

            if (questionsForTest == null)
                return;

            string questionLine = "";

            foreach(Label l in labels)
            {
                string questionValue = l.Text;

                foreach (Questions q in questionsForTest)
                {
                    if (q.GetQuestion() == questionValue)
                    {
                        if (questionLine.Length > 0)
                            questionLine += ',';

                        questionLine += Convert.ToString(q.GetId());
                        break;
                    }
                }
            }

            selectedTest.UpdateInFile(questionLine);
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
