using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class Running
    {
        int? id;
        Tests? test;
        Players? player;
        int? startTime;
        int? eindTime;

        public void SetId(int? id)
        {
            this.id = id;
        }

        public void SetTest(Tests? test)
        {
            this.test = test;
        }
        public void SetPlayer(Players? player)
        {
            this.player = player;
        }

        public void SetStartTime(int time)
        {
            this.startTime = time;
        }

        public void SetEindTime(int time)
        {
            this.eindTime = time;
        }

        public int? GetId()
        {
            return this.id;
        }

        public Players? GetPlayer()
        {
            return this.player;
        }

        public Tests? GetTest()
        {
            return this.test;
        }

        public int? GetStartTime()
        {
            return this.startTime;
        }

        public int? GetEindTime()
        {
            return this.eindTime;
        }

        public void WriteToFile()
        {
            DataSetInfo dsi = Program.GetInfo();
            string? line = this.InfoForFile();

            if (line == null)
                return;

            DataSetInfo.WriteInfo(dsi.fileRunning, line);

        }

        public void UpdateInFile()
        {
            DataSetInfo dsi = Program.GetInfo();
            string? line = this.InfoForFile();

            if (line == null || this.id == null)
                return;

            int index = DataSetInfo.GetLineById(dsi.fileRunning, (int)this.id);

            DataSetInfo.UpdateLine(dsi.fileRunning, line, index);
        }

        private string? InfoForFile()
        {
            player = this.GetPlayer();

            if (player == null)
                return null;

            test = this.GetTest();

            if (test == null)
                return null;

            return this.GetId() + "~" + test.GetId()  + "~" + player.GetId() + "~" + Convert.ToString(this.GetStartTime()) + '~' + Convert.ToString(this.GetEindTime());
        }

    }
}
