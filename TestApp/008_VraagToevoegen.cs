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
    public partial class Form3 : Form
    {
        readonly Form previous;

        public Form3(Form prev)
        {
            InitializeComponent();
            previous = prev;
        }

        private void CloseApplication(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DataSetInfo dataSetInfo = Program.GetInfo();
            int? gameId = dataSetInfo.currentGame;

            if (gameId == null)
                throw new Exception("GameId must be set. Add 'gameid=' to Setting file");

            Games Game = dataSetInfo.GetGameById(gameId);

            List<Questions>? list = dataSetInfo.GetQuestions();

            if (list == null)
                throw new Exception("List must be set. Check questions list");


            string questionValue = textBox1.Text;

            Questions question = new();
            question.SetQuestion(questionValue);
            question.SetId(list.Count);
            question.SetGame(Game);

            question.WriteToFile();
            dataSetInfo.AddToQuestionList(question);

            textBox1.Text = "";
            textBox1.Focus();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            previous.Show();
        }
    }
}
