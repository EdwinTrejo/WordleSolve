#define DEBUGGING

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace WordGuesser
{
    public class Program
    {
        private const int WORDLE_LENGTH = 5;

        private const string TEMP_WRD = "w___s";
        private const string TEMP_AS_LE = "or";

        private static List<string> WORDLE_SIZE_WORDS = new List<string>();

        private static IEnumerable<string> word_list
        {
            get
            {
                if (WORDLE_SIZE_WORDS.Count == 0)
                {
                    string text = System.IO.File.ReadAllText(@"src\out.txt");
                    WORDLE_SIZE_WORDS = text.Split("\r\n").ToList();
                    WORDLE_SIZE_WORDS = WORDLE_SIZE_WORDS.Select(x => x).Where(c => c.Length == WORDLE_LENGTH).ToList();
                }
                return WORDLE_SIZE_WORDS;
            }
        }

        private static string Version
        {
            get
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
                return String.Format("{0}.{1}", fvi.ProductMajorPart, fvi.ProductMinorPart);
            }
        }

        private static string Instructions
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"using .NET core {Environment.Version.ToString()}");
                sb.AppendLine($"WordleSolve WordGuesser {Version}");
                sb.AppendLine("Guide\n\tWord must consist of only letters and _ for unkown!!!");
                sb.AppendLine("\texample: ma__c for magic");
                sb.AppendLine("\tfor associated letters, type each letter once");
                sb.AppendLine("\texample: ci");
                return sb.ToString();
            }
        }

        public static void Main(string[] args)
        {
            bool can_use = false;
            string curr_word = string.Empty;
            HashSet<char> associated_letters = new HashSet<char>();

            Console.WriteLine(Instructions);

            while (!can_use)
            {
                Console.Write("\nEnter word: ");
#if DEBUGGING
                string new_guess = TEMP_WRD;
#else
                string new_guess = Console.ReadLine();
#endif
                if (new_guess.Length == WORDLE_LENGTH && containsOnlyLetters(new_guess, new List<char>() { '_' }))
                {
                    can_use = true;
                    curr_word = new_guess;
                }
            }
#if DEBUGGING
            Console.Write("\nEnter Associated Letters: ");
#else
            string as_le = Console.ReadLine();
#endif
            string as_le = TEMP_AS_LE;
            foreach (var let in as_le)
            {
                if (containsOnlyLetters(let.ToString(), null))
                    associated_letters.Add(let);
            }

            Console.WriteLine($"\nUsing word\t\t: {curr_word}");
            Console.WriteLine($"With associated letters\t: {listToString(associated_letters)}");

            List<string> copy_list = word_list.ToList();

            if (copy_list.Count() == 0)
                throw new ArgumentNullException();

            foreach (var itm_wrd in copy_list)
            {
                if (itm_wrd.Length != WORDLE_LENGTH)
                    throw new DataMisalignedException();
            }

            for (int i = 0; i < WORDLE_LENGTH; i++)
            {
                char _char = curr_word[i];
                if (_char != '_' && copy_list.Count() > 0)
                {
                    List<string> mod_copy_list =
                        (
                        from this_word in copy_list
                        where this_word[i] == _char
                        select this_word
                         ).ToList();
                    copy_list = mod_copy_list;
                }
                Console.WriteLine($"\nKnow letter round[{i + 1}]: Possible Guesses: {copy_list.Count()}");
            }

            if (copy_list.Count() > 0)
            {
                int round = 0;
                foreach (char _char in associated_letters)
                {
                    if (copy_list.Count() > 0)
                    {
                        List<string> mod_copy_list =
                            (
                            from this_word in copy_list
                            where this_word.Contains(_char)
                            select this_word
                             ).ToList();
                        copy_list = mod_copy_list;
                    }
                    Console.WriteLine($"\nAssociated letter[{++round}]: Possible Guesses: {copy_list.Count()}");
                }
            }

            Console.WriteLine();

            if (copy_list.Count() > 0)
            {
                foreach (var candidate in copy_list)
                {
                    Console.WriteLine($"Candidate: {candidate}");
                }
            }
            else
            {
                Console.WriteLine("NO English words found. I think you did something wrong!");
            }
        }

        public static string listToString(IEnumerable<char> list_to_read)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var let in list_to_read)
            {
                sb.Append(let);
            }
            return sb.ToString();
        }

        public static bool containsOnlyLetters(string word_to_check, List<char>? additional_checks)
        {
            bool is_clean_word = true;
            foreach (char _char in word_to_check)
            {
                if (is_clean_word)
                {
                    bool isLower = (_char >= 97) && (_char <= 122);
                    bool isUpper = (_char >= 65) && (_char <= 90);
                    bool additional_over_checked_and_found = false;
                    if (additional_checks != null)
                    {
                        foreach (char _add_check in additional_checks)
                        {
                            if (!additional_over_checked_and_found)
                            {
                                additional_over_checked_and_found = _char == _add_check;
                            }
                        }
                    }
                    is_clean_word = isLower || isUpper || additional_over_checked_and_found;
                }
            }
            return is_clean_word;
        }
    }
}
