using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class Players
    {
        protected int? id;
        protected Games? game;
        protected string? name;
        protected string? status;

        public int? GetId()
        {
            return this.id;
        }
        public Games? GetGame()
        {
            return this.game;
        }
        public string? GetName()
        {
            return this.name;
        }
        public string? GetStatus()
        {
            return this.status;
        }

        public void SetId(int? id)
        {
            this.id = id;
        }

        public void SetGame(Games? game)
        {
            if (this.game != null)
            {
                this.game.RemovePlayer(this);
            }

            this.game = game;

            if (this.game != null)
                this.game.AddPlayers(this);
        }

        public void SetName(string? name)
        {
            this.name = name;
        }

        public void SetStatus(string? status)
        {
            this.status = status;
        }

        public void WriteToFile()
        {
            DataSetInfo dsi = Program.GetInfo();
            string? line = this.InfoForFile();

            if (line == null)
                return;

            DataSetInfo.WriteInfo(dsi.filePlayers, line);

        }

        public void UpdateInFile()
        {
            DataSetInfo dsi = Program.GetInfo();
            string? line = this.InfoForFile();

            if (line == null || this.id == null)
                return;

            int index = DataSetInfo.GetLineById(dsi.filePlayers, (int) this.id);

            DataSetInfo.UpdateLine(dsi.filePlayers, line, index);
        }

        private string? InfoForFile() 
        {
            game = this.GetGame();

            if (game == null)
                return null;

            return this.GetId() + "~" + game.GetId() + "~" + this.GetName() + '~' + this.GetStatus();
        }
    }
}
