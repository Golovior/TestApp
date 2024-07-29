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
    public partial class Form21 : Form
    {
        readonly Form prev;
        readonly List<Tests>? tests;
        readonly DataSetInfo dsi;

        public Form21(Form previous)
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
            this.prev.Show();
        }

        private void ChangeTest(object sender, EventArgs e)
        {
            if (tests == null)
                return;
         
            string testName = comboBox1.Text;

            List<Running>? runnings = dsi.GetRunnings();

            if (runnings == null)
                return;

            Tests? selectedTest = null;

            foreach(Tests test in tests)
            {
                if(test.GetName() == testName)
                {
                    selectedTest = test;
                }
            }

            if (selectedTest == null)
                return;

            List<Running> testRunnings = new();

            foreach(Running r in runnings)
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

            List<string[]> setVanSpelers = new();

            foreach (Running r in testRunnings)
            {
                List<Antwoorden>? antwoordenList = dsi.GetAntwoorden();

                if (antwoordenList == null)
                    return;

                List<Antwoorden> gegevenAntwoorden = new();

                foreach (Antwoorden antwoord in antwoordenList)
                {
                    Running? antwoordRunning = antwoord.GetRunning();

                    if (antwoordRunning == null)
                        continue;

                    if (antwoordRunning.GetId() == r.GetId())
                    {
                        gegevenAntwoorden.Add(antwoord);
                    }
                }

                Players? speler = r.GetPlayer();

                if (speler == null)
                    return;

                string? spelerNaam = speler.GetName();

                if (spelerNaam == null)
                    return;

                int? startTime = r.GetStartTime();
                int? eindTime = r.GetEindTime();

                if (startTime == null)
                    return;

                if (eindTime == null)
                    return;

                int timeSpend = (int)eindTime - (int)startTime;

                string[] spelerInfo = new string[3];
                spelerInfo[0] = spelerNaam;
                spelerInfo[1] = "0";
                spelerInfo[2] = Convert.ToString(timeSpend);

                foreach(Antwoorden a in gegevenAntwoorden)
                {
                    if(a.CheckAntwoord())
                    {
                        spelerInfo[1] = Convert.ToString(Convert.ToInt32(spelerInfo[1]) + 1);
                    }
                }

                setVanSpelers.Add(spelerInfo);
            }

            this.showResultaten(setVanSpelers);
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
