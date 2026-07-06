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
    public partial class CombineSpelerAndAntwoordForm : BaseForm
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public CombineSpelerAndAntwoordForm(Form previous)
        {
            prev = previous;
            InitializeComponent();

            ds = Program.GetInfo();

            AddOpdrachtenToCombobox();
            AddPlayersToPanel();
        }
        private void AddOpdrachtenToCombobox()
        {
            List<string> opdrachten = this.ds.GetOpdrachtenClass().GetOpdrachten();

            foreach (string opdracht in opdrachten)
                comboBox1.Items.Add(opdracht);

        }

        private void AddPlayersToPanel()
        {
            List<string> spelers = this.ds.GetSpelersClass().GetSpelers();

            int order = 0;

            foreach (string speler in spelers)
            {
                order++;

                AddPlayers(speler, order, false);
            }
        }

        private void SelectedOpdracht(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();

            string selectedOpdracht = comboBox1.Text;

            if (selectedOpdracht == "")
                return;

            List<List<string>> allQuestions = ds.GetQuestionsClass().GetAllQuestions();

            foreach (List<string> question in allQuestions)
            {
                if (selectedOpdracht == question[0])
                {
                    comboBox2.Items.Add(question[1]);
                }
            }
        }

        private void SelectedQuestion(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();

            string selectedOpdracht = comboBox1.Text;
            string selectedQuestion = comboBox2.Text;

            if (selectedQuestion == "")
                return;

            List<List<string>> allAnswers = ds.GetAntwoordenClass().GetAntwoorden();

            foreach (List<string> antwoord in allAnswers)
            {
                if (selectedOpdracht == antwoord[0] && selectedQuestion == antwoord[1])
                {
                    comboBox3.Items.Add(antwoord[2]);
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string opdracht = comboBox1.Text;
            string vraag = comboBox2.Text;
            string antwoord = comboBox3.Text;

            if (opdracht == "" || vraag == "" || antwoord == "")
                return;

            List<string> players = new();

            foreach (CheckBox cb in checkboxes)
            {
                string orderId = "";
                if(cb.Checked)
                {
                    orderId = cb.Name[10..];
                }

                if (orderId == "")
                    continue;

                foreach(Label l in labels)
                {
                    if (l.Name != "label~" + orderId)
                        continue;

                    players.Add(l.Text);
                }
            }

            ds.GetAntwoordenClass().ConnectPlayersToAnswer(opdracht, vraag, antwoord, players);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }
    }
}
