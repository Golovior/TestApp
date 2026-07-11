using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace TestApp
{
    public class UitvoerenResultatenControl : UserControl
    {
        private readonly ShellForm shell;
        private readonly DataSetClass ds;

        private ComboBox testCombo = null!;
        private Panel resultatenPanel = null!;
        private ComboBox spelerCombo = null!;
        private Panel antwoordenPanel = null!;

        public UitvoerenResultatenControl(ShellForm shell)
        {
            this.shell = shell;
            ds = Program.GetInfo();

            TableLayoutPanel root = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            root.Controls.Add(BuildTestHeaderRow(), 0, 0);

            TabControl tabs = new() { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildResultatenTab());
            tabs.TabPages.Add(BuildAntwoordenTab());
            root.Controls.Add(tabs, 0, 1);

            Controls.Add(root);

            foreach (string test in ds.GetTestsClass().GetAllTests())
                testCombo.Items.Add(test);
        }

        // --- Test selector + start (018_TestUitvoeren, saveResults:true) ---
        private Panel BuildTestHeaderRow()
        {
            Panel panel = new() { Height = 40, Dock = DockStyle.Fill };

            Label testLabel = new() { Text = "Test", Location = new Point(12, 9), AutoSize = true };
            testCombo = new ComboBox { Location = new Point(90, 6), Size = new Size(260, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            testCombo.SelectedIndexChanged += (s, e) => OnTestChanged();

            Button startButton = new() { Text = "Test starten", Location = new Point(360, 5), Size = new Size(140, 23) };
            startButton.Click += (s, e) => StartTest();

            panel.Controls.Add(testLabel);
            panel.Controls.Add(testCombo);
            panel.Controls.Add(startButton);
            return panel;
        }

        private void StartTest()
        {
            string test = testCombo.Text;
            if (test == "")
                return;

            if (ds.GetTestsClass().GetGameForTest(test) == null)
            {
                MessageBox.Show("Deze test is nog niet aan een spel gekoppeld. Koppel eerst een spel via Test aanmaken.");
                return;
            }

            // Form20 (019_TestRunnen) is intentionally untouched, including its own
            // FormClosing handler which calls Application.Exit() on the OS "X" button
            // (unlike everywhere else in this app) - this is pre-existing behavior.
            Form20 form = new(shell, test, true);
            shell.Hide();
            form.Show();
        }

        private void OnTestChanged()
        {
            RefreshResultaten();
            RefreshSpelersVoorTest();
        }

        // --- Resultaten (020_TestControleren) ---
        private TabPage BuildResultatenTab()
        {
            TabPage page = new("Resultaten");
            resultatenPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            page.Controls.Add(resultatenPanel);
            return page;
        }

        private void RefreshResultaten()
        {
            resultatenPanel.Controls.Clear();

            string test = testCombo.Text;
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

            List<List<string>> afnamen = testAfnamen.GetAllTestAfnamen().Where(a => a[0] == test).ToList();
            foreach (List<string> afname in afnamen)
            {
                if (!spelers.Contains(afname[1]))
                    spelers.Add(afname[1]);
            }

            List<List<string>> juisteAntwoorden = antwoorden.GetAntwoorden().Where(a => a[3] == "1").ToList();

            List<List<string>> score = new();
            foreach (string speler in spelers)
            {
                int testScore = 0;
                foreach (List<string> antwoord in gegevenAntwoorden)
                {
                    if (antwoord[1] != speler)
                        continue;

                    foreach (List<string> ja in juisteAntwoorden)
                    {
                        if (ja[0] != antwoord[2] || ja[1] != antwoord[3])
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
                    timeSpend = ((latestEinde.Value - bijbehorendeStart.Value).TotalSeconds).ToString(CultureInfo.InvariantCulture);

                score.Add(new List<string> { speler, testScore.ToString(), timeSpend });
            }

            score.Sort((a, b) => (a[1] + a[2]).CompareTo(b[1] + b[2]));

            int key = 0;
            foreach (List<string> info in score)
            {
                string speler = info[0];
                Label naam = new() { AutoSize = true, Location = new Point(0, 10 + key * 20), Text = speler };
                Label juist = new() { AutoSize = true, Location = new Point(220, 10 + key * 20), Text = info[1] + " juist" };
                Label tijd = new() { AutoSize = true, Location = new Point(360, 10 + key * 20), Text = info[2] + " sec" };

                Button remove = new() { Text = "Verwijderen", Location = new Point(460, 6 + key * 20), Size = new Size(100, 23) };
                remove.Click += (s, e) =>
                {
                    if (!UiHelpers.ConfirmDelete($"Alle testresultaten van \"{speler}\" voor deze test verwijderen?"))
                        return;

                    foreach (List<string> afname in afnamen.Where(a => a[1] == speler))
                        testAfnamen.DeleteTestAfname(Guid.Parse(afname[5]));

                    OnTestChanged();
                };

                resultatenPanel.Controls.Add(naam);
                resultatenPanel.Controls.Add(juist);
                resultatenPanel.Controls.Add(tijd);
                resultatenPanel.Controls.Add(remove);
                key++;
            }
        }

        // --- Antwoorden (021_AntwoordenControleren) ---
        private TabPage BuildAntwoordenTab()
        {
            TabPage page = new("Antwoorden");

            Panel headerRow = new() { Height = 32, Dock = DockStyle.Top };
            Label spelerLabel = new() { Text = "Speler", Location = new Point(4, 6), AutoSize = true };
            spelerCombo = new ComboBox { Location = new Point(70, 3), Size = new Size(240, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            spelerCombo.SelectedIndexChanged += (s, e) => RefreshAntwoordenVoorSpeler();
            headerRow.Controls.Add(spelerLabel);
            headerRow.Controls.Add(spelerCombo);

            antwoordenPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            page.Controls.Add(antwoordenPanel);
            page.Controls.Add(headerRow);
            return page;
        }

        private void RefreshSpelersVoorTest()
        {
            spelerCombo.Items.Clear();
            spelerCombo.Text = "";
            antwoordenPanel.Controls.Clear();

            string testName = testCombo.Text;
            if (testName == "")
                return;

            List<string> spelers = new();
            foreach (List<string> a in ds.GetTestAntwoordenClass().GetAllTestAntwoorden())
            {
                if (a[0] != testName)
                    continue;

                if (!spelers.Contains(a[1]))
                    spelers.Add(a[1]);
            }

            spelers.Sort();
            foreach (string speler in spelers)
                spelerCombo.Items.Add(speler);
        }

        private void RefreshAntwoordenVoorSpeler()
        {
            antwoordenPanel.Controls.Clear();

            string testName = testCombo.Text;
            string speler = spelerCombo.Text;
            if (speler == "")
                return;

            List<List<string>> antwoordenSet = ds.GetTestAntwoordenClass().GetAllTestAntwoorden()
                .Where(a => a[0] == testName && a[1] == speler)
                .ToList();

            int key = 0;
            foreach (List<string> i in antwoordenSet)
            {
                Guid id = Guid.Parse(i[5]);
                Label vraagLabel = new() { AutoSize = true, Location = new Point(0, 10 + key * 15), Text = i[3] };
                Label antwoordLabel = new() { AutoSize = true, Location = new Point(361, 10 + key * 15), Text = i[4] };

                Button remove = new() { Text = "X", Location = new Point(560, 8 + key * 15), Size = new Size(29, 20) };
                remove.Click += (s, e) =>
                {
                    if (!UiHelpers.ConfirmDelete($"Antwoord op \"{i[3]}\" van deze speler verwijderen?"))
                        return;

                    ds.GetTestAntwoordenClass().DeleteTestAntwoord(id);
                    RefreshAntwoordenVoorSpeler();
                };

                antwoordenPanel.Controls.Add(vraagLabel);
                antwoordenPanel.Controls.Add(antwoordLabel);
                antwoordenPanel.Controls.Add(remove);
                key++;
            }
        }
    }
}
