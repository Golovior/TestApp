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
    public partial class Form4 : Form
    {
        readonly Form previous;
        readonly List<string>? questionsForCombobox;
        readonly List<Questions>? questions;
        readonly DataSetInfo? dsi;

        public Form4(Form prev)
        {
            this.dsi = Program.GetInfo();

            this.questions = dsi.GetGameById(dsi.currentGame).GetQuestions();

            InitializeComponent();

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

            previous = prev;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (questions == null)
                return;

            if (dsi == null)
                return;

            object selectedItem = comboBox1.SelectedItem;
            if (selectedItem == null)
                return;


            string option = textBox1.Text;
            Questions? selectedQuestion;

            foreach (Questions q in questions)
            {
                if (selectedItem.ToString() == q.GetQuestion())
                {
                    selectedQuestion = q;
                    if (selectedQuestion != null)
                    {
                        List<Options>? allOptions = this.dsi.GetOptions();
                        int counter = 0;
                        if (allOptions != null)
                            counter = allOptions.Count;

                        Options o = new();
                        o.SetValue(option);
                        o.SetQuestion(selectedQuestion);
                        o.SetId(counter);

                        o.WriteToFile();
                        this.dsi.AddToOptionList(o);

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
            previous.Show();
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
