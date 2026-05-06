namespace Word_Guessing_Game.Services
{
    public class WordProvider
    {
        private readonly List<string> wordlist;

        public WordProvider()
        {

            //hidden words
            wordlist = new List<string>
            {
                "APPLE",
                "MANGO",
                "GRAPE",
                "TRAIN",
                "PLANT",
                "BRAIN"
            };
        }

        public string GetRandomWord()
        {
            Random r = new Random();
            
            //getting a random index fromthe length of the wordlist
            int index = r.Next(wordlist.Count);

            return wordlist[index];


        }
    }
}