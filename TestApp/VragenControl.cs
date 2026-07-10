using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestApp
{
    public class VragenControl : UserControl
    {
        private readonly DataSetClass ds;

        private ComboBox opdrachtCombo = null!;
        private TextBox newVraagBox = null!;
        private CheckBox alfabetischBox = null!;
        private ComboBox vraagCombo = null!;

        private Panel antwoordenPanel = null!;
        private TextBox newAntwoordBox = null!;
        private readonly List<CheckBox> antwoordCorrectBoxes = new();
        private readonly List<Label> antwoordLabels = new();

        private Panel spelersPanel = null!;
        private ComboBox antwoordSelectCombo = null!;
        private readonly List<CheckBox> spelerCheckboxes = new();
        private readonly List<Label> spelerLabels = new();

        public VragenControl(ShellForm shell)
        {
            ds = Program.GetInfo();

            TableLayoutPanel root = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            root.Controls.Add(BuildOpdrachtRow(), 0, 0);
            root.Controls.Add(BuildVraagRow(), 0, 1);
            root.Controls.Add(BuildAntwoordenAndSpelersRow(), 0, 2);

            Controls.Add(root);

            AddPlayersToPanel();
        }

        // --- Opdracht + nieuwe vraag (008_VraagToevoegen, question part) ---
        private Panel BuildOpdrachtRow()
        {
            Panel panel = new() { Height = 40, Dock = DockStyle.Fill };

            Label opdrachtLabel = new() { Text = "Opdracht", Location = new Point(12, 9), AutoSize = true };
            opdrachtCombo = new ComboBox { Location = new Point(90, 6), Size = new Size(220, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (string opdracht in ds.GetOpdrachtenClass().GetOpdrachten())
                opdrachtCombo.Items.Add(opdracht);
            opdrachtCombo.SelectedIndexChanged += (s, e) => OnOpdrachtChanged();

            Label newVraagLabel = new() { Text = "Nieuwe vraag", Location = new Point(330, 9), AutoSize = true };
            newVraagBox = new TextBox { Location = new Point(420, 6), Size = new Size(240, 23) };
            alfabetischBox = new CheckBox { Text = "Alfabetisch", Location = new Point(670, 8), AutoSize = true };
            Button addVraagButton = new() { Text = "Vraag toevoegen", Location = new Point(770, 5), Size = new Size(140, 23) };
            addVraagButton.Click += (s, e) => AddVraag();

            panel.Controls.Add(opdrachtLabel);
            panel.Controls.Add(opdrachtCombo);
            panel.Controls.Add(newVraagLabel);
            panel.Controls.Add(newVraagBox);
            panel.Controls.Add(alfabetischBox);
            panel.Controls.Add(addVraagButton);
            return panel;
        }

        private Panel BuildVraagRow()
        {
            Panel panel = new() { Height = 36, Dock = DockStyle.Fill };
            Label vraagLabel = new() { Text = "Vraag", Location = new Point(12, 8), AutoSize = true };
            vraagCombo = new ComboBox { Location = new Point(90, 5), Size = new Size(400, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            vraagCombo.SelectedIndexChanged += (s, e) => OnVraagChanged();

            panel.Controls.Add(vraagLabel);
            panel.Controls.Add(vraagCombo);
            return panel;
        }

        private TableLayoutPanel BuildAntwoordenAndSpelersRow()
        {
            TableLayoutPanel split = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            split.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

            split.Controls.Add(BuildAntwoordenGroup(), 0, 0);
            split.Controls.Add(BuildSpelersGroup(), 1, 0);
            return split;
        }

        // --- Antwoorden (009_AntwoordToevoegen + 010_JuistAntwoordSelecteren) ---
        private TableLayoutPanel BuildAntwoordenGroup()
        {
            TableLayoutPanel panel = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Panel addRow = new() { Height = 32, Dock = DockStyle.Fill };
            Label caption = new() { Text = "Antwoorden", Location = new Point(4, 8), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            newAntwoordBox = new TextBox { Location = new Point(100, 4), Size = new Size(220, 23) };
            Button addAntwoordButton = new() { Text = "Antwoord toevoegen", Location = new Point(330, 4), Size = new Size(150, 23) };
            addAntwoordButton.Click += (s, e) => AddAntwoord();
            addRow.Controls.Add(caption);
            addRow.Controls.Add(newAntwoordBox);
            addRow.Controls.Add(addAntwoordButton);

            antwoordenPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BorderStyle = BorderStyle.FixedSingle };

            Panel saveRow = new() { Height = 32, Dock = DockStyle.Fill };
            Button saveCorrectButton = new() { Text = "Opslaan (juiste antwoord)", Location = new Point(4, 4), Size = new Size(300, 23) };
            saveCorrectButton.Click += (s, e) => SaveCorrectAntwoorden();
            saveRow.Controls.Add(saveCorrectButton);

            panel.Controls.Add(addRow, 0, 0);
            panel.Controls.Add(antwoordenPanel, 0, 1);
            panel.Controls.Add(saveRow, 0, 2);
            return panel;
        }

        // --- Spelers koppelen (014_CombineSpelerAndAntwoord) ---
        private TableLayoutPanel BuildSpelersGroup()
        {
            TableLayoutPanel panel = new()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Panel headerRow = new() { Height = 28, Dock = DockStyle.Fill };
            Label caption = new() { Text = "Antwoord", Location = new Point(4, 6), AutoSize = true };
            antwoordSelectCombo = new ComboBox { Location = new Point(70, 3), Size = new Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            headerRow.Controls.Add(caption);
            headerRow.Controls.Add(antwoordSelectCombo);

            spelersPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BorderStyle = BorderStyle.FixedSingle };

            Panel saveRow = new() { Height = 32, Dock = DockStyle.Fill };
            Button saveButton = new() { Text = "Opslaan (koppelen)", Location = new Point(4, 4), Size = new Size(200, 23) };
            saveButton.Click += (s, e) => SaveSpelerKoppeling();
            saveRow.Controls.Add(saveButton);

            panel.Controls.Add(headerRow, 0, 0);
            panel.Controls.Add(spelersPanel, 0, 1);
            panel.Controls.Add(saveRow, 0, 2);
            return panel;
        }

        private void OnOpdrachtChanged()
        {
            vraagCombo.Items.Clear();
            OnVraagChanged();

            string opdracht = opdrachtCombo.Text;
            if (opdracht == "")
                return;

            foreach (List<string> vraag in ds.GetQuestionsClass().GetAllQuestions())
            {
                if (vraag[0] == opdracht)
                    vraagCombo.Items.Add(vraag[1]);
            }
        }

        private void AddVraag()
        {
            string opdracht = opdrachtCombo.Text;
            string question = newVraagBox.Text;
            if (opdracht == "" || question == "")
                return;

            Questions questions = ds.GetQuestionsClass();
            if (questions.QuestionAlreadyExists(opdracht, question))
                return;

            questions.AddQuestion(opdracht, question, alfabetischBox.Checked ? "1" : "0");

            newVraagBox.Text = "";
            alfabetischBox.Checked = false;
            newVraagBox.Focus();

            OnOpdrachtChanged();
            vraagCombo.SelectedItem = question;
        }

        private void OnVraagChanged()
        {
            antwoordenPanel.Controls.Clear();
            antwoordCorrectBoxes.Clear();
            antwoordLabels.Clear();
            antwoordSelectCombo.Items.Clear();

            string opdracht = opdrachtCombo.Text;
            string vraag = vraagCombo.Text;
            if (opdracht == "" || vraag == "")
                return;

            int order = 0;
            foreach (List<string> answer in ds.GetAntwoordenClass().GetAntwoorden())
            {
                if (answer[0] != opdracht || answer[1] != vraag)
                    continue;

                MakeAnswerRow(answer, order, answer[3] == "1");
                antwoordSelectCombo.Items.Add(answer[2]);
                order++;
            }
        }

        private void MakeAnswerRow(List<string> antwoord, int order, bool correct)
        {
            int height = 27 * order;

            CheckBox checkbox = new()
            {
                Location = new Point(4, height),
                Size = new Size(20, 20),
                Name = "correct~" + order,
                Checked = correct
            };

            Label label = new()
            {
                Location = new Point(28, height + 3),
                Size = new Size(antwoordenPanel.ClientSize.Width - 40, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "label~" + order,
                Text = antwoord[2]
            };

            antwoordenPanel.Controls.Add(checkbox);
            antwoordenPanel.Controls.Add(label);

            antwoordCorrectBoxes.Add(checkbox);
            antwoordLabels.Add(label);
        }

        private void AddAntwoord()
        {
            string opdracht = opdrachtCombo.Text;
            string vraag = vraagCombo.Text;
            string antwoord = newAntwoordBox.Text;

            if (opdracht == "" || vraag == "" || antwoord == "")
                return;

            Antwoorden antwoordClass = ds.GetAntwoordenClass();
            if (antwoordClass.AntwoordAlreadyExists(opdracht, vraag, antwoord))
                return;

            antwoordClass.AddAntwoord(opdracht, vraag, antwoord);

            newAntwoordBox.Text = "";
            newAntwoordBox.Focus();
            OnVraagChanged();
        }

        private void SaveCorrectAntwoorden()
        {
            string opdracht = opdrachtCombo.Text;
            string vraag = vraagCombo.Text;
            if (opdracht == "" || vraag == "")
                return;

            foreach (CheckBox cb in antwoordCorrectBoxes)
            {
                if (!cb.Checked)
                    continue;

                string orderId = cb.Name!["correct~".Length..];
                Label? label = antwoordLabels.FirstOrDefault(l => l.Name == "label~" + orderId);
                if (label != null)
                    ds.GetAntwoordenClass().SetAsCorrectAntwoord(opdracht, vraag, label.Text);
            }
        }

        // Populated once with every speler, independent of Opdracht/Vraag/Antwoord
        // selection - matches 014_CombineSpelerAndAntwoord's original behavior exactly
        // (it does not restore previously-connected players when re-selecting an answer).
        private void AddPlayersToPanel()
        {
            int order = 0;
            foreach (string speler in ds.GetSpelersClass().GetSpelers())
            {
                order++;
                MakeSpelerRow(speler, order);
            }
        }

        private void MakeSpelerRow(string speler, int order)
        {
            int height = 27 * order;

            CheckBox checkbox = new()
            {
                Location = new Point(4, height),
                Size = new Size(20, 20),
                Name = "connected~" + order
            };

            Label label = new()
            {
                Location = new Point(28, height + 3),
                Size = new Size(spelersPanel.ClientSize.Width - 40, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "label~" + order,
                Text = speler
            };

            spelersPanel.Controls.Add(checkbox);
            spelersPanel.Controls.Add(label);

            spelerCheckboxes.Add(checkbox);
            spelerLabels.Add(label);
        }

        private void SaveSpelerKoppeling()
        {
            string opdracht = opdrachtCombo.Text;
            string vraag = vraagCombo.Text;
            string antwoord = antwoordSelectCombo.Text;

            if (opdracht == "" || vraag == "" || antwoord == "")
                return;

            List<string> players = new();
            foreach (CheckBox cb in spelerCheckboxes)
            {
                if (!cb.Checked)
                    continue;

                string orderId = cb.Name!["connected~".Length..];
                Label? label = spelerLabels.FirstOrDefault(l => l.Name == "label~" + orderId);
                if (label != null)
                    players.Add(label.Text);
            }

            ds.GetAntwoordenClass().ConnectPlayersToAnswer(opdracht, vraag, antwoord, players);
        }
    }
}
