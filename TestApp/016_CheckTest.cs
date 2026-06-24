using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form17 : Form
    {
        readonly Form prev;
        readonly DataSetClass ds;

        readonly List<string> tests;

        public Form17(Form previous)
        {
            prev = previous;
            ds = Program.GetInfo();
        
            InitializeComponent();

            tests = ds.GetTestsClass().GetAllTests();

            FillComboboxWithTests(tests);
        }

        private void FillComboboxWithTests(List<string> tests)
        {
            foreach (string test in tests)
            {
                comboBox1.Items.Add(test);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string testName = comboBox1.Text;

            List<string> errors = new();

            if (testName == "")
                return;

            List<List<string>> testVragen = ds.GetTestVragenClass().GetAllTestVragen();
            List<List<string>> allPlayers = ds.GetSpelersClass().GetSpelers();

            List<List<string>> vragen = new();
            List<string> activePlayers = new();

            foreach(List<string> questions in testVragen)
            {
                if (questions[0] == testName)
                    vragen.Add(questions);
            }

            foreach(List<string> speler in allPlayers)
            {
                if (speler[1] == "1")
                    activePlayers.Add(speler[0]);
            }

            activePlayers.Sort();
            List<List<string>> antwoorden = ds.GetAntwoordenClass().GetAntwoorden();

            int juisteAntwoorden = 0;

            foreach (List<string> vraag in vragen)
            {
                List<List<string>> mogelijkeAntwoorden = new();
                List<string> playersForQuestionOptions = new();

                bool juistGevonden = false;

                foreach(List<string> antwoord in antwoorden)
                {
                    if (antwoord[0] != vraag[1])
                    {
                        continue;
                    }

                    if (antwoord[1] != vraag[2])
                    {
                        continue;
                    }

                    mogelijkeAntwoorden.Add(antwoord);

                    if (antwoord[3] == "1")
                    {
                        if(!juistGevonden)
                            juistGevonden = true;
                        else 
                        {
                            string error = "Vraag \"" + antwoord[1] + "\" heeft meerdere antwoorden juist.";
                            errors.Add(error);
                        }
                    }

                    if (antwoord[4].Length > 4)
                    {
                        List<string> pqo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(antwoord[4]) ?? new();
                        foreach(string s in pqo)
                        {
                            playersForQuestionOptions.Add(s); 
                        }
                    }
                }

                if (playersForQuestionOptions.Count > 0)
                {
                    bool foundAllPlayers = true;
                    foreach (string ap in activePlayers)
                    {
                        if (!playersForQuestionOptions.Contains(ap))
                            foundAllPlayers = false;
                    }

                    if (!foundAllPlayers)
                    {
                        string error = "Vraag \"" + vraag[2] + "\" heeft niet voor alle spelers een geselecteerd antwoord.";
                        errors.Add(error);
                    }

                    if(playersForQuestionOptions.Count > playersForQuestionOptions.Distinct().Count())
                    {
                        string error = "Vraag \"" + vraag[2] + "\" heeft spelers dubbel gekoppeld aan een antwoord.";
                        errors.Add(error);
                    }
                }

                if (mogelijkeAntwoorden.Count == 0)
                {
                    string error = "Vraag \"" + vraag[2] + "\" heeft geen antwoorden.";
                    errors.Add(error);
                }
                else
                {
                    if (juistGevonden)
                        juisteAntwoorden++;
                    else
                    {
                        string error = "Vraag \"" + vraag[2] + "\" heeft geen juist antwoord geselecteerd.";
                        errors.Add(error);
                    }
                }
            }

            int order = 0;
            foreach (string error in errors)
            {
                MakeErrorRow(error, order);
                order++;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            prev.Show();
        }
    }
}
