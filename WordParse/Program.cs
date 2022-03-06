using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WordParse
{
    public class Program
    {
        private const int WORDLE_LENGTH = 5;
        public static void Main(string[] args)
        {
            string text = System.IO.File.ReadAllText(@"src\words.txt");
            List<string> words = text.Split("\n").ToList();
            StringBuilder sb = new StringBuilder();

            foreach (string curr_word in words)
            {
                if (curr_word.Length == WORDLE_LENGTH)
                {
                    if (containsOnlyLetters(curr_word))
                    {
                        sb.AppendLine(curr_word);
                    }
                }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"out.txt"))
            {
                file.WriteLine(sb.ToString()); // "sb" is the StringBuilder
            }
        }

        public static bool containsOnlyLetters(string word_to_check)
        {
            bool is_clean_word = true;
            foreach (char _char in word_to_check)
            {
                if (is_clean_word)
                {
                    bool isLower = (_char >= 97) && (_char <= 122);
                    bool isUpper = (_char >= 65) && (_char <= 90);
                    is_clean_word = isLower || isUpper;
                }
            }
            return is_clean_word;
        }
    }
}
