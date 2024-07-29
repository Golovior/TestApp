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
        readonly bool isRealTest;
        Tests? test;
        List<Questions>? questions;

        readonly DataSetInfo dsi;

        readonly Games currentGame;
        Players? currentPlayer;
        Questions? currentQuestion;

        readonly int daySeconds;

        Running? thisRun;
        int nextQuestionId = 0;

        public Form20(Form previous, bool realTest)
        {
            this.prev = previous;
            this.isRealTest = realTest;
            this.daySeconds = Convert.ToInt32(new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds());

            dsi = Program.GetInfo();

            currentGame = dsi.GetGameById(dsi.currentGame);

            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            this.repositionElements();
        }

        public void SetTest(Tests t)
        {
            test = t;
            questions = test.GetQuestions();
            return;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            currentPlayer = null;
            string name = textBox1.Text;

            if(textBox1.Text == "Exit")
            {
                this.Dispose();
                this.prev.Show();
            }

            nextQuestionId = 0;

            if (!isRealTest)
            {
                this.ShowQuestionLayout();
                this.Button2_Click(sender, e);
                return;
            }

            List<Players>? playersInGame = currentGame.GetPlayers();

            if (playersInGame == null)
                return;

            foreach(Players p in playersInGame)
            {
                if (name == p.GetName())
                    currentPlayer = p;
            }

            if (currentPlayer == null)
                return;

            List<Running>? allRunnings = dsi.GetRunnings();

            int nextId = 0;
            if (allRunnings != null)
                nextId = allRunnings.Count;

            thisRun = new Running();
            thisRun.SetId(nextId);
            thisRun.SetStartTime(Convert.ToInt32((DateTimeOffset.UtcNow.ToUnixTimeSeconds() - daySeconds) * 1000) + DateTimeOffset.UtcNow.Millisecond);
            thisRun.SetPlayer(currentPlayer);
            thisRun.SetTest(test);

            thisRun.WriteToFile();
            dsi.AddToRunningList(thisRun);

            this.ShowQuestionLayout();

            this.Button2_Click(sender, e);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (questions == null || (thisRun == null && isRealTest))
                return;

            if (nextQuestionId > 0)
            {
                if (radioButtons == null || currentQuestion == null)
                    return;

                List<Options>? optionsForQuestion = currentQuestion.GetOptions();

                if (optionsForQuestion == null)
                    return;

                Options selectedOption = new();

                foreach(RadioButton r in radioButtons)
                {
                    if(r.Checked)
                    {
                        foreach(Options o in optionsForQuestion)
                        {
                            if(o.GetValue() == r.Text)
                            {
                                selectedOption = o;
                                break;
                            }
                        }
                        break;
                    }
                }

                if (selectedOption.GetValue() == null)
                    return;

                if (isRealTest)
                {
                    int antwoordId = 0;

                    List<Antwoorden>? alleAntwoorden = dsi.GetAntwoorden();

                    if (alleAntwoorden != null)
                        antwoordId = alleAntwoorden.Count;

                    Antwoorden antwoord = new();
                    antwoord.SetRunning(thisRun);
                    antwoord.SetId(antwoordId);
                    antwoord.SetOption(selectedOption);

                    antwoord.WriteToFile();

                    dsi.AddToAntwoordList(antwoord);
                }
            }


            if(nextQuestionId == questions.Count)
            {
                if(isRealTest && thisRun != null)
                {
                    thisRun.SetEindTime(Convert.ToInt32((DateTimeOffset.UtcNow.ToUnixTimeSeconds() - daySeconds) * 1000) + DateTimeOffset.UtcNow.Millisecond);
                    thisRun.UpdateInFile();
                }

                this.ShowStartup();
                return;
            }

            currentQuestion = questions[nextQuestionId];

            nextQuestionId++;

            this.PrepNextQuestion(currentQuestion);
        }

        public void ShowStartup()
        {
            panel1.Controls.Clear();
            panel2.Controls.Clear();

            panel1.Visible = false;
            panel2.Visible = false;

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
            panel2.Visible = true;

            label5.Visible = true;

            label2.Visible = true;
            label3.Visible = true;

            button2.Visible = true;

            button1.Visible = false;
            label1.Visible = false;
            label4.Visible = false;
            textBox1.Visible = false;
        }

        public void PrepNextQuestion(Questions question)
        {
            panel1.Controls.Clear();
            panel2.Controls.Clear();

            label2.Text = Convert.ToString(nextQuestionId);
            label3.Text = question.GetQuestion();

            List<Options>? options = question.GetOptions();

            if (options == null)
                return;

            List<Players>? allPlayers = currentGame.GetPlayers();
            List<Players> activePlayers = new();

            if (allPlayers == null)
                return;

            foreach (Players player in allPlayers)
            {
                if(player.GetStatus() == "Actief")
                    activePlayers.Add(player);
            }

            List<Options> activeOptions = new();

            foreach (Options option in options)
            {
                bool hasOption = false;
                List<Players>? playersForOption = option.GetPlayers();

                if (playersForOption == null)
                    continue;

                foreach(Players p in playersForOption)
                {
                    foreach(Players ap in activePlayers)
                    {
                        if(ap.GetId() == p.GetId() && !hasOption)
                        {
                            hasOption = true;
                            activeOptions.Add(option);
                        }
                    }
                }
            }

            if (activeOptions.Count == 0)
                throw new Exception("Er zijn geen antwoorden voor deze vraag.");

            this.prepareQuestion(activeOptions);

            return;
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
