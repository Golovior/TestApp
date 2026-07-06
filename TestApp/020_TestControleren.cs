using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace TestApp
{
    public partial class Form21 : BaseForm
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public Form21(Form previous)
        {
            this.prev = previous;
            ds = Program.GetInfo();

            InitializeComponent();

            FillComboboxWithTests();

        }

        public void FillComboboxWithTests()
        {
            List<string> tests = ds.GetTestsClass().GetAllTests();

            foreach (string test in tests)
            {
                comboBox1.Items.Add(test);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.prev.Show();
        }

        private void ChangeTest(object sender, EventArgs e)
        {
            string test = comboBox1.Text;

            if (test == "")
                return;

            TestAntwoorden testAntwoorden = ds.GetTestAntwoordenClass();
            Antwoorden antwoorden = ds.GetAntwoordenClass();
            TestAfnamen testAfnamen = ds.GetTestAfnamenClass();

            List<List<string>> gegevenAntwoorden = new();
            List<string> spelers = new();

            foreach (List<string> a in testAntwoorden.GetAllTestAntwoorden())
            {
                if (a[0] != test)
                    continue;

                gegevenAntwoorden.Add(a);
                if (!spelers.Contains(a[1]))
                    spelers.Add(a[1]);
            }

            List<List<string>> afnamen = testAfnamen.GetAllTestAfnamen()
                .Where(a => a[0] == test)
                .ToList();

            foreach (List<string> afname in afnamen)
            {
                if (!spelers.Contains(afname[1]))
                    spelers.Add(afname[1]);
            }

            List<List<string>> juisteAntwoorden = new();

            foreach(List<string> a in antwoorden.GetAntwoorden())
            {
                if (a[3] == "1")
                    juisteAntwoorden.Add(a);
            }

            List<List<string>> score = new();

            foreach(string speler in spelers)
            {
                List<string> spelerInfo = new()
                {
                    speler
                };
                int testScore = 0;

                foreach(List<string> antwoord in gegevenAntwoorden)
                {
                    if (antwoord[1] != speler)
                        continue;

                    foreach(List<string> ja in juisteAntwoorden)
                    {
                        if (ja[0] != antwoord[2])
                            continue;

                        if (ja[1] != antwoord[3])
                            continue;

                        if (ja[2] == antwoord[4])
                            testScore++;
                    }
                }

                string timeSpend = "0";
                DateTime? latestEinde = null;
                DateTime? bijbehorendeStart = null;

                foreach (List<string> afname in afnamen)
                {
                    if (afname[1] != speler || string.IsNullOrEmpty(afname[3]))
                        continue;

                    DateTime eindtijd = DateTime.Parse(afname[3], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

                    if (latestEinde.HasValue && eindtijd <= latestEinde.Value)
                        continue;

                    latestEinde = eindtijd;
                    bijbehorendeStart = DateTime.Parse(afname[2], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }

                if (latestEinde.HasValue && bijbehorendeStart.HasValue)
                    timeSpend = Convert.ToString((latestEinde.Value - bijbehorendeStart.Value).TotalSeconds);

                spelerInfo.Add(Convert.ToString(testScore));
                spelerInfo.Add(timeSpend);
                score.Add(spelerInfo);
            }

            score.Sort(delegate (List<string> a, List<string> b)
            {
                string sa = a[1] + a[2];
                string sb = b[1] + b[2];
                return sa.CompareTo(sb);
            });

            showResultaten(score);
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            this.prev.Show();
        }
    }
}
