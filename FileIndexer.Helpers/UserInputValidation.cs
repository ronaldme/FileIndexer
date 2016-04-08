using System.Linq;

namespace FileIndexer.Helpers
{
    public class UserInputValidation
    {
        /// <summary>
        /// Simply validates if the format is correct.
        /// No checks for file locations etc.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ValidInputFormat(string input)
        {
            var results = input.Split(' ');

            if (results.Any(string.IsNullOrEmpty) || (results.Length != 2 && results.Length != 3))
            {
                return false;
            }

            return true;
        }

        public static bool ValidInputFormat(string mainPath, string syncPath, bool delete)
        {
            return ValidInputFormat(delete ?
                $"{mainPath} {syncPath} -d" :
                $"{mainPath} {syncPath}");
        }
    }
}