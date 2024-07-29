using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class Options
    {
        protected int? id;
        protected Questions? question;
        protected string? value;
        protected List<Players>? players;

        public int? GetId()
        {
            return this.id;
        }

        public Questions? GetQuestion()
        {
            return this.question;
        }

        public string? GetValue()
        {
            return this.value;
        }

        public List<Players>? GetPlayers()
        {
            return this.players;
        }

        public void SetId (int? id)
        {
            this.id = id;
        }

        public void SetQuestion (Questions? question)
        {
            this.question = question;
            if(question != null)
                question.AddOptions(this);
        }

        public void SetValue (string? value)
        {
            this.value = value;
        }

        public void ClearPlayers()
        {
            this.players = new List<Players>();
        }

        public void AddPlayer (Players player)
        {
            if(this.players == null)
                this.players = new List<Players>();

            this.players.Add(player);
        }

        public void RemovePlayer (Players player)
        {
            if (this.players == null)
                return;

            foreach (Players p in this.players)
            {
                if (p.GetId() == player.GetId())
                    this.players.Remove(p);
            }
        }

        public void UpdateInFile()
        {
            DataSetInfo dsi = Program.GetInfo();
            string line = this.InfoForFile();

            if (this.id == null)
                return;

            int index = DataSetInfo.GetLineById(dsi.fileOptions, (int) this.id);

            DataSetInfo.UpdateLine(dsi.fileOptions, line, index);
        }

        public void WriteToFile()
        {
            DataSetInfo dsi = Program.GetInfo();
            string line = this.InfoForFile();

            DataSetInfo.WriteInfo(dsi.fileOptions, line);

        }

        private string InfoForFile()
        {
            Questions? q = this.GetQuestion();

            if (q == null)
                throw new Exception("Question must be set for option before saving.");

            List<Players>? players = this.GetPlayers();
            string playerString = "";

            if (players != null)
            {
                bool firstOption = true;
                foreach (Players p in players)
                {
                    if (!firstOption)
                        playerString += ",";

                    playerString += p.GetId();
                    firstOption = false;
                }
            }

            return this.GetId() + "~" + q.GetId() + "~" + this.GetValue() + "~" + playerString;
        }
    }
}
