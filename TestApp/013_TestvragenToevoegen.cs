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
    public partial class Form14 : Form
    {
        readonly Form prev;
        readonly DataSetClass ds;

        public Form14(Form previous)
        {
            InitializeComponent();
            prev = previous;

            ds = Program.GetInfo();

            AddTestsToCombobox();
            AddOpdrachtenToCombobox();

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.Dispose();
            prev.Show();
        }

        private void AddTestsToCombobox()
        {
            List<string> tests = this.ds.GetTestsClass().GetAllTests();

            foreach (string test in tests)
                comboBox1.Items.Add(test);

        }

        private void AddOpdrachtenToCombobox()
        {
            List<string> opdrachten = this.ds.GetOpdrachtenClass().GetOpdrachten();

            foreach (string opdracht in opdrachten)
                comboBox2.Items.Add(opdracht);

        }

        private void TestChange(object sender, EventArgs e)
        {
            label4.Text = "0";

            string test = comboBox1.Text;
            
            if (test == "")
                return;

            TestVragen testVragen = ds.GetTestVragenClass();

            List<List<string>> alleTestVragen = testVragen.GetAllTestVragen();
            int alToegevoegdeVragen = 0;

            foreach(List<string> vragen in alleTestVragen)
            {
                if (vragen[0] == test)
                    alToegevoegdeVragen++;
            }

            label4.Text = Convert.ToString(alToegevoegdeVragen);
        }

        private void OpdrachtChange(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();

            string opdracht = comboBox2.Text;

            if (opdracht == "")
                return;

            List<List<string>> vragen = this.ds.GetQuestionsClass().GetAllQuestions();

            foreach (List<string> vraag in vragen)
            {
                if (vraag[0] == opdracht)
                    comboBox3.Items.Add(vraag[1]);
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string test = comboBox1.Text;
            string opdracht = comboBox2.Text;
            string vraag = comboBox3.Text;
            string amount = label4.Text;

            if (test == "")
                return;

            if (opdracht == "")
                return;

            if (vraag == "")
                return;

            TestVragen testVragen = ds.GetTestVragenClass();

            if (testVragen.QuestionAlreadyExists(test, opdracht, vraag))
                return;

            int iAmount = Convert.ToInt32(amount);

            iAmount++;

            amount = Convert.ToString(iAmount);

            testVragen.AddTestVraag(test, opdracht, vraag, amount);

            comboBox3.Text = "";

            label4.Text = amount;

        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
