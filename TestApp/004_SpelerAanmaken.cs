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

    public partial class Form8 : Form
    {
        readonly Form prev;
        readonly List<string>? gamesForCombobox;
        readonly List<Games>? games;
        readonly DataSetInfo? dsi;

        public Form8(Form previous)
        {
            this.dsi = Program.GetInfo();

            this.games = dsi.GetGames();

            InitializeComponent();

            if (games != null)
            {
                gamesForCombobox = new List<string>();
                foreach (Games game in games)
                {
                    string? g = game.GetName();
                    if (g == null)
                        continue;

                    gamesForCombobox.Add(g);
                }

                comboBox1.Items.AddRange(gamesForCombobox.ToArray());
            }

            prev = previous;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (games == null)
                return;

            if (dsi == null)
                return;

            object selectedItem = comboBox1.SelectedItem;
            if (selectedItem == null)
                return;

            string player = textBox1.Text;
            Games? selectedGame;

            foreach (Games g in games)
            {
                if (selectedItem.ToString() == g.GetName())
                {
                    selectedGame = g;
                    if (selectedGame != null)
                    {
                        List<Players>? allPlayers = this.dsi.GetPlayers();
                        int counter = 0;
                        if (allPlayers != null)
                            counter = allPlayers.Count;

                        Players p = new();
                        p.SetName(player);
                        p.SetGame(selectedGame);
                        p.SetStatus("Actief");
                        p.SetId(counter);

                        p.WriteToFile();
                        this.dsi.AddToPlayerList(p);

                        textBox1.Text = "";
                        textBox1.Focus();

                        return;
                    }
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
