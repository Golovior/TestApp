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
    public partial class Form12 : Form
    {
        readonly Form prev;
        readonly List<Players>? players;
        readonly DataSetInfo dsi;

        public Form12(Form previous)
        {
            dsi = Program.GetInfo();

            players = dsi.GetGameById(dsi.currentGame).GetPlayers();

            InitializeComponent();
            prev = previous;

            if (players == null)
                return;

            List<string> playersInList = new();

            foreach(Players p in players)
            {
                string? name = p.GetName();
                if (name == null)
                    continue;

                playersInList.Add(name);
            }

            comboBox1.Items.AddRange(playersInList.ToArray());
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (players == null)
                return;

            string status = "";
            if (radioButton1.Checked)
                status = "Actief";

            if (radioButton2.Checked)
                status = "Afgevallen";

            if (status == "")
                return;

            string selectedName = comboBox1.Text;

            foreach (Players p in players)
            {
                if (p.GetName() == selectedName)
                {
                    p.SetStatus(status);
                    p.UpdateInFile();
                    break;
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void UpdateSelectedPlayer(object sender, EventArgs e)
        {
            if (players == null)
                return;

            string selectedName = comboBox1.Text;
            Players selectedPlayer = new();

            foreach(Players p in players)
            {
                if (p.GetName() == selectedName)
                {
                    selectedPlayer = p;
                    break;
                }
            }

            if (selectedPlayer.GetName() == null)
                return;

            if (selectedPlayer.GetStatus() == "Actief")
            {
                radioButton1.Checked = true;
                radioButton2.Checked = false;
            } 
            else
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true;
            }
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
