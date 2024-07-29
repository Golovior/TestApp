using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class Tests
    {
        protected int? id;
        protected List<Questions> questions;
        protected string? name;

        public Tests()
        {
            this.questions = new List<Questions>();
        }

        public int? GetId()
        {
            return this.id;
        }

        public string? GetName()
        {
            return this.name;
        }

        public void SetId(int? id)
        {
            this.id = id;
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
                {
                    this.questions.Remove(q);
                    break;
                }
            }
        }

        public void WriteToFile()
        {
            DataSetInfo dsi = Program.GetInfo();

            string line = this.InfoForFile();

            DataSetInfo.WriteInfo(dsi.fileTests, line);
        }

        public void UpdateInFile(string? questionLine = null)
        {
            DataSetInfo dsi = Program.GetInfo();
            string line = this.InfoForFile(questionLine);

            if (this.id == null)
                return;

            int index = DataSetInfo.GetLineById(dsi.fileTests, (int)this.id);

            DataSetInfo.UpdateLine(dsi.fileTests, line, index);
        }

        private string InfoForFile(string? questionLine = null)
        {
            if(questionLine == null)
            {
                questionLine = "";

                if (this.questions != null)
                {
                    foreach (Questions q in this.questions)
                    {
                        if (questionLine.Length > 0)
                            questionLine += ',';

                        questionLine += Convert.ToString(q.GetId());
                    }
                }
            }

            return this.GetId() + "~" + this.GetName() + "~" + questionLine;
        }
    }
}
