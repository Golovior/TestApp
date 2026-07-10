using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestApp
{
    public class SpelSpelersControl : UserControl
    {
        private readonly DataSetClass ds;

        private readonly List<Label> playersList = new();
        private readonly List<RadioButton> activeList = new();
        private readonly List<RadioButton> inactiveList = new();
        private readonly List<Panel> playerRowPanels = new();

        private ComboBox gameCombo = null!;
        private ComboBox unassignedPlayerCombo = null!;
        private Panel rosterPanel = null!;

        public SpelSpelersControl(ShellForm shell)
        {
            ds = Program.GetInfo();

            TabControl tabs = new() { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildSpellenTab());
            tabs.TabPages.Add(BuildOpdrachtenTab());
            tabs.TabPages.Add(BuildSpelersTab());
            tabs.TabPages.Add(BuildInstellingenTab());

            Controls.Add(tabs);
        }

        // --- Spellen (003_SpelAanmaken) ---
        private TabPage BuildSpellenTab()
        {
            TabPage page = new("Spellen");

            Label label = new() { Text = "Naam", Location = new Point(12, 15), AutoSize = true };
            TextBox nameBox = new() { Location = new Point(80, 12), Size = new Size(240, 23) };
            Button save = new() { Text = "Toevoegen", Location = new Point(12, 41), Size = new Size(308, 23) };

            save.Click += (s, e) =>
            {
                Games gameClass = ds.GetGamesClass();
                string gameName = nameBox.Text;

                if (gameName == "" || gameClass.GameAlreadyExists(gameName))
                    return;

                gameClass.AddGame(gameName);
                nameBox.Text = "";
                nameBox.Focus();
            };

            page.Controls.Add(label);
            page.Controls.Add(nameBox);
            page.Controls.Add(save);
            return page;
        }

        // --- Opdrachten (004_OpdrachtAanmaken) ---
        private TabPage BuildOpdrachtenTab()
        {
            TabPage page = new("Opdrachten");

            Label label = new() { Text = "Naam", Location = new Point(12, 15), AutoSize = true };
            TextBox nameBox = new() { Location = new Point(80, 12), Size = new Size(240, 23) };
            Button save = new() { Text = "Toevoegen", Location = new Point(12, 41), Size = new Size(308, 23) };

            save.Click += (s, e) =>
            {
                Opdrachten opdrachtenClass = ds.GetOpdrachtenClass();
                string opdrachtName = nameBox.Text;

                if (opdrachtName == "" || opdrachtenClass.OpdrachtAlreadyExists(opdrachtName))
                    return;

                opdrachtenClass.AddOpdracht(opdrachtName);
                nameBox.Text = "";
                nameBox.Focus();
            };

            page.Controls.Add(label);
            page.Controls.Add(nameBox);
            page.Controls.Add(save);
            return page;
        }

        // --- Instellingen (006_SettingWijziging) ---
        private TabPage BuildInstellingenTab()
        {
            TabPage page = new("Instellingen");

            Label settingLabel = new() { Text = "Setting", Location = new Point(12, 15), AutoSize = true };
            ComboBox settingCombo = new() { Location = new Point(90, 12), Size = new Size(230, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (string key in ds.GetSettingsClass().GetKeys())
                settingCombo.Items.Add(key);

            Label valueLabel = new() { Text = "Nieuwe value", Location = new Point(12, 47), AutoSize = true };
            TextBox valueBox = new() { Location = new Point(90, 44), Size = new Size(230, 23) };

            Button save = new() { Text = "Opslaan", Location = new Point(12, 77), Size = new Size(308, 23) };
            save.Click += (s, e) =>
            {
                if (settingCombo.Text == "" || valueBox.Text == "")
                    return;

                ds.GetSettingsClass().UpdateSetting(settingCombo.Text, valueBox.Text);
                valueBox.Text = "";
                valueBox.Focus();
            };

            Label syncStatusLabel = new() { Text = "", Location = new Point(12, 145), AutoSize = true };
            Button syncButton = new() { Text = "Informatie syncen", Location = new Point(12, 116), Size = new Size(308, 23) };
            syncButton.Click += async (s, e) =>
            {
                syncButton.Enabled = false;
                syncStatusLabel.Text = "Bezig met syncen...";
                try
                {
                    await ds.GetApiClass().SaveData();
                    syncStatusLabel.Text = "Sync gelukt.";
                }
                catch (Exception ex)
                {
                    syncStatusLabel.Text = "Sync mislukt: " + ex.Message;
                }
                finally
                {
                    syncButton.Enabled = true;
                }
            };

            page.Controls.Add(settingLabel);
            page.Controls.Add(settingCombo);
            page.Controls.Add(valueLabel);
            page.Controls.Add(valueBox);
            page.Controls.Add(save);
            page.Controls.Add(syncButton);
            page.Controls.Add(syncStatusLabel);
            return page;
        }

        // --- Spelers (005_AddPlayers + 022_PlayerStatus) ---
        private TabPage BuildSpelersTab()
        {
            TabPage page = new("Spelers");

            Panel topPanel = new() { Dock = DockStyle.Top, Height = 150 };

            Label newPlayerLabel = new() { Text = "Nieuwe speler", Location = new Point(12, 15), AutoSize = true };
            TextBox newPlayerBox = new() { Location = new Point(110, 12), Size = new Size(240, 23) };
            Button addPlayerButton = new() { Text = "Toevoegen", Location = new Point(360, 12), Size = new Size(120, 23) };
            addPlayerButton.Click += (s, e) =>
            {
                string playerName = newPlayerBox.Text;
                if (playerName == "")
                    return;

                Spelers spelers = ds.GetSpelersClass();
                if (spelers.SpelerAlreadyExists(playerName))
                    return;

                spelers.AddSpeler(playerName);
                newPlayerBox.Text = "";
                newPlayerBox.Focus();
                RefreshForGame(gameCombo.Text);
            };

            Label gameLabel = new() { Text = "Spel", Location = new Point(12, 50), AutoSize = true };
            gameCombo = new ComboBox { Location = new Point(110, 47), Size = new Size(240, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (string game in ds.GetGamesClass().GetAllGames())
                gameCombo.Items.Add(game);
            gameCombo.SelectedIndexChanged += (s, e) => RefreshForGame(gameCombo.Text);

            unassignedPlayerCombo = new ComboBox { Location = new Point(110, 82), Size = new Size(240, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            Button assignButton = new() { Text = "Toevoegen aan spel", Location = new Point(360, 82), Size = new Size(120, 23) };
            assignButton.Click += (s, e) =>
            {
                string game = gameCombo.Text;
                string speler = unassignedPlayerCombo.Text;
                if (game == "" || speler == "")
                    return;

                ds.GetGameSpelersClass().AssignSpelerToGame(game, speler);
                RefreshForGame(game);
            };

            Button saveStatusButton = new() { Text = "Status opslaan", Location = new Point(12, 117), Size = new Size(468, 23) };
            saveStatusButton.Click += (s, e) => SaveStatuses();

            topPanel.Controls.Add(newPlayerLabel);
            topPanel.Controls.Add(newPlayerBox);
            topPanel.Controls.Add(addPlayerButton);
            topPanel.Controls.Add(gameLabel);
            topPanel.Controls.Add(gameCombo);
            topPanel.Controls.Add(unassignedPlayerCombo);
            topPanel.Controls.Add(assignButton);
            topPanel.Controls.Add(saveStatusButton);

            rosterPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            page.Controls.Add(rosterPanel);
            page.Controls.Add(topPanel);
            return page;
        }

        private void RefreshForGame(string game)
        {
            rosterPanel.Controls.Clear();
            playersList.Clear();
            activeList.Clear();
            inactiveList.Clear();
            playerRowPanels.Clear();

            unassignedPlayerCombo.Items.Clear();
            unassignedPlayerCombo.Text = "";

            if (game == "")
                return;

            List<string> alreadyAssigned = ds.GetGameSpelersClass().GetSpelersForGame(game)
                .Select(speler => speler[0])
                .ToList();

            foreach (string speler in ds.GetSpelersClass().GetSpelers())
            {
                if (!alreadyAssigned.Contains(speler))
                    unassignedPlayerCombo.Items.Add(speler);
            }

            int order = 0;
            foreach (List<string> speler in ds.GetGameSpelersClass().GetSpelersForGame(game))
            {
                bool active = speler.Count > 1 && speler[1] == "1";
                MakePlayerRow(speler[0], order, active);
                order++;
            }
        }

        private void MakePlayerRow(string player, int order, bool activePlayer)
        {
            Panel rowPanel = new()
            {
                Location = new Point(4, order * 30),
                Size = new Size(rosterPanel.ClientSize.Width - 24, 27),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "playerRow~" + order
            };

            Label nameLabel = new()
            {
                Location = new Point(0, 4),
                Size = new Size(rowPanel.Width - 180, 18),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Text = player,
                Name = "player~" + order
            };

            Label activeCaption = new() { Location = new Point(rowPanel.Width - 170, 4), AutoSize = true, Text = "Actief", Anchor = AnchorStyles.Top | AnchorStyles.Right };
            RadioButton active = new()
            {
                Location = new Point(rowPanel.Width - 120, 0),
                Size = new Size(22, 22),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Name = "active~" + order,
                Checked = activePlayer
            };

            Label inactiveCaption = new() { Location = new Point(rowPanel.Width - 90, 4), AutoSize = true, Text = "Afgevallen", Anchor = AnchorStyles.Top | AnchorStyles.Right };
            RadioButton inactive = new()
            {
                Location = new Point(rowPanel.Width - 10, 0),
                Size = new Size(22, 22),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Name = "inactive~" + order,
                Checked = !activePlayer
            };

            rowPanel.Controls.Add(nameLabel);
            rowPanel.Controls.Add(activeCaption);
            rowPanel.Controls.Add(active);
            rowPanel.Controls.Add(inactiveCaption);
            rowPanel.Controls.Add(inactive);

            playersList.Add(nameLabel);
            activeList.Add(active);
            inactiveList.Add(inactive);
            playerRowPanels.Add(rowPanel);

            rosterPanel.Controls.Add(rowPanel);
        }

        private void SaveStatuses()
        {
            string game = gameCombo.Text;
            if (game == "")
                return;

            GameSpelers gameSpelers = ds.GetGameSpelersClass();

            foreach (Label l in playersList)
            {
                string orderId = l.Name["player~".Length..];

                RadioButton? active = activeList.FirstOrDefault(rb => rb.Name == "active~" + orderId);
                if (active != null && active.Checked)
                    gameSpelers.SetStatus(game, l.Text, "1");

                RadioButton? inactive = inactiveList.FirstOrDefault(rb => rb.Name == "inactive~" + orderId);
                if (inactive != null && inactive.Checked)
                    gameSpelers.SetStatus(game, l.Text, "0");
            }
        }
    }
}
