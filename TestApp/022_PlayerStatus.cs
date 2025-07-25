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
    public partial class PlayerStatus : Form
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public PlayerStatus(Form previous)
        {
            this.prev = previous;

            InitializeComponent();

            this.ds = Program.GetInfo();

            AddPlayersToPanel();
        }

        public void AddPlayersToPanel()
        {
            Spelers spelers = ds.GetSpelersClass();

            int order = 0;

            foreach(List<string> speler in spelers.GetSpelers())
            {
                bool active = false;
                if(speler[1] == "1")
                    active = true;

                MakePlayerRow(speler[0], order, active);
                order++;
            }
        }

        public void Button1_Click(object sender, EventArgs e)
        {
            Spelers spelers = ds.GetSpelersClass();

            foreach(Label l in playersList)
            {
                string orderId = l.Name[7..];

                foreach (RadioButton rb in activeList)
                {
                    if (rb.Name != "active~" + orderId)
                        continue;

                    if (rb.Checked)
                        spelers.SavePlayerStatus(l.Text, "1");
                }

                foreach (RadioButton rb in inactiveList)
                {
                    if (rb.Name != "inactive~" + orderId)
                        continue;

                    if (rb.Checked)
                        spelers.SavePlayerStatus(l.Text, "0");
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
