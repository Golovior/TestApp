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
    public partial class Form16 : Form
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public Form16(Form previous)
        {
            prev = previous;

            InitializeComponent();

            ds = Program.GetInfo();
            
            PutTestsInCombobox();

        }

        private void PutTestsInCombobox()
        {
            Tests tests = ds.GetTestsClass();

            List<string> allTests = tests.GetAllTests();

            foreach(string test in allTests)
            {
                comboBox1.Items.Add(test);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void ButtonUp_click(object sender, EventArgs e)
        {
            Button clicked = (Button)sender;
            
            string name = clicked.Name.Replace("Up~","");
            string prevQuestionId = Convert.ToString(Convert.ToInt32(name) - 1);

            if (prevQuestionId == "0")
                return;

            string questionValue = "";
            string questionReplaceValue = "";


            foreach (Label l in labels)
            {
                if(l.Name == "label~" + name)
                {
                    questionValue = l.Text;
                }

                if (l.Name == "label~" + prevQuestionId)
                {
                    questionReplaceValue = l.Text;
                }
            }

            if (questionValue == "" || questionReplaceValue == "")
                return;

            foreach (Label l in labels)
            {
                if (l.Name == "label~" + name)
                {
                    l.Text = questionReplaceValue;
                }
                if (l.Name == "label~" + prevQuestionId)
                {
                    l.Text = questionValue;
                }
            }
        }

        private void ButtonDown_click(object sender, EventArgs e)
        {

            Button clicked = (Button)sender;

            string name = clicked.Name.Replace("Down~", "");
            string prevQuestionId = Convert.ToString(Convert.ToInt32(name) + 1);

            string questionValue = "";
            string questionReplaceValue = "";

            foreach (Label l in labels)
            {
                if (l.Name == "label~" + name)
                {
                    questionValue = l.Text;
                }

                if (l.Name == "label~" + prevQuestionId)
                {
                    questionReplaceValue = l.Text;
                }
            }

            if (questionValue == "" || questionReplaceValue == "")
                return;

            foreach (Label l in labels)
            {
                if (l.Name == "label~" + name)
                {
                    l.Text = questionReplaceValue;
                }
                if (l.Name == "label~" + prevQuestionId)
                {
                    l.Text = questionValue;
                }
            }
        }

        private void ButtonRemove_click(object sender, EventArgs e)
        {
            Button clicked = (Button)sender;

            string name = clicked.Name.Replace("Remove~", "");
            string questionValue = "";

            foreach (Label l in labels)
            {
                if (l.Name == "label~" + name)
                {
                    questionValue = l.Text;
                    break;
                }
            }

            if (questionValue == "")
                return;

        }

        private void ChangeTest(object sender, EventArgs e)
        {
            TestVragen testVragen = ds.GetTestVragenClass();

            List<List<string>> alleVragen = testVragen.GetAllTestVragen();

            string test = comboBox1.Text;
            List<List<string>> vraagVoorTest = new();

            foreach (List<string> vraag in alleVragen) {
                if (vraag[0] == test)
                    vraagVoorTest.Add(vraag);
            }

            List<List<string>> orderedVraagVoorTest = new();

            for (int i = 0; i <= alleVragen.Count; i++)
            {
                foreach (List<string> vraag in vraagVoorTest)
                {
                    if (i == Convert.ToInt32(vraag[3]))
                        orderedVraagVoorTest.Add(vraag);
                }
            }

            foreach(List<string> vraag in orderedVraagVoorTest)
            {
                this.MakeQuestionRow(vraag);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            TestVragen testVragen = ds.GetTestVragenClass();

            List<List<string>> alleVragen = testVragen.GetAllTestVragen();

            string test = comboBox1.Text;

            List<List<string>> vraagVoorTest = new();

            foreach (List<string> vraag in alleVragen)
            {
                if (vraag[0] == test)
                {
                    vraagVoorTest.Add(vraag);
                }
            }

            for (int i = 0; i < vraagVoorTest.Count; i++)
            {
                testVragen.RemoveTestVragen(vraagVoorTest[i]);
            }
                
            for (int i = 0; i < vraagVoorTest.Count; i++)
            {
                foreach (List<string> vraag in vraagVoorTest)
                {
                    foreach (Label label in labels)
                    {
                        if (label.Text == vraag[2])
                        {
                            string order = label.Name.Replace("label~", "");
                            if(Convert.ToInt32(order) == i + 1)
                                testVragen.AddTestVraag(test, vraag[1], vraag[2], order);
                        }
                    }
                }
            }
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            prev.Show();
        }
    }
}
