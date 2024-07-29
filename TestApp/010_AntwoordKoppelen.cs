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
    public partial class Form10 : Form
    {
        readonly Form prev;
        readonly List<Players>? players;
        readonly List<string>? playerNames;
        readonly List<Questions>? questions;
        readonly List<string>? questionsForCombobox;

        List<Options>? options;
        List<string>? optionsForCombobox = null;

        readonly DataSetInfo dsi;
        readonly List<CheckBox> checkboxes;

        public Form10(Form previous)
        {
            prev = previous;
            dsi = Program.GetInfo();

            InitializeComponent();

            checkboxes = new()
            {
                checkBox1,
                checkBox2,
                checkBox3,
                checkBox4,
                checkBox5,
                checkBox6,
                checkBox7,
                checkBox8,
                checkBox9,
                checkBox10,
                checkBox11,
                checkBox12,
                checkBox13,
                checkBox14,
                checkBox15
            };

            players = dsi.GetGameById(dsi.currentGame).GetPlayers();

            if (players == null)
                return;

            playerNames = new List<string>(); 

            foreach (Players player in players)
            {
                string? playerName = player.GetName();
                if (playerName == null)
                    continue;

                playerNames.Add(playerName);
            }

            int index = 0;

            foreach(string playerName in playerNames)
            {
                checkboxes[index].Text = playerName;
                checkboxes[index].Visible = true;

                index++;
            }

            this.questions = dsi.GetGameById(dsi.currentGame).GetQuestions();

            if (questions != null)
            {
                questionsForCombobox = new List<string>();
                foreach (Questions question in questions)
                {
                    string? q = question.GetQuestion();
                    if (q == null)
                        continue;

                    questionsForCombobox.Add(q);
                }

                comboBox1.Items.AddRange(questionsForCombobox.ToArray());
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (players == null || options == null)
                return;

            List<Players> playersForOption = new();
            foreach (CheckBox checkbox in checkboxes)
            {
                if (checkbox.Enabled)
                {
                    if (checkbox.Checked)
                    {
                        foreach (Players p in players)
                        {
                            if (checkbox.Text == p.GetName())
                            {
                                playersForOption.Add(p);
                                break;
                            }
                        }
                    }
                }
            }

            Options selectedOption = new();

            foreach (Options option in options)
            {
                if (option.GetValue() == comboBox2.Text)
                {
                    selectedOption = option;
                }
            }

            if (selectedOption.GetValue() == null)
                return;

            selectedOption.ClearPlayers();

            foreach(Players p in playersForOption)
            {
                selectedOption.AddPlayer(p);
            }

            selectedOption.UpdateInFile();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void ChangeQuestion(object sender, EventArgs e)
        {
            if (questions == null)
                return;

            string questionValue = comboBox1.Text;
            Questions? selectedQuestion = new();

            foreach (Questions q in this.questions)
            {
                if (q.GetQuestion() == questionValue)
                {
                    selectedQuestion = q;
                    break;
                }
            }

            options = selectedQuestion.GetOptions();

            if (options != null)
            {
                optionsForCombobox = new();
                foreach (Options optionForQuestion in options)
                {
                    string? o = optionForQuestion.GetValue();
                    if (o == null)
                        continue;

                    optionsForCombobox.Add(o);
                }

                comboBox2.Items.Clear();
                comboBox2.Items.AddRange(optionsForCombobox.ToArray());
            }
        }

        private void ChangeAnswer(object sender, EventArgs e)
        {
            if (options == null)
                return;

            List<Players> playersForOption = new();
            List<Players> playersNotForOption = new();

            foreach (Options option in options)
            {
                if(option.GetValue() == comboBox2.Text)
                {
                    List<Players>? pfo = option.GetPlayers();
                    if (pfo != null)
                        playersForOption = pfo;
                }
                else
                {
                    List<Players>? notForOption = option.GetPlayers();
                    if (notForOption == null)
                        continue;

                    foreach(Players player in notForOption)
                    {
                        playersNotForOption.Add(player);
                    }
                }
            }

            foreach (CheckBox c in checkboxes)
            {
                c.Checked = false;
                c.Enabled = true;
            }

            if(playersNotForOption.Count > 0)
            {
                foreach (Players player in playersNotForOption)
                {
                    foreach (CheckBox c in checkboxes)
                    {
                        if (c.Text == player.GetName())
                        {
                            c.Checked = true;
                            c.Enabled = false;
                            break;
                        }
                    }
                }
            }

            if (playersForOption.Count > 0)
            {
                foreach (Players player in playersForOption)
                {
                    foreach (CheckBox c in checkboxes)
                    {
                        if (c.Text == player.GetName())
                        {
                            c.Checked = true;
                            c.Enabled = true;
                            break;
                        }
                    }
                }
            }
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
