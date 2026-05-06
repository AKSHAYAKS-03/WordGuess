using System;

namespace Word_Guessing_Game.Services
{
    public class FeedbackGenerator
    {
        public string GenerateFeedback(string hiddenword,string guess)
        {

            // string to return the feedback
            char[] feedback = new char[guess.Length];

            bool[] used = new bool[hiddenword.Length];

            for(int i=0; i < guess.Length; i++)
            {
                if(guess[i] == hiddenword[i])
                {
                    feedback[i] = 'G';
                    used[i] = true;
                }
            }

            for(int i=0; i < guess.Length; i++){
                
                if(feedback[i] == 'G')
                {
                    continue;
                }
                bool found = false;
                for (int j = 0; j < hiddenword.Length; j++)
                {
                    // check unused matching letter
                    if (!used[j] && guess[i] == hiddenword[j])
                    {
                        found = true;
                        used[j] = true;
                        break;
                    }
                }
                if (found)
                {
                    feedback[i] = 'Y';
                }
                else
                {
                    feedback[i] = 'X';
                }
            }
            
            return new string(feedback);
        }
    }
}
