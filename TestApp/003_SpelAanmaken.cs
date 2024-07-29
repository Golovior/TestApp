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
    public partial class Form7 : Form
    {
        readonly Form prev;
        readonly DataSetInfo dsi;

        public Form7(Form previous)
        {
            InitializeComponent();
            prev = previous;
            dsi = Program.GetInfo();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            List<Games>? allGames = Program.GetInfo().GetGames();
            Int32 nextId = 1;

            if (allGames != null)
                nextId = allGames.Count + 1;

            Games game = new();
            game.SetName(textBox1.Text);
            game.SetId(nextId);

            game.WriteToFile();

            dsi.AddToGameList(game);

            textBox1.Text = "";
            textBox1.Focus();
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
