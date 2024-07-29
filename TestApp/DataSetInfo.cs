using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    internal class DataSetInfo
    {
        protected List<Games> gamesList = new();
        protected List<Players> playersList = new();
        protected List<Tests> testsList = new();
        protected List<Questions> questionsList = new();
        protected List<Options> optionsList = new();
        protected List<Antwoorden> antwoordenList = new();
        protected List<Running> runningList = new();
        protected List<string> settingsList = new();

        protected List<string> questions = new();
        protected List<string> options = new();
        protected List<string> games = new();
        protected List<string> tests = new();
        protected List<string> players = new();
        protected List<string> antwoorden = new();
        protected List<string> running = new();
        protected List<string> settings = new();

        protected string docPath;
        protected string path;

        public string fileQuestions;
        public string fileOptions;
        public string fileGames;
        public string fileTests;
        public string filePlayers;
        public string fileRunning;
        public string fileAntwoorden;
        public string fileSettings;

        public int currentGame;
        public int mole = -1;

        public DataSetInfo()
        {
            this.docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            this.path = Path.Combine(this.docPath, "widm/testprogram/");
            this.fileQuestions = Path.Combine(this.path, "questions.txt");
            this.fileOptions = Path.Combine(this.path, "options.txt");
            this.fileGames = Path.Combine(this.path, "games.txt");
            this.fileTests = Path.Combine(this.path, "tests.txt");
            this.filePlayers = Path.Combine(this.path, "players.txt");
            this.fileRunning = Path.Combine(this.path, "running.txt");
            this.fileAntwoorden = Path.Combine(this.path, "antwoorden.txt");
            this.fileSettings = Path.Combine(this.path, "settings.txt");

            this.ReadyList(this.path, this.fileGames, this.games, "games");
            this.ReadyList(this.path, this.filePlayers, this.players, "players");
            this.ReadyList(this.path, this.fileQuestions, this.questions, "questions");
            this.ReadyList(this.path, this.fileOptions, this.options, "options");
            this.ReadyList(this.path, this.fileTests, this.tests, "tests");
            this.ReadyList(this.path, this.fileRunning, this.running, "running");
            this.ReadyList(this.path, this.fileAntwoorden, this.antwoorden, "antwoorden");
            this.ReadyList(this.path, this.fileSettings, this.settings, "settings");
        }

        public void ReadyList(string path, string file, List<string> listing, string listName) 
        {
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            String? line;

            try
            {
                if(!File.Exists(file))
                {
                    FileStream fileCreated = File.Create(file);
                    fileCreated.Close();
                }   

                StreamReader reader = new(file);
                line = reader.ReadLine();
                while(line != null)
                {
                    listing.Add(line);
                    switch(listName)
                    {
                        case "games": this.AddGame(line); break;
                        case "players": this.AddPlayer(line); break;
                        case "questions": this.AddQuestion(line); break;
                        case "options": this.AddOption(line); break;
                        case "settings": this.SetSetting(line); break;
                        case "tests": this.AddTest(line); break;
                        case "running": this.AddRunning(line); break;
                        case "antwoorden": this.AddAntwoord(line); break;
                    }

                    line = reader.ReadLine();
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine(listing.Count);
            }
        }

        private void AddGame(string line)
        {
            string[] parts = line.Split('~');

            Games game = new();
            game.SetId(Convert.ToInt32(parts[0]));
            game.SetName(parts[1]);

            this.gamesList.Add(game);
        }

        private void AddTest(string line)
        {
            string[] parts = line.Split('~');

            Tests test = new();
            test.SetId(Convert.ToInt32(parts[0]));
            test.SetName(parts[1]);

            if (parts[2].Length > 0)
            {
                string[] questionsForTest = parts[2].Split(',');

                foreach (string q in questionsForTest)
                {
                    if (questionsList == null)
                        break;

                    foreach (Questions question in questionsList)
                    {
                        if (question.GetId() == Convert.ToInt32(q))
                            test.AddQuestion(question);
                    }
                }
            }

            this.testsList.Add(test);
        }

        private void AddPlayer(string line)
        {
            string[] parts = line.Split('~');

            Players player = new();
            player.SetId(Convert.ToInt32(parts[0]));
            player.SetName(parts[2]);
            player.SetGame(this.GetGameById(Convert.ToInt32(parts[1])));
            player.SetStatus(parts[3]);

            this.AddToPlayerList(player);
        }

        private void AddAntwoord(string line)
        {
            string[] parts = line.Split('~');

            Antwoorden antwoord = new();
            antwoord.SetId(Convert.ToInt32(parts[0]));
            antwoord.SetRunning(this.GetRunningByIdAlways(Convert.ToInt32(parts[1])));
            antwoord.SetOption(this.GetOptionById(Convert.ToInt32(parts[2])));

            this.AddToAntwoordList(antwoord);
        }

        private void AddRunning(string line)
        {
            string[] parts = line.Split('~');

            Running running = new();
            running.SetId(Convert.ToInt32(parts[0]));
            running.SetTest(this.GetTestById(Convert.ToInt32(parts[1])));
            running.SetPlayer(this.GetPlayerByIdAlways(Convert.ToInt32(parts[2])));
            running.SetStartTime(Convert.ToInt32(parts[3])); 
            running.SetEindTime(Convert.ToInt32(parts[4]));

            this.AddToRunningList(running);
        }

        private void AddQuestion(string line)
        {
            string[] parts = line.Split('~');

            Questions question = new();
            question.SetId(Convert.ToInt32(parts[0]));
            question.SetQuestion(parts[2]);
            question.SetGame(this.GetGameById(Convert.ToInt32(parts[1])));

            this.AddToQuestionList(question);
        }

        private void AddOption(string line)
        {
            string[] parts = line.Split('~');

            if (parts.Length != 4)
                return;

            Options option = new();
            option.SetId(Convert.ToInt32(parts[0]));
            option.SetValue(parts[2]);
            option.SetQuestion(this.GetQuestionById(Convert.ToInt32(parts[1])));

            if (parts[3].Length > 0)
            {
                string[] playerIds = parts[3].Split(',');

                foreach (string playerId in playerIds)
                {
                    Players? player = this.GetPlayerById(Convert.ToInt32(playerId));

                    if (player != null)
                        option.AddPlayer(player);
                }
            }

            this.AddToOptionList(option);
        }

        private void SetSetting(string line)
        {
            if (line.StartsWith("gameid="))
                this.currentGame = Convert.ToInt32(line.Split('=')[1]);

            if (line.StartsWith("specialPlayer="))
                this.mole = Convert.ToInt32(line.Split('=')[1]);

            this.settingsList.Add(line);
        }

        public void AddToGameList(Games game)
        {
            this.gamesList.Add(game);
        }

        public void AddToAntwoordList(Antwoorden antwoord)
        {
            this.antwoordenList.Add(antwoord);
        }

        public void AddToRunningList(Running running)
        {
            this.runningList.Add(running);
        }

        public void AddToPlayerList(Players player)
        {
            this.playersList.Add(player);
        }

        public void AddToQuestionList(Questions question)
        {
            this.questionsList.Add(question);
        }

        public void AddToOptionList(Options option)
        {
            this.optionsList.Add(option);
        }

        public void AddToTestList(Tests test)
        {
            this.testsList.Add(test);
        }

        public static void WriteInfo(string file, string info)
        {
            StreamWriter sw = new(file, true);
            sw.WriteLine(info);
            sw.Close();
        }

        public static int GetLineById(string file, int id)
        {
            int line = 0;
            
            string[] arrLine = File.ReadAllLines(file);

            foreach(string l in arrLine)
            {
                if (l.StartsWith(Convert.ToString(id) + '~'))
                    return line;
                
                line++;
            }

            throw new Exception("not found id " + Convert.ToString(id) + " for file " + file);
        }

        public static void UpdateLine(string file, string info, int index)
        {
            string[] arrLine = File.ReadAllLines(file);
            arrLine[index] = info;
            File.WriteAllLines(file, arrLine);
        }

        public void UpdateSetting(string infoToUpdate, string newValue)
        {
            string[] arrLine = File.ReadAllLines(this.fileSettings);
            Int32 key = 0;

            bool foundSetting = false;

            if(infoToUpdate == "gameid")
            {
                this.currentGame = Convert.ToInt32(newValue);
            }

            foreach(string line in arrLine)
            {
                if(line.StartsWith(infoToUpdate))
                {
                    foundSetting = true;
                    arrLine[key] = infoToUpdate + '=' + newValue;
                    break;
                }
                key++;
            }

            if(!foundSetting)
            {
                string info = infoToUpdate + '=' + newValue;
                StreamWriter sw = new(this.fileSettings, true);
                sw.WriteLine(info);
                sw.Close();
            }
            else
            {
                File.WriteAllLines(this.fileSettings, arrLine);
            }
        }

        public List<string> GetList(string listName)
        {
            switch (listName)
            {
                case "questions": return this.questions;
                case "options": return this.options;
                case "games": return this.games;
                case "players": return this.players;
                default:
                    break;
            }

            return new List<string>();
        }

        public List<Games>? GetGames()
        {
            return this.gamesList;
        }

        public List<Tests>? GetTests()
        {
            return this.testsList;
        }

        public List<Players>? GetPlayers()
        {
            return this.playersList;
        }

        public List<Questions>? GetQuestions()
        {
            return this.questionsList;
        }

        public List<Options>? GetOptions()
        {
            return this.optionsList;
        }

        public List<Antwoorden>? GetAntwoorden()
        {
            return this.antwoordenList;
        }

        public List<Running>? GetRunnings()
        {
            return this.runningList;
        }

        public List<string>? GetSettings()
        {
            return this.settingsList;
        }

        public Games GetGameById(int? id)
        {
            if(id == null)
                throw new Exception("Gameid should not be null");

            foreach (Games game in this.gamesList)
            {
                if (game.GetId() == id)
                    return game;
            }

            throw new Exception("Game not found with id " + Convert.ToString(id));
        }

        public Tests GetTestById(int id)
        {
            if(this.testsList == null)
                throw new Exception("No test created yet.");

            foreach (Tests t in this.testsList)
            {
                if (t.GetId() == id)
                    return t;
            }

            throw new Exception("Test not found with id " + Convert.ToString(id));
        }

        public Players GetPlayerByIdAlways(int? id)
        {
            if (id == null)
                throw new Exception("Playerid should not be null");

            foreach (Players player in this.playersList)
            {
                if (player.GetId() == id)
                    return player;
            }

            throw new Exception("Player not found with id " + Convert.ToString(id));
        }

        public Players? GetPlayerById(int? id)
        {
            if (id == null)
                return null;

            foreach (Players player in this.playersList)
            {
                if (player.GetId() == id)
                    return player;
            }

            return null;
        }

        public Questions? GetQuestionById(int? id)
        {
            if (id == null)
                return null;

            foreach (Questions question in this.questionsList)
            {
                if (question.GetId() == id)
                    return question;
            }

            return null;
        }

        public Options? GetOptionById(int? id)
        {
            if (id == null)
                return null;

            foreach (Options option in this.optionsList)
            {
                if (option.GetId() == id)
                    return option;
            }

            return null;
        }

        public Running GetRunningByIdAlways(int? id)
        {
            if (id == null)
                throw new Exception("Runningid should not be null");

            foreach (Running running in this.runningList)
            {
                if (running.GetId() == id)
                    return running;
            }

            throw new Exception("Running not found with id " + Convert.ToString(id));
        }
    }
}
