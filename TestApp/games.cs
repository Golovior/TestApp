using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class Games
    {
        protected int? id;
        protected List<Players>? players;
        protected List<Questions> questions;
        protected string? name;

        public Games ()
        {
            this.players = new List<Players>();
            this.questions = new List<Questions>();
        }

        public int? GetId()
        {
            return this.id;
        }

        public List<Players>? GetPlayers()
        {
            return this.players;
        }

        public string? GetName()
        {
            return this.name;
        }

        public void SetId(int? id)
        {
            this.id = id;
        }

        public void AddPlayers(Players player)
        {
            if (this.players == null)
                this.players = new List<Players>();

            this.players.Add(player);
        }

        public void RemovePlayer(Players player)
        {
            if (this.players == null)
                return;

            foreach(Players p in this.players)
            {
                if(p.GetId() == player.GetId())
                    this.players.Remove(p);
            }
        }

        public void SetName(string? name)
        {
            this.name = name;
        }

        public List<Questions>? GetQuestions()
        {
            return this.questions;
        }

        public void AddQuestion(Questions question)
        {
            if (this.questions == null)
                this.questions = new List<Questions>();

            this.questions.Add(question);
        }

        public void RemoveQuestion(Questions question)
        {
            if (this.questions == null)
                return;

            foreach (Questions q in this.questions)
            {
                if (q.GetId() == question.GetId())
                    this.questions.Remove(q);
            }
        }

        public void WriteToFile()
        {
            DataSetInfo dsi = Program.GetInfo();

            string line = this.GetId() + "~" + this.GetName();

            DataSetInfo.WriteInfo(dsi.fileGames, line);
        }
    }
}
