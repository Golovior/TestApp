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
    public partial class Form17 : Form
    {
        readonly Form prev;

        readonly List<Tests>? tests;
        readonly DataSetInfo dsi;

        Tests? selectedTest;

        public Form17(Form previous)
        {
            prev = previous;
            this.dsi = Program.GetInfo();

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
            if (selectedTest == null)
                return;

            List<Questions>? questions = selectedTest.GetQuestions();

            if (questions == null)
            {
                label2.Text = "Er zitten geen vragen in deze test.";
                return;
            }

            Games? game = dsi.GetGameById(dsi.currentGame);

            if (game == null)
            {
                label2.Text = "Waarom zit deze test niet in een bestaande game???";
                return;
            }

            List<Players>? players = game.GetPlayers();

            if(players == null)
            {
                label2.Text = "Er zitten geen spelers in dit spel. Dus geen test te checken.";
                return;
            }

            List<Players> activePlayers = new();

            foreach(Players p in players)
            {
                if(p.GetStatus() == "Actief")
                {
                    activePlayers.Add(p);
                }
            }

            if(activePlayers.Count == 0)
            {
                label2.Text = "Geen actieve spelers in dit spel.";
                return;
            }
            
            foreach(Questions q in questions)
            {
                List<Options>? options = q.GetOptions();
                if(options == null)
                {
                    label2.Text = "Vraag: '" + q.GetQuestion() + "' heeft geen opties.";
                    return;
                }
                
                List<Players> playersWithOptions = new();
                List<Options> activeOptions = new();

                foreach (Options o in options)
                {
                    List<Players>? optionPlayers = o.GetPlayers();
                    if (optionPlayers == null)
                        continue;

                    foreach(Players op in optionPlayers)
                    {
                        foreach(Players ap in activePlayers)
                        {
                            if(ap.GetId() == op.GetId())
                            {
                                activeOptions.Add(o);
                                playersWithOptions.Add(ap);
                            }
                        }
                    }
                }

                if(playersWithOptions.Count != activePlayers.Count)
                {
                    label2.Text = "Niet alle actieve spelers hebben een geselecteerd antwoord voor vraag '" + q.GetQuestion() + "'";
                    return;
                }

                if (activeOptions.Count == 1)
                {
                    label2.Text = "Vraag '" + q.GetQuestion() + "' heeft maar 1 actieve optie '";
                    return;
                }
            }

            label2.Text = "Test lijkt volledig in orde.";
        }

        private void Button2_Click(object sender, EventArgs e)
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
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
