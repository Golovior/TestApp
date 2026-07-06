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
    public partial class PlayerStatus : BaseForm
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public PlayerStatus(Form previous)
        {
            this.prev = previous;

            InitializeComponent();

            this.ds = Program.GetInfo();

            AddGamesToCombobox();
        }

        private void AddGamesToCombobox()
        {
            foreach (string game in ds.GetGamesClass().GetAllGames())
                comboBox1.Items.Add(game);
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshForGame(comboBox1.Text);
        }

        private void RefreshForGame(string game)
        {
            panel1.Controls.Clear();
            playersList.Clear();
            activeList.Clear();
            inactiveList.Clear();
            panels.Clear();

            comboBox2.Items.Clear();
            comboBox2.Text = "";

            if (game == "")
                return;

            List<string> alreadyAssigned = ds.GetGameSpelersClass().GetSpelersForGame(game)
                .Select(speler => speler[0])
                .ToList();

            foreach (string speler in ds.GetSpelersClass().GetSpelers())
            {
                if (!alreadyAssigned.Contains(speler))
                    comboBox2.Items.Add(speler);
            }

            AddPlayersToPanel(game);
        }

        public void AddPlayersToPanel(string game)
        {
            GameSpelers gameSpelers = ds.GetGameSpelersClass();

            int order = 0;

            foreach (List<string> speler in gameSpelers.GetSpelersForGame(game))
            {
                bool active = speler.Count > 1 && speler[1] == "1";

                MakePlayerRow(speler[0], order, active);
                order++;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            string game = comboBox1.Text;
            string speler = comboBox2.Text;

            if (game == "" || speler == "")
                return;

            ds.GetGameSpelersClass().AssignSpelerToGame(game, speler);

            RefreshForGame(game);
        }

        public void Button1_Click(object sender, EventArgs e)
        {
            string game = comboBox1.Text;

            if (game == "")
                return;

            GameSpelers gameSpelers = ds.GetGameSpelersClass();

            foreach(Label l in playersList)
            {
                string orderId = l.Name[7..];

                foreach (RadioButton rb in activeList)
                {
                    if (rb.Name != "active~" + orderId)
                        continue;

                    if (rb.Checked)
                        gameSpelers.SetStatus(game, l.Text, "1");
                }

                foreach (RadioButton rb in inactiveList)
                {
                    if (rb.Name != "inactive~" + orderId)
                        continue;

                    if (rb.Checked)
                        gameSpelers.SetStatus(game, l.Text, "0");
                }
            }
        }

        public void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }
    }
}
