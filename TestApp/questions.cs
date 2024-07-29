using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class Questions
    {
        protected int? id;
        protected Games? game;
        protected string? question;
        protected List<Options>? options;

        public Questions()
        {
            this.options = new List<Options>();
        }

        public int GetId()
        {
            if (id == null)
                throw new Exception("This should not be possible....");

            return (int) id;
        }

        public Games GetGame()
        {
            if (this.game == null)
                throw new Exception("Expected a game. Null available.");

            return this.game;
        }

        public Games? GetGameOrNull()
        {
            return this.game;
        }

        public string? GetQuestion()
        {
            return this.question;
        }

        public List<Options>? GetOptions()
        {
            return this.options;
        }

        public void SetId(int? id)
        {
            this.id = id;
        }

        public void SetGame(Games? game)
        {
            this.game = game;
            if (game != null)
                game.AddQuestion(this);
        }

        public void SetQuestion(string? question)
        {
            this.question = question;
        }

        public void AddOptions(Options option)
        {
            if (this.options == null)
                this.options = new List<Options>();

            this.options.Add(option);
        }

        public void RemoveOption(Options option)
        {
            if (this.options == null)
                return;

            foreach (Options o in this.options)
            {
                if (o.GetId() == option.GetId())
                    this.options.Remove(o);
            }
        }

        public void WriteToFile()
        {
            DataSetInfo dsi = Program.GetInfo();

            string line = this.GetId() + "~" + this.GetGame().GetId() + "~" + this.GetQuestion();

            DataSetInfo.WriteInfo(dsi.fileQuestions, line);
            
        }
    }
}
