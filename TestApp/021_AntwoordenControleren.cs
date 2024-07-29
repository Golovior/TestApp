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
    public partial class Form22 : Form
    {
        readonly Form prev;
        readonly List<Tests>? tests;
        readonly DataSetInfo dsi;
        List<Running>? testRunnings;

        public Form22(Form previous)
        {
            this.dsi = Program.GetInfo();
            this.prev = previous;

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

        private void ChangeTest(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Text = "";

            if (tests == null)
                return;

            string testName = comboBox1.Text;

            List<Running>? runnings = dsi.GetRunnings();

            if (runnings == null)
                return;

            Tests? selectedTest = null;

            foreach (Tests test in tests)
            {
                if (test.GetName() == testName)
                {
                    selectedTest = test;
                }
            }

            if (selectedTest == null)
                return;

            testRunnings = new List<Running>();

            foreach (Running r in runnings)
            {
                Tests? testForRunning = r.GetTest();
                if (testForRunning == null)
                    continue;

                if (testForRunning.GetId() == selectedTest.GetId())
                {
                    testRunnings.Add(r);
                }
            }

            if (testRunnings.Count == 0)
                return;

            List<string> playersWhoMadeTest = new();

            foreach (Running r in testRunnings)
            {
                Players? speler = r.GetPlayer();

                if (speler == null)
                    return;

                string? spelerNaam = speler.GetName();

                if (spelerNaam == null)
                    return;

                playersWhoMadeTest.Add(spelerNaam);
            }

            comboBox2.Items.AddRange(playersWhoMadeTest.ToArray());
        }

        private void ChangePlayer(object sender, EventArgs e)
        {
            string name = comboBox2.Text;

            if (name == null)
                return;

            if (testRunnings == null)
                return;

            Running? selectedRunning = null;

            foreach (Running r in testRunnings)
            {
                Players? speler = r.GetPlayer();

                if (speler == null)
                    continue;

                if (speler.GetName() == name)
                {
                    selectedRunning = r;
                    break;
                }
            }

            if (selectedRunning == null)
                return;

            List<Antwoorden>? alleAntwoorden = dsi.GetAntwoorden();
            List<Antwoorden> interessanteAntwoorden = new();

            if (alleAntwoorden == null)
                return;

            foreach(Antwoorden a in alleAntwoorden)
            {
                Running? run = a.GetRunning();

                if (run == null)
                    continue;

                if(run.GetId() == selectedRunning.GetId())
                    interessanteAntwoorden.Add(a);
            }

            if (interessanteAntwoorden.Count == 0)
                return;

            List<string[]> gegevenAntwoorden = new();

            foreach(Antwoorden antwoord in interessanteAntwoorden)
            {
                string[] vraaginfo = new string[2];

                Options o = antwoord.GetOption();
                string? option = o.GetValue();
                
                if (option == null)
                    continue;

                Questions? vraag = o.GetQuestion();

                if (vraag == null)
                    continue;

                string? vraagValue = vraag.GetQuestion();

                if (vraagValue == null)
                    continue;

                vraaginfo[0] = vraagValue;
                vraaginfo[1] = option;

                gegevenAntwoorden.Add(vraaginfo);
            }

            this.showAntwoordenFromPlayer(gegevenAntwoorden);
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
