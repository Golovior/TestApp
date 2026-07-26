using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestApp
{
    public class TestsControl : UserControl
    {
        private readonly ShellForm shell;
        private readonly DataSetClass ds;

        private readonly List<Button> buttonsUp = new();
        private readonly List<Button> buttonsDown = new();
        private readonly List<Button> buttonsRemove = new();
        private readonly List<Label> vraagLabels = new();
        private readonly List<Label> errorLabels = new();

        private ComboBox testCombo = null!;
        private ComboBox linkGameCombo = null!;
        private ComboBox newTestGameCombo = null!;
        private TextBox newTestNameBox = null!;
        private ComboBox opdrachtCombo = null!;
        private ComboBox vraagCombo = null!;
        private Label vragenCountLabel = null!;
        private Panel vragenPanel = null!;
        private Panel errorsPanel = null!;

        public TestsControl(ShellForm shell)
        {
            this.shell = shell;
            ds = Program.GetInfo();

            TableLayoutPanel root = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(BuildTestHeaderRow(), 0, 0);
            root.Controls.Add(BuildVraagToevoegenRow(), 0, 1);
            root.Controls.Add(BuildVragenListRow(), 0, 2);
            root.Controls.Add(BuildVolgordeCheckRow(), 0, 3);
            root.Controls.Add(BuildErrorsRow(), 0, 4);
            root.Controls.Add(BuildDoorklikkenRow(), 0, 5);

            Controls.Add(root);

            RefreshTestCombo();
        }

        // --- Row 0: test selector + create new test + (re)link game (012_TestAanmaken) ---
        private Panel BuildTestHeaderRow()
        {
            Panel panel = new() { Height = 96, Dock = DockStyle.Fill };

            Label testLabel = new() { Text = "Test", Location = new Point(12, 12), AutoSize = true };
            testCombo = new ComboBox { Location = new Point(90, 9), Size = new Size(260, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            testCombo.SelectedIndexChanged += (s, e) => OnTestChanged();

            Button deleteTestButton = new() { Text = "Verwijderen", Location = new Point(670, 44), Size = new Size(100, 23) };
            deleteTestButton.Click += (s, e) =>
            {
                string test = testCombo.Text;
                if (test == "")
                    return;

                if (!UiHelpers.ConfirmDelete($"Test \"{test}\" en alle bijbehorende testvragen verwijderen?"))
                    return;

                ds.GetTestsClass().DeleteTest(test);
                RefreshTestCombo();
                testCombo.Text = "";
                OnTestChanged();
            };

            Label linkLabel = new() { Text = "Gekoppeld spel", Location = new Point(360, 12), AutoSize = true };
            linkGameCombo = new ComboBox { Location = new Point(460, 9), Size = new Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (string game in ds.GetGamesClass().GetAllGames())
                linkGameCombo.Items.Add(game);
            Button linkButton = new() { Text = "Koppelen", Location = new Point(670, 9), Size = new Size(100, 23) };
            linkButton.Click += (s, e) =>
            {
                string test = testCombo.Text;
                string game = linkGameCombo.Text;
                if (test == "" || game == "")
                    return;

                ds.GetTestsClass().SetGameForTest(test, game);
            };

            Label newTestLabel = new() { Text = "Nieuwe test", Location = new Point(12, 47), AutoSize = true };
            newTestNameBox = new TextBox { Location = new Point(90, 44), Size = new Size(180, 23) };
            Label newTestGameLabel = new() { Text = "Spel", Location = new Point(280, 47), AutoSize = true };
            newTestGameCombo = new ComboBox { Location = new Point(315, 44), Size = new Size(160, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (string game in ds.GetGamesClass().GetAllGames())
                newTestGameCombo.Items.Add(game);
            Button newTestButton = new() { Text = "Aanmaken", Location = new Point(485, 44), Size = new Size(100, 23) };
            newTestButton.Click += (s, e) =>
            {
                if (newTestNameBox.Text == "" || newTestGameCombo.Text == "")
                    return;

                Tests tests = ds.GetTestsClass();
                string test = newTestNameBox.Text;

                if (tests.TestAlreadyExists(test))
                    return;

                tests.AddTest(test);
                tests.SetGameForTest(test, newTestGameCombo.Text);

                newTestNameBox.Text = "";
                newTestNameBox.Focus();

                RefreshTestCombo();
                testCombo.SelectedItem = test;
            };

            panel.Controls.Add(testLabel);
            panel.Controls.Add(testCombo);
            panel.Controls.Add(deleteTestButton);
            panel.Controls.Add(linkLabel);
            panel.Controls.Add(linkGameCombo);
            panel.Controls.Add(linkButton);
            panel.Controls.Add(newTestLabel);
            panel.Controls.Add(newTestNameBox);
            panel.Controls.Add(newTestGameLabel);
            panel.Controls.Add(newTestGameCombo);
            panel.Controls.Add(newTestButton);
            return panel;
        }

        // --- Row 1: add a vraag to the test (013_TestvragenToevoegen) ---
        private Panel BuildVraagToevoegenRow()
        {
            Panel panel = new() { Height = 40, Dock = DockStyle.Fill };

            Label opdrachtLabel = new() { Text = "Opdracht", Location = new Point(12, 9), AutoSize = true };
            opdrachtCombo = new ComboBox { Location = new Point(90, 6), Size = new Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (string opdracht in ds.GetOpdrachtenClass().GetOpdrachten())
                opdrachtCombo.Items.Add(opdracht);
            opdrachtCombo.SelectedIndexChanged += (s, e) => OnOpdrachtChanged();

            Label vraagLabel = new() { Text = "Vraag", Location = new Point(300, 9), AutoSize = true };
            vraagCombo = new ComboBox { Location = new Point(345, 6), Size = new Size(600, 23), DropDownStyle = ComboBoxStyle.DropDownList };

            vragenCountLabel = new Label { Text = "0 vragen toegevoegd", Location = new Point(955, 9), AutoSize = true };

            Button addButton = new() { Text = "Vraag toevoegen", Location = new Point(1100, 5), Size = new Size(140, 23) };
            addButton.Click += (s, e) => AddVraagToTest();

            panel.Controls.Add(opdrachtLabel);
            panel.Controls.Add(opdrachtCombo);
            panel.Controls.Add(vraagLabel);
            panel.Controls.Add(vraagCombo);
            panel.Controls.Add(vragenCountLabel);
            panel.Controls.Add(addButton);
            return panel;
        }

        private Panel BuildVragenListRow()
        {
            vragenPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BorderStyle = BorderStyle.FixedSingle };
            Panel wrapper = new() { Dock = DockStyle.Fill };
            wrapper.Controls.Add(vragenPanel);
            return wrapper;
        }

        private Panel BuildVolgordeCheckRow()
        {
            Panel panel = new() { Height = 36, Dock = DockStyle.Fill };
            Button saveOrderButton = new() { Text = "Volgorde opslaan", Location = new Point(12, 6), Size = new Size(200, 23) };
            saveOrderButton.Click += (s, e) => SaveOrder();

            Button checkButton = new() { Text = "Check test", Location = new Point(220, 6), Size = new Size(200, 23) };
            checkButton.Click += (s, e) => CheckTest();

            panel.Controls.Add(saveOrderButton);
            panel.Controls.Add(checkButton);
            return panel;
        }

        private Panel BuildErrorsRow()
        {
            errorsPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BorderStyle = BorderStyle.FixedSingle };
            Panel wrapper = new() { Dock = DockStyle.Fill };
            wrapper.Controls.Add(errorsPanel);
            return wrapper;
        }

        private Panel BuildDoorklikkenRow()
        {
            Panel panel = new() { Height = 36, Dock = DockStyle.Fill };
            Button practiceButton = new() { Text = "Test doorklikken (oefenen, niet opslaan)", Location = new Point(12, 6), Size = new Size(400, 23) };
            practiceButton.Click += (s, e) => StartTest(saveResults: false);
            panel.Controls.Add(practiceButton);
            return panel;
        }

        private void RefreshTestCombo()
        {
            string previouslySelected = testCombo.Text;
            testCombo.Items.Clear();
            foreach (string test in ds.GetTestsClass().GetAllTests())
                testCombo.Items.Add(test);

            if (testCombo.Items.Contains(previouslySelected))
                testCombo.SelectedItem = previouslySelected;
        }

        private void OnTestChanged()
        {
            vragenCountLabel.Text = "0 vragen toegevoegd";
            vragenPanel.Controls.Clear();
            errorsPanel.Controls.Clear();
            buttonsUp.Clear();
            buttonsDown.Clear();
            buttonsRemove.Clear();
            vraagLabels.Clear();
            errorLabels.Clear();

            string test = testCombo.Text;
            if (test == "")
                return;

            List<List<string>> alleTestVragen = ds.GetTestVragenClass().GetAllTestVragen();
            int count = alleTestVragen.Count(v => v[0] == test);
            vragenCountLabel.Text = count + " vragen toegevoegd";

            List<List<string>> vraagVoorTest = alleTestVragen.Where(v => v[0] == test).ToList();
            List<List<string>> ordered = new();
            for (int i = 0; i <= alleTestVragen.Count; i++)
                ordered.AddRange(vraagVoorTest.Where(v => Convert.ToInt32(v[3]) == i));

            foreach (List<string> vraag in ordered)
                MakeQuestionRow(vraag);
        }

        private void OnOpdrachtChanged()
        {
            vraagCombo.Items.Clear();
            string opdracht = opdrachtCombo.Text;
            if (opdracht == "")
                return;

            foreach (List<string> vraag in ds.GetQuestionsClass().GetAllQuestions())
            {
                if (vraag[0] == opdracht)
                    vraagCombo.Items.Add(vraag[1]);
            }
        }

        private void AddVraagToTest()
        {
            string test = testCombo.Text;
            string opdracht = opdrachtCombo.Text;
            string vraag = vraagCombo.Text;

            if (test == "" || opdracht == "" || vraag == "")
                return;

            TestVragen testVragen = ds.GetTestVragenClass();

            if (testVragen.TestIsAfgenomen(test))
            {
                MessageBox.Show("Deze test is al afgenomen en kan niet meer aangepast worden.");
                return;
            }

            if (testVragen.QuestionAlreadyExists(test, opdracht, vraag))
                return;

            int amount = int.Parse(vragenCountLabel.Text.Split(' ')[0]) + 1;
            testVragen.AddTestVraag(test, opdracht, vraag, amount.ToString());

            vraagCombo.Text = "";
            vragenCountLabel.Text = amount + " vragen toegevoegd";
            OnTestChanged();
        }

        private void MakeQuestionRow(List<string> vraag)
        {
            int order = Convert.ToInt32(vraag[3]);
            int height = 27 * order;
            int panelWidth = vragenPanel.ClientSize.Width;
            int upX = panelWidth - 139;
            int downX = panelWidth - 104;
            int removeX = panelWidth - 69;

            Button up = new()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(upX, height),
                Size = new Size(31, 23),
                Text = "^",
                Name = "Up~" + vraag[3]
            };
            up.Click += ButtonUp_click;

            Button down = new()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(downX, height),
                Size = new Size(29, 23),
                Text = "v",
                Name = "Down~" + vraag[3]
            };
            down.Click += ButtonDown_click;

            Button remove = new()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(removeX, height),
                Size = new Size(29, 23),
                Text = "X",
                Name = "Remove~" + vraag[3]
            };
            remove.Click += ButtonRemove_click;

            Label label = new()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(12, height + 4),
                Size = new Size(upX - 24, 18),
                Text = vraag[2],
                Name = "label~" + vraag[3]
            };

            vragenPanel.Controls.Add(up);
            vragenPanel.Controls.Add(down);
            vragenPanel.Controls.Add(remove);
            vragenPanel.Controls.Add(label);

            buttonsUp.Add(up);
            buttonsDown.Add(down);
            buttonsRemove.Add(remove);
            vraagLabels.Add(label);
        }

        private void ButtonUp_click(object? sender, EventArgs e)
        {
            Button clicked = (Button)sender!;
            string name = clicked.Name!.Replace("Up~", "");
            string prevId = (Convert.ToInt32(name) - 1).ToString();
            if (prevId == "0")
                return;

            SwapLabels(name, prevId);
        }

        private void ButtonDown_click(object? sender, EventArgs e)
        {
            Button clicked = (Button)sender!;
            string name = clicked.Name!.Replace("Down~", "");
            string nextId = (Convert.ToInt32(name) + 1).ToString();

            SwapLabels(name, nextId);
        }

        private void SwapLabels(string idA, string idB)
        {
            Label? labelA = vraagLabels.FirstOrDefault(l => l.Name == "label~" + idA);
            Label? labelB = vraagLabels.FirstOrDefault(l => l.Name == "label~" + idB);
            if (labelA == null || labelB == null)
                return;

            (labelA.Text, labelB.Text) = (labelB.Text, labelA.Text);
        }

        private void ButtonRemove_click(object? sender, EventArgs e)
        {
            Button clicked = (Button)sender!;
            string name = clicked.Name!.Replace("Remove~", "");
            Label? label = vraagLabels.FirstOrDefault(l => l.Name == "label~" + name);
            if (label == null)
                return;

            string testName = testCombo.Text;
            if (ds.GetTestVragenClass().TestIsAfgenomen(testName))
            {
                MessageBox.Show("Deze test is al afgenomen en kan niet meer aangepast worden.");
                return;
            }

            if (!UiHelpers.ConfirmDelete($"Vraag \"{label.Text}\" uit deze test verwijderen?"))
                return;

            List<List<string>> testvragen = ds.GetTestVragenClass().GetAllTestVragen();
            foreach (List<string> tv in testvragen)
            {
                if (tv[0] != testName)
                    continue;

                if (tv[2] == label.Text)
                {
                    ds.GetTestVragenClass().RemoveTestVragen(tv);
                    break;
                }
            }

            SaveOrder();
            OnTestChanged();
        }

        private void SaveOrder()
        {
            TestVragen testVragen = ds.GetTestVragenClass();
            string test = testCombo.Text;
            if (test == "")
                return;

            if (testVragen.TestIsAfgenomen(test))
            {
                MessageBox.Show("Deze test is al afgenomen en kan niet meer aangepast worden.");
                return;
            }

            List<List<string>> alleVragen = testVragen.GetAllTestVragen();
            List<List<string>> vraagVoorTest = alleVragen.Where(v => v[0] == test).ToList();

            foreach (List<string> vraag in vraagVoorTest)
                testVragen.RemoveTestVragen(vraag);

            for (int i = 0; i < vraagVoorTest.Count; i++)
            {
                foreach (List<string> vraag in vraagVoorTest)
                {
                    foreach (Label label in vraagLabels)
                    {
                        if (label.Text != vraag[2])
                            continue;

                        string order = label.Name!.Replace("label~", "");
                        if (Convert.ToInt32(order) == i + 1)
                            testVragen.AddTestVraag(test, vraag[1], vraag[2], order);
                    }
                }
            }
        }

        private void CheckTest()
        {
            errorsPanel.Controls.Clear();
            errorLabels.Clear();

            string testName = testCombo.Text;
            if (testName == "")
                return;

            string? gameName = ds.GetTestsClass().GetGameForTest(testName);
            if (gameName == null)
            {
                MessageBox.Show("Deze test is nog niet aan een spel gekoppeld. Koppel eerst een spel via Test aanmaken.");
                return;
            }

            List<string> errors = new();
            List<List<string>> vragen = ds.GetTestVragenClass().GetAllTestVragen().Where(v => v[0] == testName).ToList();

            List<string> activePlayers = ds.GetGameSpelersClass().GetSpelersForGame(gameName)
                .Where(speler => speler.Count > 1 && speler[1] == "1")
                .Select(speler => speler[0])
                .ToList();
            activePlayers.Sort();

            List<List<string>> antwoorden = ds.GetAntwoordenClass().GetAntwoorden();

            foreach (List<string> vraag in vragen)
            {
                List<List<string>> mogelijkeAntwoorden = new();
                List<string> playersForQuestionOptions = new();
                bool juistGevonden = false;

                foreach (List<string> antwoord in antwoorden)
                {
                    if (antwoord[0] != vraag[1] || antwoord[1] != vraag[2])
                        continue;

                    mogelijkeAntwoorden.Add(antwoord);

                    if (antwoord[3] == "1")
                    {
                        if (!juistGevonden)
                            juistGevonden = true;
                        else
                            errors.Add("Vraag \"" + antwoord[1] + "\" heeft meerdere antwoorden juist.");
                    }

                    if (antwoord[4].Length > 4)
                    {
                        List<string> pqo = System.Text.Json.JsonSerializer.Deserialize<List<string>>(antwoord[4]) ?? new();
                        playersForQuestionOptions.AddRange(pqo);
                    }
                }

                if (playersForQuestionOptions.Count > 0)
                {
                    bool foundAllPlayers = activePlayers.All(playersForQuestionOptions.Contains);
                    if (!foundAllPlayers)
                        errors.Add("Vraag \"" + vraag[2] + "\" heeft niet voor alle spelers een geselecteerd antwoord.");

                    if (playersForQuestionOptions.Count > playersForQuestionOptions.Distinct().Count())
                        errors.Add("Vraag \"" + vraag[2] + "\" heeft spelers dubbel gekoppeld aan een antwoord.");
                }

                if (mogelijkeAntwoorden.Count == 0)
                {
                    errors.Add("Vraag \"" + vraag[2] + "\" heeft geen antwoorden.");
                }
                else if (!juistGevonden)
                {
                    errors.Add("Vraag \"" + vraag[2] + "\" heeft geen juist antwoord geselecteerd.");
                }
            }

            int order = 0;
            foreach (string error in errors)
            {
                Label errorLabel = new()
                {
                    Location = new Point(12, order * 20),
                    Size = new Size(errorsPanel.ClientSize.Width - 24, 18),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Text = error
                };
                errorsPanel.Controls.Add(errorLabel);
                errorLabels.Add(errorLabel);
                order++;
            }

            if (errors.Count == 0)
            {
                Label okLabel = new() { Location = new Point(12, 0), AutoSize = true, Text = "Geen problemen gevonden." };
                errorsPanel.Controls.Add(okLabel);
            }
        }

        // --- Practice run entry point (018_TestUitvoeren, saveResults:false) ---
        private void StartTest(bool saveResults)
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
            Form20 form = new(shell, test, saveResults);
            shell.Hide();
            form.Show();
        }
    }
}
