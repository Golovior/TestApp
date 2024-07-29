using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class Antwoorden
    {
        int? id;
        Running? running;
        Options? option;

        public void SetId(int? id)
        {
            this.id = id;
        }

        public void SetRunning(Running? running)
        {
            this.running = running;
        }

        public void SetOption(Options? option)
        {
            this.option = option;
        }

        public int GetId()
        {
            if (this.id == null)
                throw new Exception("Er moet een id voor een antwoord gezet zijn.");

            return (int) this.id;
        }

        public Running GetRunning()
        {
            if (this.running == null)
                throw new Exception("Er moet een running voor een antwoord gezet zijn.");

            return this.running;
        }

        public Options GetOption()
        {
            if (this.option == null)
                throw new Exception("Er moet een option voor een antwoord gezet zijn.");

            return this.option;
        }

        public bool CheckAntwoord()
        {
            if (option == null)
                return false;

            List<Players>? playersByOption = option.GetPlayers();

            if (playersByOption == null)
                return false;

            DataSetInfo dsi = Program.GetInfo();

            foreach(Players p in playersByOption)
            {
                if(p.GetId() == dsi.mole)
                {
                    return true;
                }
            }

            return false;
        }

        public void WriteToFile()
        {
            DataSetInfo dsi = Program.GetInfo();

            string line = this.GetId() + "~" + this.GetRunning().GetId() + "~" + this.GetOption().GetId();

            DataSetInfo.WriteInfo(dsi.fileAntwoorden, line);

        }
    }
}
