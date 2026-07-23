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
        readonly List<List<string>> displayableQuestions = new();
        readonly bool saveResult;
        readonly string testName;
        readonly string gameName;
        string speler = "";
        int currentQuestion = 0;
        Guid? currentAfnameId;

        private List<List<string>> GetSortedQuestions()
        {
            return questions
                .Where(question => question.Count >= 4)
                .OrderBy(question => int.TryParse(question[3], out int order) ? order : int.MaxValue)
                .ThenBy(question => question[2])
                .ToList();
        }

        private List<List<string>> GetDisplayableQuestions()
        {
            List<List<string>> sortedQuestions = GetSortedQuestions();
            List<List<string>> filteredQuestions = new();

            List<string> activePlayers = ds.GetGameSpelersClass().GetSpelersForGame(gameName)
                .Where(speler => speler.Count > 1 && speler[1] == "1")
                .Select(speler => speler[0])
                .ToList();

            List<List<string>> allAntwoorden = ds.GetAntwoordenClass().GetAntwoorden();

            foreach (List<string> vraag in sortedQuestions)
            {
                bool hasAnswerOption = false;

                foreach (List<string> a in allAntwoorden)
                {
                    if (a.Count < 3)
                        continue;

                    if (a[0] != vraag[1])
                        continue;

                    if (a[1] != vraag[2])
                        continue;

                    string connectedPlayersRaw = a.Count > 4 ? a[4] : "[]";

                    if (connectedPlayersRaw.Length > 4)
                    {
                        List<string> pqo = System.Text.Json.JsonSerializer.Deserialize<List<string>>(connectedPlayersRaw) ?? new();

                        bool answerNeeded = false;

                        foreach (string p in pqo)
                        {
                            if (activePlayers.Contains(p))
                                answerNeeded = true;
                        }

                        if (!answerNeeded)
                            continue;
                    }

                    hasAnswerOption = true;
                    break;
                }

                if (hasAnswerOption)
                    filteredQuestions.Add(vraag);
            }

            return filteredQuestions;
        }

        public Form20(Form previous, string testName, bool saveResult = false)
        {
            this.prev = previous;

            this.saveResult = saveResult;

            ds = Program.GetInfo();
            this.testName = testName;
            this.gameName = ds.GetTestsClass().GetGameForTest(testName) ?? string.Empty;

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

                // Rebuilds the Resultaten page from scratch (deselected test dropdown, no
                // results shown yet) instead of leaving the stale pre-test instance in place -
                // the user reselects the test to see the just-saved results.
                if (this.saveResult && this.prev is ShellForm shellForm)
                    shellForm.ShowSection(Section.UitvoerenResultaten);

                return;
            }

            Spelers spelersClass = ds.GetSpelersClass();
            bool foundSpeler = false;

            foreach (string s in spelersClass.GetSpelers())
            {
                if (s.ToLower() == name.ToLower())
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
            this.displayableQuestions.Clear();
            this.displayableQuestions.AddRange(GetDisplayableQuestions());

            this.currentAfnameId = this.saveResult
                ? ds.GetTestAfnamenClass().StartAfname(testName, name)
                : null;

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

            List<string>? currentVraag = displayableQuestions.ElementAtOrDefault(currentQuestion - 1);

            if (currentVraag == null || !currentAfnameId.HasValue)
                return false;

            return testAntwoorden.TryAddTestAntwoord(currentAfnameId.Value, currentVraag[1], currentVraag[2], geselecteerdAntwoord);
        }

        private void NextQuestion()
        {
            List<List<string>> sortedQuestions = displayableQuestions;

            while (currentQuestion < sortedQuestions.Count)
            {
                currentQuestion++;
                List<string> volgendeVraag = sortedQuestions[currentQuestion - 1];

                List<string> activePlayers = ds.GetGameSpelersClass().GetSpelersForGame(gameName)
                    .Where(speler => speler.Count > 1 && speler[1] == "1")
                    .Select(speler => speler[0])
                    .ToList();

                List<List<string>> antwoordMogelijkheden = new();

                foreach (List<string> a in ds.GetAntwoordenClass().GetAntwoorden())
                {
                    if (a.Count < 3)
                        continue;

                    if (a[0] != volgendeVraag[1])
                        continue;

                    if (a[1] != volgendeVraag[2])
                        continue;

                    string connectedPlayersRaw = a.Count > 4 ? a[4] : "[]";

                    if (connectedPlayersRaw.Length > 4)
                    {
                        List<string> pqo = System.Text.Json.JsonSerializer.Deserialize<List<string>>(connectedPlayersRaw) ?? new();

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
                    continue;

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
                return;
            }

            EindeTest();
        }

        private void EindeTest()
        {
            if (this.saveResult && this.currentAfnameId.HasValue)
                ds.GetTestAfnamenClass().EindeAfname(this.currentAfnameId.Value);

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
