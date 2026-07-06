using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    internal class DataSetClass
    {
        readonly Games games = new();
        readonly Settings settings = new();
        readonly Opdrachten opdrachten = new();
        readonly Questions questions = new();
        readonly Antwoorden antwoorden = new();
        readonly Tests tests = new();
        readonly TestVragen testVragen = new();
        readonly TestAntwoorden testAntwoorden = new();
        readonly TestAfnamen testAfnamen = new();
        readonly GameSpelers gameSpelers = new();
        readonly Spelers spelers = new();
        readonly Api api;

        public DataSetClass() {
            api = new(this);
        }

        public Games GetGamesClass()
        {
            return games;
        }

        public Settings GetSettingsClass()
        {
            return settings;
        }

        public Opdrachten GetOpdrachtenClass()
        {
            return opdrachten;
        }

        public Questions GetQuestionsClass()
        {
            return questions;
        }

        public Antwoorden GetAntwoordenClass()
        {
            return antwoorden;
        }

        public Tests GetTestsClass()
        {
            return tests;
        }

        public TestVragen GetTestVragenClass()
        {
            return testVragen;
        }

        public TestAntwoorden GetTestAntwoordenClass()
        {
            return testAntwoorden;
        }

        public TestAfnamen GetTestAfnamenClass()
        {
            return testAfnamen;
        }

        public GameSpelers GetGameSpelersClass()
        {
            return gameSpelers;
        }

        public Spelers GetSpelersClass()
        {
            return spelers;
        }

        public Api GetApiClass()
        {
            return api;
        }
    }
}
