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
    public partial class Form20 : Form
    {
        readonly Form prev;
        readonly DataSetClass ds;
        readonly List<List<string>> questions = new();
        readonly bool saveResult;
        readonly string testName;
        string speler = "";
        int currentQuestion = 0;

        public Form20(Form previous, string testName, bool saveResult = false)
        {
            this.prev = previous;

            this.saveResult = saveResult;

            ds = Program.GetInfo();
            this.testName = testName;

            questions.Clear();

            GetAllQuestionsForTest(testName);

            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            this.repositionElements();
        }

        private void GetAllQuestionsForTest(string testName)
        {
            List<List<string>> testVragen = ds.GetTestVragenClass().GetAllTestVragen();

            foreach (List<string> tv in testVragen)
            {
                if (tv.Count < 4)
                    continue;

                if (tv[0] == testName)
                    questions.Add(tv);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;

            if (name == "Exit")
            {
                this.Dispose();
                this.prev.Show();
                return;
            }

            Spelers spelersClass = ds.GetSpelersClass();
            bool foundSpeler = false;

            foreach (List<string> s in spelersClass.GetSpelers())
            {
                if (s[0].ToLower() == name.ToLower())
                    foundSpeler = true;
            }

            bool madeTest = false;

            TestAntwoorden ta = ds.GetTestAntwoordenClass();

            foreach (List<string> a in ta.GetAllTestAntwoorden())
            {
                if (a[0] != testName)
                    continue;

                if (a[1].ToLower() == name.ToLower())
                {
                    madeTest = true;
                    break;
                }
            }

            if ((!foundSpeler || madeTest) && saveResult && !name.StartsWith("speler"))
                return;

            this.speler = name;
            this.currentQuestion = 0;
            label6.Text = DateTime.Now.ToString();

            this.ShowQuestionLayout();
            this.Button2_Click(sender, e);

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (this.saveResult && currentQuestion > 0)
            {
                bool succesvol = SaveAnswer();

                if (!succesvol)
                    return;
            }

            NextQuestion();
        }

        private bool SaveAnswer()
        {
            string currentQuestion = label3.Text;
            TestAntwoorden testAntwoorden = ds.GetTestAntwoordenClass();
            string antwoordKeuze = "";
            string geselecteerdAntwoord = "";

            foreach (RadioButton rb in antwoordRBs)
            {
                if (rb.Checked)
                {
                    antwoordKeuze = rb.Name[6..];
                }
            }

            if (antwoordKeuze == "")
                return false;

            foreach (Label al in antwoordLabels)
            {
                if (al.Name == "label~" + antwoordKeuze)
                    geselecteerdAntwoord = al.Text;
            }

            if (geselecteerdAntwoord == "")
                return false;

            foreach (List<string> q in questions)
            {
                if (q[2] != currentQuestion)
                    continue;

                if (testAntwoorden.AntwoordAlreadyExists(q[0], speler, q[1], q[2], geselecteerdAntwoord))
                    return false;

                testAntwoorden.AddTestAntwoord(q[0], speler, q[1], q[2], geselecteerdAntwoord);
            }

            return true;
        }

        private void NextQuestion()
        {
            currentQuestion++;
            List<string>? volgendeVraag = questions
                .OrderBy(question => int.TryParse(question[3], out int order) ? order : int.MaxValue)
                .ThenBy(question => question[2])
                .ElementAtOrDefault(currentQuestion - 1);

            if (volgendeVraag == null)
            {
                EindeTest();
                return;
            }

            List<List<string>> allPlayers = ds.GetSpelersClass().GetSpelers();
            List<string> activePlayers = new();

            foreach (List<string> speler in allPlayers)
            {
                if (speler[1] == "1")
                    activePlayers.Add(speler[0]);
            }

            List<List<string>> antwoordMogelijkheden = new();

            foreach (List<string> a in ds.GetAntwoordenClass().GetAntwoorden())
            {
                if (a[0] != volgendeVraag[1])
                    continue;

                if (a[1] != volgendeVraag[2])
                    continue;

                if (a[4].Length > 4)
                {
                    List<string> pqo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(a[4]) ?? new();

                    bool answerNeeded = false;

                    foreach (string p in pqo)
                    {
                        if (activePlayers.Contains(p))
                            answerNeeded = true;
                    }

                    if (!answerNeeded)
                        continue;
                }

                antwoordMogelijkheden.Add(a);
            }

            if (antwoordMogelijkheden.Count == 0)
                return;

            Questions vragen = ds.GetQuestionsClass();
            string alfabetisch = "1";

            foreach (List<string> v in vragen.GetAllQuestions())
            {
                if (v[0] != volgendeVraag[1])
                    continue;

                if (v[1] != volgendeVraag[2])
                    continue;

                alfabetisch = v[2];
            }

            ShowNextQuestion(currentQuestion, volgendeVraag[2], antwoordMogelijkheden, alfabetisch);
        }

        private void EindeTest()
        {
            TestAntwoorden testAntwoorden = ds.GetTestAntwoordenClass();

            DateTime starttime = DateTime.Parse(label6.Text);
            DateTime endTime = DateTime.Now;

            TimeSpan span = endTime - starttime;

            string ms = Convert.ToString(span.TotalSeconds);

            testAntwoorden.AddTestAntwoord(testName, speler, "einde Test", "Tijd gespendeerd", ms);

            ShowStartup();
        }

        public void ShowStartup()
        {
            panel1.Controls.Clear();

            panel1.Visible = false;

            label5.Visible = false;

            label2.Visible = false;
            label3.Visible = false;

            button2.Visible = false;

            button1.Visible = true;
            label1.Visible = true;
            label4.Visible = true;
            textBox1.Visible = true;

            textBox1.Text = "";
            textBox1.Focus();
        }

        public void ShowQuestionLayout()
        {
            panel1.Visible = true;

            if (this.saveResult)
                label5.Visible = true;

            label2.Visible = true;
            label3.Visible = true;

            button2.Visible = true;

            button1.Visible = false;
            label1.Visible = false;
            label4.Visible = false;
            textBox1.Visible = false;
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
                this.Button1_Click(sender, e);
        }
    }
}
