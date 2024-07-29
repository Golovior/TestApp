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
    public partial class Form14 : Form
    {
        readonly Form prev;
        readonly List<Tests>? tests;
        readonly List<Questions>? questions;
        readonly DataSetInfo? dsi;

        Tests? selectedTest;
        List<Questions>? testQuestions;

        public Form14(Form previous)
        {
            this.dsi = Program.GetInfo();

            InitializeComponent();
            prev = previous;

            this.tests = dsi.GetTests();
            this.questions = dsi.GetQuestions();

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

            if (questions != null)
            {
                List<string> questionsForCombobox = new();
                foreach (Questions question in questions)
                {
                    string? q = question.GetQuestion();
                    if (q == null)
                        continue;

                    questionsForCombobox.Add(q);
                }

                comboBox2.Items.AddRange(questionsForCombobox.ToArray());
            }

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
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

            this.testQuestions = this.selectedTest.GetQuestions();

            if (this.testQuestions == null)
                return;

            int numberOfQuestions = this.testQuestions.Count;

            label4.Text = Convert.ToString(numberOfQuestions);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (this.selectedTest == null)
                return;

            if (this.questions == null)
                return;

            string questionSelectedValue = comboBox2.Text;

            if (questionSelectedValue == "")
                return;

            Questions questionSelected = new();
            
            foreach(Questions q in questions)
            {
                if (q.GetQuestion() == questionSelectedValue)
                {
                    questionSelected = q;
                    break;
                }
            }

            if (questionSelected.GetQuestion() == null)
                return;

            if (this.testQuestions != null)
            {
                foreach (Questions q in this.testQuestions)
                {
                    if (questionSelected.GetId() == q.GetId())
                        return;
                }
            }

            selectedTest.AddQuestion(questionSelected);

            selectedTest.UpdateInFile();
            comboBox2.Text = "";

            List<Questions>? testQuestionsSelected = this.selectedTest.GetQuestions();

            if (testQuestionsSelected == null)
                throw new Exception("Er zou een vraag toegevoegd moeten zijn!!");

            label4.Text = Convert.ToString(testQuestionsSelected.Count);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (this.selectedTest == null)
                return;

            int? id = selectedTest.GetId();

            if (id == null)
                return;

            Form15 form = new(this, Convert.ToInt32(id));

            this.Hide();
            form.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
